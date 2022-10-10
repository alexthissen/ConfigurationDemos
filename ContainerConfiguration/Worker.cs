using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ContainerConfiguration
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IOptions<WorkerSettings> settings;
        private readonly IConfiguration configuration;

        public Worker(ILogger<Worker> logger, IOptions<WorkerSettings> settings,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.settings = settings;
            this.configuration = configuration;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Running with delay of {delay}ms", settings.Value.DelayInMilliSeconds);
            logger.LogInformation(((IConfigurationRoot)configuration).GetDebugView());
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(settings.Value.DelayInMilliSeconds, stoppingToken);
            }
        }
    }
}