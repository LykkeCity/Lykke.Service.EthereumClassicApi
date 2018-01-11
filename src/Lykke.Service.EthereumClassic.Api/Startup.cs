using System;
using Lykke.Service.EthereumClassic.Api.Actors.Extensions;
using Lykke.Service.EthereumClassic.Api.Common.Settings;
using Lykke.SettingsReader;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;


namespace Lykke.Service.EthereumClassic.Api
{
    public class Startup
    {
        private readonly IReloadingManager<AppSettings> _settings;
        

        public Startup(IHostingEnvironment env)
        {
            _settings = LoadSettings(env);
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            app
                .UseMvc()
                .UseSwagger(SetupSwagger)
                .UseSwaggerUI(SetupSwaggerUI)
                .UseStaticFiles();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc();

            services
                .AddActorSystem(_settings);

            services
                .AddSwaggerGen(SetupSwaggerGen);
            
            return services.BuildServiceProvider();
        }

        private static IReloadingManager<AppSettings> LoadSettings(IHostingEnvironment environment)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddEnvironmentVariables()
                .Build();

            return configuration.LoadSettings<AppSettings>();
        }

        private static void SetupSwagger(SwaggerOptions options)
        {
            options.PreSerializeFilters.Add
            (
                (swagger, httpReq) => swagger.Host = httpReq.Host.Value
            );
        }

        private static void SetupSwaggerGen(SwaggerGenOptions options)
        {
            options.SwaggerDoc("v1", new Info { Title = "Ethereum Classic API", Version = "v1" });
        }

        private static void SetupSwaggerUI(SwaggerUIOptions options)
        {
            options.RoutePrefix = "swagger/ui";

            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        }
    }
}