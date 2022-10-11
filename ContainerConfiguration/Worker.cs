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
            logger.LogWarning(settings.Value.WelcomeText);
            logger.LogWarning("Delay of {delay}ms", settings.Value.DelayInMilliSeconds);
            logger.LogWarning($"Admin password is '{settings.Value.AdminPassword}'");

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