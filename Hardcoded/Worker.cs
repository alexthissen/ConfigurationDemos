using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NoConfiguration
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private const int DELAY_IN_MILLISECONDS = 2000;
        private readonly string WelcomeText = "Hard coded welcome";
        private readonly string AdminPassword = "h4rd2Gu355";

        public Worker(ILogger<Worker> logger)
        {
            this.logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogWarning(WelcomeText);
            logger.LogWarning("Delay of {delay}ms", DELAY_IN_MILLISECONDS);
            logger.LogWarning($"Admin password is '{AdminPassword}'");

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(DELAY_IN_MILLISECONDS, stoppingToken);
            }
        }
    }
}
