using Microsoft.Extensions.Configuration;
using System.Net.Sockets;
using WorkerService;

//IHost host = Host.CreateDefaultBuilder(args)
IHost host = new HostBuilder()
    // Passed to ConfigureAppConfiguration and ConfigureHostConfiguration
    .ConfigureDefaults(args)
    .ConfigureWebHostDefaults(webBuilder => 
    {
        webBuilder
            .UseSetting(WebHostDefaults.ApplicationKey, "TestName")
            .CaptureStartupErrors(true)
            .PreferHostingUrls(false)
            .SuppressStatusMessages(false);
        webBuilder.UseShutdownTimeout(TimeSpan.FromSeconds(10));
        webBuilder.UseUrls("http://localhost:5010");
        webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
        webBuilder.UseEnvironment(Environments.Staging);
        webBuilder.UseWebRoot(Directory.GetCurrentDirectory());
        webBuilder.UseStaticWebAssets();
    })
    .ConfigureServices((builder, services) =>
    {
        services.Configure<WorkerOptions>(builder.Configuration.GetSection(nameof(Worker)));

        services.AddHostedService<Worker>();
        services.ConfigureOptions<SlicksOptionsConfigurator>();
        //services.AddOptions<WorkerOptions>().ValidateOnStart();
    })
    .Build();

host.Run();
