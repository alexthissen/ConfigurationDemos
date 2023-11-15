using AdvancedConfiguration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;
using System.Text;

IHost host = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.Sources.Clear();
        IHostEnvironment env = hostingContext.HostingEnvironment;

        config.AddJsonFile("appsettings.json", optional: true);
        config.AddJsonFile($"appsettings.{env.EnvironmentName}.json");

        config.AddIniFile("appsettings.ini", optional: false);
        config.AddXmlFile("appsettings.xml", optional: true, reloadOnChange: true);

        // Add stream containing JSON
        Stream json = new MemoryStream(Encoding.UTF8.GetBytes("{ \"DelayInMilliSeconds\": \"1337\"}"));
        config.AddJsonStream(json);

        // Alternative add a complete IConfiguration structure
        var dictionary = new Dictionary<string, string?> {
            { "HostOption", "From in-memory HostConfiguration" }
        };
        ConfigurationBuilder builder = new ConfigurationBuilder();
        builder.AddInMemoryCollection(dictionary);
        config.AddConfiguration(builder.Build());

        var con = config.Build();
    })
    .ConfigureServices((context, services) =>
    {
        IConfigurationSection section = context.Configuration.GetRequiredSection(nameof(Worker));

        // Binding requires existing object
        WorkerSettings workerSettings = new();
        section.Bind(workerSettings);

        // Get with binding to non-public props
        workerSettings = section.Get<WorkerSettings>(options =>
        {
            options.BindNonPublicProperties = true;
        }) ?? new();

        // Settings shouldn't change and can be singleton
        services.AddSingleton(workerSettings!);
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();