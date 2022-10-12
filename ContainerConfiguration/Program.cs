using ContainerConfiguration;

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