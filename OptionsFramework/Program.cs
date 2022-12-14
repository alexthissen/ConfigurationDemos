using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OptionsFramework;
using System;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<PopularPasswordChecker>();

        IConfigurationSection section = hostContext.Configuration.GetRequiredSection(nameof(Worker));

        // Simple version
        services.Configure<WorkerOptions>(section);

        // Advanced version
        services.AddOptions<WorkerOptions>()
            .Bind(section,
                binder =>
                {
                    binder.ErrorOnUnknownConfiguration = true;
                    binder.BindNonPublicProperties = true;
                })
            .ValidateDataAnnotations()
            .Validate<PopularPasswordChecker>(
                (options, checker) => !checker.IsPopular(options.AdminPassword),
                "Admin password is common.")
            .ValidateOnStart();

        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();

public class PopularPasswordChecker
{
    public bool IsPopular(string password) => String.Equals(password, "easy", StringComparison.CurrentCultureIgnoreCase);
}
