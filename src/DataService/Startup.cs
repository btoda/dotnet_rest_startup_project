using System.Reflection;
using Data.Models;
using DataService.Features;
using DataService.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Hosting;

namespace DataService
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
        private void AddRestModel<T>(IServiceCollection services, GenericControllerActionVisibility actionVisibility = GenericControllerActionVisibility.All) where T : Data.Models.Metadata
        {
            services.AddTransient<IRepository<T>, Repository<T>>();
            Features.EntityTypes.Types.Add(typeof(T).GetTypeInfo());
            Features.EntityTypes.ActionVisibility[typeof(T).FullName] = actionVisibility;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // register routes for generic controllers
            services.AddMvc(o => o.Conventions.Add(
                new GenericControllerRouteConvention()
            ))
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            })
            .ConfigureApplicationPartManager(apm =>
                apm.FeatureProviders.Add(new GenericControllerFeatureProvider()));
            services.AddControllers();
            services.AddSwaggerGen(a =>
            {
                a.OperationFilter<Swagger.ActionVisibilityOperationFilter>();
                a.DocumentFilter<Swagger.ActionVisibilityDocumentFilter>();
                a.OperationFilter<Swagger.AddRequiredHeaderParameters>();
            });
            services.Configure<Config>(Configuration);

            // ADD Your rest models registration here
            AddRestModel<Product>(services, GenericControllerActionVisibility.All);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DataService rest service API v1.");
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
