using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Options;

namespace AzureAppConfiguration
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IOptionsMonitor<WorkerSettings> settings;
        private readonly IConfigurationRefresher refresher;

        public Worker(ILogger<Worker> logger, 
            IConfigurationRefresher refresher,
            IOptionsMonitor<WorkerSettings> settings)
        {
            this.logger = logger;
            this.settings = settings;
            this.refresher = refresher;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogWarning(settings.CurrentValue.WelcomeText);
            logger.LogWarning("Delay of {delay}ms", settings.CurrentValue.DelayInMilliSeconds);
            logger.LogWarning($"Admin password is '{settings.CurrentValue.AdminPassword}'");

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker says {Welcome}", settings.CurrentValue.WelcomeText);
                await Task.Delay(settings.CurrentValue.DelayInMilliSeconds, stoppingToken);

                // Use internal or external signal to refresh
                // In ASP.NET Core use app.UseAzureAppConfiguration() middleware
                await refresher.TryRefreshAsync(stoppingToken);
            }
        }
    }
}