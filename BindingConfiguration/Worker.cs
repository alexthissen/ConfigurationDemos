using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BindingConfiguration
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
            // Binding
            WorkerSettings workerSettings = new();
            configuration.Bind(configuration.GetSection(nameof(Worker)).Path, workerSettings);

            // Get
            workerSettings = configuration.GetSection(nameof(Worker)).Get<WorkerSettings>(options =>
            {
                options.BindNonPublicProperties = true;
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
