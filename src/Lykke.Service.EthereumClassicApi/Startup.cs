using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common.Log;
using FluentValidation.AspNetCore;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Common.Chaos;
using Lykke.Logs;
using Lykke.Service.EthereumClassicApi.Actors;
using Lykke.Service.EthereumClassicApi.Blockchain;
using Lykke.Service.EthereumClassicApi.Common.Exceptions;
using Lykke.Service.EthereumClassicApi.Common.Settings;
using Lykke.Service.EthereumClassicApi.Filters;
using Lykke.Service.EthereumClassicApi.Modules;
using Lykke.Service.EthereumClassicApi.Repositories;
using Lykke.Service.EthereumClassicApi.Services;
using Lykke.SettingsReader;
using Lykke.SlackNotification.AzureQueue;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;


namespace Lykke.Service.EthereumClassicApi
{
    public class Startup
    {
        private readonly IHostingEnvironment _environment;
        private readonly IReloadingManager<AppSettings> _settings;

        private IActorSystemFacade _actorSystemFacade;
        private IContainer _container;
        private ILog _log;


        public Startup(IHostingEnvironment environment)
        {
            _environment = environment;
            _log = new LogToConsole();
            _settings = LoadSettings();
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            try
            {
                if (env.IsDevelopment())
                {
                    app
                        .UseDeveloperExceptionPage();
                }

                app
                    .UseLykkeMiddleware("EthereumClassicApi", ex => new { Message = ex.ToString() });

                app
                    .UseMvc()
                    .UseSwagger(SetupSwagger)
                    .UseSwaggerUI(SetupSwaggerUI)
                    .UseStaticFiles();

                appLifetime
                    .ApplicationStarted.Register(() => StartApplication().GetAwaiter().GetResult());

                appLifetime
                    .ApplicationStopping.Register(() => StopApplication().GetAwaiter().GetResult());

                appLifetime
                    .ApplicationStopped.Register(() => CleanUp().GetAwaiter().GetResult());
            }
            catch (Exception e)
            {
                WriteFatalError(e, nameof(Configure));

                throw;
            }
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            try
            {
                _log = CreateLogWithSlack(services, _settings);

                services
                    .AddMvc(o =>
                    {
                        o.Filters.Add(new ExceptionFilterAttribute(typeof(BadRequestException), System.Net.HttpStatusCode.BadRequest));
                        o.Filters.Add(new ExceptionFilterAttribute(typeof(ConflictException), System.Net.HttpStatusCode.Conflict));
                        o.Filters.Add(new ExceptionFilterAttribute(typeof(ChaosException), System.Net.HttpStatusCode.InternalServerError));
                    })
                    .AddFluentValidation(fv =>
                    {
                        fv.RegisterValidatorsFromAssemblyContaining<Startup>();
                    });

                services
                    .AddSwaggerGen(SetupSwaggerGen);

                var builder = new ContainerBuilder();

                builder
                    .RegisterModule(new SettingsModule(_settings))
                    .RegisterModule(new LoggerModule(_log))
                    .RegisterModule<ActorsModule>()
                    .RegisterModule<BlockchainModule>()
                    .RegisterModule<RepositoriesModule>()
                    .RegisterModule<ServicesModule>();

                builder
                    .Register(ctx => ActorSystemFacadeFactory.Build(_container))
                    .As<IActorSystemFacade>()
                    .SingleInstance();

                builder
                    .Populate(services);

                _container = builder.Build();


                _actorSystemFacade = _container
                    .Resolve<IActorSystemFacade>();

                return new AutofacServiceProvider(_container);
            }
            catch (Exception e)
            {
                WriteFatalError(e, nameof(ConfigureServices));

                throw;
            }
        }

        private async Task CleanUp()
        {
            try
            {
                await _log.WriteMonitorAsync("", $"Env: {Program.EnvInfo}", "Terminating");

                _container.Dispose();
            }
            catch (Exception ex)
            {
                WriteFatalError(ex, nameof(CleanUp));

                throw;
            }
        }

        private IReloadingManager<AppSettings> LoadSettings()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(_environment.ContentRootPath)
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
            options.SwaggerDoc("v1", new Info
            {
                Title = "Ethereum Classic API",
                Version = "v1"
            });
        }

        private static void SetupSwaggerUI(SwaggerUIOptions options)
        {
            options.RoutePrefix = "swagger/ui";

            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        }

        private async Task StartApplication()
        {
            try
            {
                await _log.WriteMonitorAsync("", $"Env: {Program.EnvInfo}", "Started");
            }
            catch (Exception ex)
            {
                WriteFatalError(ex, nameof(StartApplication));

                throw;
            }
        }

        private async Task StopApplication()
        {
            try
            {
                if (_actorSystemFacade != null)
                {
                    await _actorSystemFacade.ShutdownAsync();
                }
            }
            catch (Exception ex)
            {
                WriteFatalError(ex, nameof(StopApplication));

                throw;
            }
        }

        private void WriteFatalError(Exception e, string process)
        {
            _log.WriteFatalErrorAsync
            (
                nameof(Startup),
                process,
                "",
                e
            ).GetAwaiter().GetResult();
        }

        private static ILog CreateLogWithSlack(IServiceCollection services, IReloadingManager<AppSettings> settings)
        {
            var consoleLogger = new LogToConsole();
            var aggregateLogger = new AggregateLogger();

            aggregateLogger.AddLog(consoleLogger);

            var dbLogConnectionStringManager = settings.Nested(x => x.EthereumClassicApi.Db.LogsConnectionString);
            var dbLogConnectionString = dbLogConnectionStringManager.CurrentValue;

            if (string.IsNullOrEmpty(dbLogConnectionString))
            {
                consoleLogger.WriteWarningAsync(nameof(Startup), nameof(CreateLogWithSlack), "Table loggger is not inited").Wait();
                return aggregateLogger;
            }

            if (dbLogConnectionString.StartsWith("${") && dbLogConnectionString.EndsWith("}"))
            {
                throw new InvalidOperationException($"LogsConnString {dbLogConnectionString} is not filled in settings");
            }

            var persistenceManager = new LykkeLogToAzureStoragePersistenceManager
            (
                AzureTableStorage<LogEntity>.Create(dbLogConnectionStringManager, "LykkeServiceLog", consoleLogger),
                consoleLogger
            );

            // Creating slack notification service, which logs own azure queue processing messages to aggregate log
            var slackService = services.UseSlackNotificationsSenderViaAzureQueue(new AzureQueueIntegration.AzureQueueSettings
            {
                ConnectionString = settings.CurrentValue.SlackNotifications.AzureQueue.ConnectionString,
                QueueName = settings.CurrentValue.SlackNotifications.AzureQueue.QueueName
            }, aggregateLogger);

            var slackNotificationsManager = new LykkeLogToAzureSlackNotificationsManager(slackService, consoleLogger);

            // Creating azure storage logger, which logs own messages to concole log
            var azureStorageLogger = new LykkeLogToAzureStorage(
                persistenceManager,
                slackNotificationsManager,
                consoleLogger);

            azureStorageLogger.Start();

            aggregateLogger.AddLog(azureStorageLogger);

            return aggregateLogger;
        }
    }
}
