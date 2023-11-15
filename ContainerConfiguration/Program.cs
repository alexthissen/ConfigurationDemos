using ContainerConfiguration;

// New .NET 8.0 startup for worker services and background services
var builder = Host.CreateApplicationBuilder(args);

// Dynamically mounted file from Kubernetes secret
builder.Configuration.AddJsonFile("secrets/appsettings.secrets.json", optional: true);

builder.Services.AddHostedService<Worker>();
builder.Services.Configure<WorkerOptions>(builder.Configuration.GetSection(nameof(Worker)));

var host = builder.Build();
host.Run();

/* Old worker process startup
 
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, configuration) =>
    {
        // Dynamically mounted file from Kubernetes secret
        configuration.AddJsonFile("secrets/appsettings.secrets.json", optional: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<WorkerOptions>(context.Configuration.GetSection(nameof(Worker)));
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
*/