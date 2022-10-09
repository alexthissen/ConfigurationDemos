using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

public class WorkerOptions
{
    [Required]
    [Range(500, 10000)]
    public int DelayInMilliSeconds { get; set; }
}

public class SlicksOptions
{
    public int DelayInMilliSeconds { get; set; }
}

public class SlicksOptionsConfigurator : IConfigureOptions<SlicksOptions>, 
    IValidateOptions<SlicksOptions>,
    IPostConfigureOptions<SlicksOptions>
{
    private readonly IConfiguration config;
    private readonly IServiceProvider provider;

    public SlicksOptionsConfigurator(IConfiguration config, IServiceProvider provider)
    {
        this.config = config;
        this.provider = provider;
    }

    public void Configure(SlicksOptions options)
    {
        var delay = Int32.Parse(config["Worker:DelayInMilliSeconds"]);
        options.DelayInMilliSeconds = delay + RandomNumberGenerator.GetInt32(1, 100);
    }

    public void PostConfigure(string? name, SlicksOptions options)
    {
       
    }

    public ValidateOptionsResult Validate(string? name, SlicksOptions options)
    {
        return ValidateOptionsResult.Success;
    }
}