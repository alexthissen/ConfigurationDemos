using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OptionsFramework;
using System;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<PopularPasswordChecker>();

        //services.Configure<WorkerOptions>(hostContext.Configuration);
        services.AddOptions<WorkerOptions>()
            .Bind(hostContext.Configuration.GetRequiredSection(nameof(Worker)),
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

        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();

public class PopularPasswordChecker
{
    public bool IsPopular(string password) => String.Equals(password, "easy", StringComparison.CurrentCultureIgnoreCase);
}
