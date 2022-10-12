using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BasicConfiguration
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IConfiguration configuration;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            // Working directly with IConfiguration
            this.configuration = configuration;
            this.logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            // Parse non-string values
            int delay = Int32.Parse(configuration["DelayInMilliSeconds"]);

            logger.LogWarning(configuration["WelcomeText"]);
            logger.LogWarning("Delay of {delay}ms", delay);
            logger.LogWarning($"Admin password is '{configuration["AdminPassword"]}'");

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Better way to extract a typed value
            int delay = configuration.GetValue<int>("DelayInMilliSeconds");

            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker says {welcome} running at: {time}",
                    configuration["WelcomeText"], DateTimeOffset.Now);
                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
