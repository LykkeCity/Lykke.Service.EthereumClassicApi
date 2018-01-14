using System;
using System.IO;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.PlatformAbstractions;

namespace Lykke.Service.EthereumClassicApi
{
    internal sealed class Program
    {
        public static string EnvInfo 
            => Environment.GetEnvironmentVariable("ENV_INFO");

        public static async Task Main(string[] args)
        {
            Console.WriteLine($"{Constants.ApplicationName} version {PlatformServices.Default.Application.ApplicationVersion}");
            Console.WriteLine($"Is {(Constants.IsDebug ? "DEBUG" : "RELEASE")}");
            
            var environmentInfo = Environment.GetEnvironmentVariable("ENV_INFO");

            if (!string.IsNullOrEmpty(environmentInfo))
            {
                Console.WriteLine($"ENV_INFO: {environmentInfo}");
            }
            
            try
            {
                var host = new WebHostBuilder()
                    .UseKestrel()
                    .UseUrls("http://*:5000")
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseStartup<Startup>()
                    .UseApplicationInsights()
                    .Build();

                await host.RunAsync();
            }
            catch (Exception ex)
            {
                var delay = TimeSpan.FromMinutes(1);

                Console.WriteLine("Fatal error:");
                Console.WriteLine();
                Console.WriteLine(ex);
                Console.WriteLine();
                Console.WriteLine($"Process will be terminated in {delay}. Press any key to terminate immediately.");
                Console.WriteLine();

                await Task.WhenAny
                (
                    Task.Delay(delay),
                    Task.Run(() => { Console.ReadKey(true); })
                );
            }

            Console.WriteLine("Terminated");
        }
    }
}
