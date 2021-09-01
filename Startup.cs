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
            services.AddSingleton<IMongoClient>(serviceProvider =>
            {
                // "MongoDbSetting" in appsettings.json
                var settings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
                return new MongoClient(settings.ConnectionString);
            });
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

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
