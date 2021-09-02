using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Catalog.Repositories;
using Catalog.Settings;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using System.IO;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;

namespace learn_net5_webapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // The container here refers to ServiceContainer, which will responsible for dependency injection
        public void ConfigureServices(IServiceCollection services)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));
            var mongoDbSettings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
            services.AddSingleton<IMongoClient>(serviceProvider =>
            {
                // "MongoDbSetting" in appsettings.json
                // Mongo database password was injected from dotnet user-secrets instead of appsettings.json
                // .Net will automatically overwrite the settings in appsettings.json, if it exists in environments, or user-secret
                return new MongoClient(mongoDbSettings.ConnectionString);
            });
            // Add Cors policies, basically it's rules  
            services.AddCors(sc => sc.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyHeader()
                .AllowAnyMethod().AllowAnyOrigin();
            }));
            services.AddSingleton<IItemsRepository, MongoItemRepository>();
            services.AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false; // Disable auto-remove controller method async suffix
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "learn_net5_webapi", Version = "v1" });
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "learn-net5-webapi.xml"); // Refer to {project_name}.xml in bin
                c.IncludeXmlComments(filePath); // Enable swagger comment annotation
                c.EnableAnnotations(); // Swashbuckle.AspNetCore.Annotations, which enable [SwaggerOperation()] [SwaggerResponse()] and others annotation
            });

            // Add health check service (helpful for kurbenetes)
            services.AddHealthChecks().AddMongoDb(mongoDbSettings.ConnectionString, name: "mongodb", timeout: TimeSpan.FromSeconds(5), tags: new[] { "dbready" });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger(); // Middleware
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "learn_net5_webapi v1"));
            }
            app.UseCors("AllowAll"); // AllowAll = Policy added at ConfigureServices
            // app.UseHttpsRedirection(); // Causes issue in docker port mapping
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                // Check mongodb is healthy or not, mongodb health check was tag with "ready" at ConfigureServices
                endpoints.MapHealthChecks("/health/dbready", new HealthCheckOptions
                {
                    // Predicate = Filter only the health check type we want for this endpoint
                    Predicate = (check) => check.Tags.Contains("dbready"),
                    // Custom response for health check API
                    ResponseWriter = async (context, report) =>
                    {
                        var result = JsonSerializer.Serialize(
                            new
                            {
                                status = report.Status.ToString(),
                                checks = report.Entries.Select(entry => new
                                {
                                    name = entry.Key,
                                    status = entry.Value.Status.ToString(),
                                    exception = entry.Value.Exception != null ? entry.Value.Exception.Message : null,
                                    duration = entry.Value.Duration.ToString()
                                })
                            }
                        );
                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        await context.Response.WriteAsync(result);
                    }
                });
                // Check only our REST api service, without others
                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
                {
                    Predicate = (_) => false
                });
            });
        }
    }
}
