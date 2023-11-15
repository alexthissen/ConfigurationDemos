using Azure.Identity;
using AzureAppConfiguration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;

IConfigurationRefresher? refresher = null;

var builder = Host.CreateApplicationBuilder(args);

// Partial config build no longer needed when using ConfigurationManager
//string connection = config.Build().GetConnectionString("AppConfig");

string? connection = builder.Configuration.GetConnectionString("AppConfig");

if (!String.IsNullOrEmpty(connection))
{
    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(connection);

        // Config is environment aware
        options.Select(KeyFilter.Any, builder.Environment.EnvironmentName);
        options.ConfigureRefresh(refresh =>
        {
            refresh.Register("Sentinel", refreshAll: true)
                .SetCacheExpiration(new TimeSpan(0, 1, 0));
        });
        refresher = options.GetRefresher();

        options.UseFeatureFlags(feature =>
        {
            feature.CacheExpirationInterval = TimeSpan.FromSeconds(30);
            feature.Label = builder.Environment.EnvironmentName;
        });
    }, false); // Important to set for missing App Configuration service
}

IConfigurationSection section = builder.Configuration.GetSection(nameof(Worker));
        
ChangeToken.OnChange(
    () => section.GetReloadToken(),
    state => { Debug.WriteLine("Config has changed"); },
    builder.Environment
);

builder.Services.Configure<WorkerOptions>(section);
builder.Services.AddHostedService<Worker>();
if (refresher is not null) builder.Services.AddSingleton(refresher);

// Required for refresh
builder.Services.AddAzureAppConfiguration();

var host = builder.Build();
host.Run();