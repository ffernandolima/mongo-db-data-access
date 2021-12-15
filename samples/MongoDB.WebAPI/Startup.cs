using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Data;
using MongoDB.Infrastructure.Abstractions;
using MongoDB.Infrastructure.Extensions;
using MongoDB.Models;
using MongoDB.UnitOfWork.Abstractions;
using MongoDB.UnitOfWork.Abstractions.Extensions;
using MongoDB.WebAPI.Swagger.Filters;
using MongoDB.WebAPI.Swagger.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Reflection;

namespace MongoDB.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            services.AddVersionedApiExplorer(options =>
            {
                // Format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            services.AddMvc().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.MetadataPropertyHandling = MetadataPropertyHandling.Ignore;
                options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            services.AddMongoDbContext<IMongoDbContext, BloggingContext>(provider =>
            {
                var connectionString = Configuration.GetValue<string>("MongoSettings:ConnectionString");
                var databaseName = Configuration.GetValue<string>("MongoSettings:DatabaseName");

                var bloggingContext = new BloggingContext(connectionString, databaseName);

                return bloggingContext;
            });

            services.AddMongoDbUnitOfWork();
            services.AddMongoDbUnitOfWork<BloggingContext>();

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllParametersInCamelCase();
                options.OperationFilter<SwaggerDefaultValues>();

                var appPath = AppDomain.CurrentDomain.BaseDirectory;

                var entryAssembly = Assembly.GetEntryAssembly();
                var assemblyName = entryAssembly?.GetName();
                var appName = assemblyName?.Name;

                var filePath = Path.Combine(appPath, $"{appName ?? "EntityFrameworkCore.WebAPI"}.xml");

                options.IncludeXmlComments(filePath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"API {description.GroupName.ToUpperInvariant()}");
                }
            });

            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

            using var serviceScope = serviceScopeFactory.CreateScope();

            var context = serviceScope.ServiceProvider.GetService<BloggingContext>();

            var databaseName = Configuration.GetValue<string>("MongoSettings:DatabaseName");
            context.Client.DropDatabase(databaseName);

            var unitOfWork = serviceScope.ServiceProvider.GetService<IMongoDbUnitOfWork>();
            var repository = unitOfWork.Repository<Blog>();

            if (!repository.Any())
            {
                var blogs = Seeder.SeedBlogs();

                repository.InsertMany(blogs);
                unitOfWork.SaveChanges();
            }
        }
    }
}
