using Azure.Identity;
using AzureAppConfiguration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

IConfigurationRefresher? refresher = null;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        // Partial config build needed when using ConfigurationBuilder
        string connection = config.Build().GetConnectionString("AppConfig");

        if (!String.IsNullOrEmpty(connection))
        {
            config.AddAzureAppConfiguration(options =>
            {
                options.Connect(connection);

                // TODO: Make it environment aware
                options.Select(KeyFilter.Any);
                options.ConfigureRefresh(refresh =>
                {
                    refresh.Register("Sentinel", refreshAll: true)
                        .SetCacheExpiration(new TimeSpan(0, 1, 0));
                });
                refresher = options.GetRefresher();

                options.UseFeatureFlags(feature =>
                {
                    feature.CacheExpirationInterval = TimeSpan.FromSeconds(30);
                    feature.Label = context.HostingEnvironment.EnvironmentName;
                });
            }, false); // Important to set for missing App Configuration service
        }
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<WorkerSettings>(context.Configuration.GetSection(nameof(Worker)));
        services.AddHostedService<Worker>();
        if (refresher is not null) services.AddSingleton(refresher);

        // Required for refresh
        services.AddAzureAppConfiguration();
    })
    .Build();

host.Run();