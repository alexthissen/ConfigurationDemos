using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NoConfiguration
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private const int DELAY_IN_MILLISECONDS = 2000;

        public Worker(ILogger<Worker> logger)
        {
            this.logger = logger;
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
