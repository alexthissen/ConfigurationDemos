using HostStartup;
using System.Reflection;

IHost host = Host.CreateDefaultBuilder(args)
    .UseEnvironment(Environments.Production) // Safe default
    .UseConsoleLifetime(options => { options.SuppressStatusMessages = false; })
    .ConfigureHostConfiguration(config =>
    {
        var dictionary = new Dictionary<string, string> {
                        { "HostOption", "From in-memory HostConfiguration" }
        };
        config.AddInMemoryCollection(dictionary);
    })
    .ConfigureHostOptions(options =>
    {
        // New extension method in .NET 6
        options.ShutdownTimeout = TimeSpan.FromSeconds(10);
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder
//            .UseSetting(WebHostDefaults.ApplicationKey, "TestName")
            .CaptureStartupErrors(true)
            .PreferHostingUrls(false)
            .SuppressStatusMessages(false);

        webBuilder.UseShutdownTimeout(TimeSpan.FromSeconds(10));
        webBuilder.UseUrls("http://localhost:5010");
        webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
        webBuilder.UseEnvironment(Environments.Staging);
        webBuilder.UseWebRoot(Directory.GetCurrentDirectory());
        webBuilder.UseStaticWebAssets();
        webBuilder.UseSetting(WebHostDefaults.StartupAssemblyKey, Assembly.GetExecutingAssembly().GetName().Name);
        
        // Kestrel server can also be configured programmatically
        webBuilder.ConfigureKestrel(options =>
        {
            options.AllowSynchronousIO = true;
            options.Limits.MaxConcurrentConnections = 100;
            options.Limits.MaxConcurrentUpgradedConnections = 100;
            options.Limits.MaxRequestBodySize = 52428800;
        });
    })
    .ConfigureAppConfiguration((context, builder) =>
    {
        // Retrieve configuration from host
        var config = context.Configuration;
        context.HostingEnvironment.EnvironmentName = "Different";

        // Add additional providers for application configuration
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<HostOptions>(options =>
        {
            // In .NET 5.0 DOTNET_SHUTDOWNTIMEOUTSECONDS is not respected. Fixed in .NET 6.0
            options.ShutdownTimeout = TimeSpan.FromSeconds(10);
        });
        services.Configure<WorkerOptions>(context.Configuration.GetRequiredSection(nameof(Worker)));
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();