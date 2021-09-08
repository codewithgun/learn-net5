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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Catalog.Context;
using Microsoft.EntityFrameworkCore;
using Catalog.Entities;
using System.Threading;

namespace learn_net5_webapi
{
    public class Startup
    {
        // When a function return "Task", it represent the task can be async
        // Unlike javascript, when a function is async, the function will be put to into nextTick queue
        // Which means, in javascript, the async function execution will be immediately postponed
        // Output: Start -> End -> Doing some heavy work -> Finish heavy work (change await Task.Delay() to Thread.Sleep())

        // In C#, you to need use await on heavy task to free-up the thread to do other task
        // public async Task IsItAsync()
        // {
        //     Console.WriteLine("Doing some heavy work");
        //     await Task.Delay(10000);
        //     Console.WriteLine("Finish heavy work");
        // }

        // public async void Test()
        // {
        //     Console.WriteLine("Start");
        //     var task = this.IsItAsync();
        //     Console.WriteLine("End");
        //     await task;
        // }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            // this.Test();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // The container here refers to ServiceContainer, which will responsible for dependency injection
        public void ConfigureServices(IServiceCollection services)
        {
            //
            var jwtSetting = Configuration.GetSection(nameof(JwtSetting)).Get<JwtSetting>();

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

            // Build connection string for Postgresql
            var postgresDbSettings = Configuration.GetSection(nameof(PostgresDbSettings)).Get<PostgresDbSettings>();
            services.AddDbContext<PostgresDbContext>(options =>
            {
                options.UseNpgsql(postgresDbSettings.ConnectionString);
            });

            // Tell .net core to use JwtBearer for authentication 
            // https://stackoverflow.com/questions/46223407/asp-net-core-2-authenticationschemes
            // DefaultScheme: if specified, all the other defaults will fallback to this value
            // DefaultAuthenticateScheme: if specified, AuthenticateAsync() will use this scheme, and also the AuthenticationMiddleware added by UseAuthentication() will use this scheme to set context.User automatically. (Corresponds to AutomaticAuthentication)
            // DefaultChallengeScheme if specified, ChallengeAsync() will use this scheme, [Authorize] with policies that don't specify schemes will also use this
            // DefaultSignInScheme is used by SignInAsync() and also by all of the remote auth schemes like Google/Facebook/OIDC/OAuth, typically this would be set to a cookie.
            // DefaultSignOutScheme is used by SignOutAsync() falls back to DefaultSignInScheme
            // DefaultForbidScheme is used by ForbidAsync(), falls back to DefaultChallengeScheme
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwt =>
            {
                var secretKey = Encoding.ASCII.GetBytes(jwtSetting.Secret);
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    RequireExpirationTime = false,
                };
            });


            // Add Cors policies, basically it's rules  
            services.AddCors(sc => sc.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyHeader()
                .AllowAnyMethod().AllowAnyOrigin();
            }));
            // Mongodb repository implementation
            services.AddScoped<IItemsRepository, MongoItemRepository>();
            services.AddScoped<ICategoryRepository, MongoCategoryRepository>();

            // In-memory repository implementation
            // services.AddSingleton<IItemsRepository, InMemoryItemsRepository>();
            // services.AddSingleton<ICategoryRepository, InMemoryCategoryRepository>();

            // Postgresql repository implementation
            // services.AddScoped<IItemsRepository, PgItemRepository>();
            // services.AddScoped<ICategoryRepository, PgCategoryRepository>();
            services.AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false; // Disable auto-remove controller method async suffix
            });
            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo { Title = "learn_net5_webapi", Version = "v1" });
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "learn-net5-webapi.xml"); // Refer to {project_name}.xml in bin
                swagger.IncludeXmlComments(filePath); // Enable swagger comment annotation
                swagger.EnableAnnotations(); // Swashbuckle.AspNetCore.Annotations, which enable [SwaggerOperation()] [SwaggerResponse()] and others annotation
                // Add the "Authorize" button in the swagger page
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT authorization header using the bearer schema"
                });
            });

            // Add health check service (helpful for kurbenetes)
            services.AddHealthChecks().AddMongoDb(mongoDbSettings.ConnectionString, name: "mongodb", timeout: TimeSpan.FromSeconds(5), tags: new[] { "dbready" });
            // services.AddHealthChecks().AddNpgSql(postgresDbSettings.ConnectionString, timeout: TimeSpan.FromSeconds(5), tags: new[] { "dbready" });
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
