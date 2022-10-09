using System.Collections.Generic;
using System.Text;

namespace ProviderConfiguration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.Sources.Clear();
                    IHostEnvironment env = hostingContext.HostingEnvironment;
                    
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddJsonFile($"appsettings.{env.EnvironmentName}.json");

                    config.AddIniFile("appsettings.ini", optional: false);
                    config.AddXmlFile("appsettings.xml");

                    // Add stream containing JSON
                    Stream json = new MemoryStream(Encoding.UTF8.GetBytes("{ \"DelayInMilliSeconds\": \"1337\"}")); ;
                    config.AddJsonStream(json);

                    // Alternative add a complete IConfiguration structure
                    var dictionary = new Dictionary<string, string> {
                        { "HostOption", "From in-memory HostConfiguration" }
                    };
                    ConfigurationBuilder builder = new ConfigurationBuilder();
                    builder.AddInMemoryCollection(dictionary);
                    config.AddConfiguration(builder.Build());

                    var con = config.Build();
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();
                })
                .Build();

            host.Run();
        }
    }
}