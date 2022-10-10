using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;

namespace OptionsFramework;

public class WorkerOptionsConfigurator : IConfigureOptions<WorkerOptions>, 
    IValidateOptions<WorkerOptions>,
    IPostConfigureOptions<WorkerOptions>
{
    private readonly IConfiguration config;
    private readonly IServiceProvider provider;

    public WorkerOptionsConfigurator(IConfiguration config, IServiceProvider provider)
    {
        this.config = config;
        this.provider = provider;
    }

    public void Configure(WorkerOptions options)
    {
        var delay = Int32.Parse(config["Worker:DelayInMilliSeconds"]);
        options.DelayInMilliSeconds = delay + RandomNumberGenerator.GetInt32(1, 100);
    }

    public void PostConfigure(string? name, WorkerOptions options)
    {
       
    }

    public ValidateOptionsResult Validate(string? name, WorkerOptions options)
    {
        return ValidateOptionsResult.Success;
    }
}