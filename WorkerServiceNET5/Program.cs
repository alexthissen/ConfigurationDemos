using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WorkerServiceNET5
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseEnvironment(Environments.Staging)
                .UseConsoleLifetime(options => { options.SuppressStatusMessages = false; })
                .ConfigureAppConfiguration((context, cfb) => {
                    
                })
                .ConfigureHostConfiguration(config =>
                {
                    var dictionary = new Dictionary<string, string> { 
                        { "HostOption", "From in-memory HostConfiguration" }
                    };
                    ConfigurationBuilder builder = new ConfigurationBuilder();
                    builder.AddInMemoryCollection(dictionary);
                    config.AddConfiguration(builder.Build());
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<HostOptions>(options =>
                    {
                        // In .NET 5.0 DOTNET_SHUTDOWNTIMEOUTSECONDS is not respected. Fixed in .NET 6.0
                        options.ShutdownTimeout = TimeSpan.FromSeconds(10);
                    });
                    services.AddHostedService<Worker>();

                    var settings = hostContext.Configuration.GetSection(nameof(Worker)).Get<WorkerSettings>();
                    services.AddSingleton<WorkerSettings>(settings);
                });
    }
}
