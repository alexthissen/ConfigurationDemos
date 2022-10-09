using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerServiceNET5
{
    internal record WorkerSettings
    {
        public int DelayInMilliSeconds { get; init; }
    }

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IConfiguration configuration;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, 
            IOptions<HostOptions> hostOptions)
        {
            this.logger = logger;
            this.configuration = configuration;
            
            WorkerSettings bindOptions = new ();
            configuration.GetSection(nameof(Worker)).Bind(bindOptions);

            var getOptions = configuration.GetSection(nameof(Worker)).Get<WorkerSettings>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //var delay = Int32.Parse(configuration["DelayInMilliSeconds"]);
            var delay = configuration.GetValue<int>("DelayInMilliSeconds", 1000);
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
