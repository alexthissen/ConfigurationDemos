using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SectionConfiguration
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IConfiguration configuration;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //int delay = configuration.GetSection(nameof(Worker)).GetValue<int>("DelayInMilliSeconds", 1000);
            
            // New required section in .NET 6
            IConfigurationSection section = configuration.GetRequiredSection(nameof(Worker));
            int delay = section.GetValue<int>("DelayInMilliSeconds", 1000);
            
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
