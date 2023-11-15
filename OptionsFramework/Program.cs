using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OptionsFramework;
using System;


// New .NET 8.0 startup for worker services and background services
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<PopularPasswordChecker>();

IConfigurationSection section = builder.Configuration.GetRequiredSection(nameof(Worker));

// Simple version
builder.Services.Configure<WorkerOptions>(section);

// Advanced version
builder.Services
    .AddOptions<WorkerOptions>()
    .Bind(section,
        binder =>
        {
            binder.ErrorOnUnknownConfiguration = true;
            binder.BindNonPublicProperties = true;
        })
    .ValidateDataAnnotations()
    .Validate<PopularPasswordChecker>(
        (options, checker) => !checker.IsPopular(options.AdminPassword),
        "Admin password is common.");
    //.ValidateOnStart();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();

public class PopularPasswordChecker
{
    public bool IsPopular(string password) => String.Equals(password, "easy", StringComparison.CurrentCultureIgnoreCase);
}
