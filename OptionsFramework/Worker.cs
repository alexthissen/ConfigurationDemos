using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OptionsFramework
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly WorkerOptions options;
        
        public Worker(ILogger<Worker> logger, 
            IOptions<WorkerOptions> options)
        {
            this.logger = logger;
            try
            {
                this.options = options.Value;
            }
            catch (OptionsValidationException ex)
            {
                foreach (string failure in ex.Failures)
                {
                    logger.LogWarning(failure);
                }
                // Safe defaults
                this.options = new();
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogWarning(options.WelcomeText);
            logger.LogWarning("Delay of {delay}ms", options.DelayInMilliSeconds);
            logger.LogWarning($"Admin password is '{options.AdminPassword}'");

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(options.DelayInMilliSeconds, stoppingToken);
            }
        }
    }
}
