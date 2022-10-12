using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace SecureConfiguration
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IOptions<WorkerOptions> options;
        private readonly IConfiguration configuration;

        public Worker(ILogger<Worker> logger, IOptions<WorkerOptions> options, IConfiguration configuration)
        {
            this.logger = logger;
            this.options = options;

            // For demos purposes only
            this.configuration = configuration;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Running with delay of {delay}ms", options.Value.DelayInMilliSeconds);
            logger.LogInformation(((IConfigurationRoot)configuration)
                .GetDebugView(context => context switch 
                {
                    { ConfigurationProvider: AzureKeyVaultConfigurationProvider } => "***",
                    { ConfigurationProvider: PhysicalFileProvider } => "***",
                    _ => context.Value ?? String.Empty
                }));

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}