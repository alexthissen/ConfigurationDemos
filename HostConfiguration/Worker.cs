using Microsoft.Extensions.Options;

namespace HostStartup
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IHostEnvironment env;
        private readonly WorkerOptions options;

        public Worker(ILogger<Worker> logger, IHostEnvironment env, IOptions<WorkerOptions> options)
        {
            this.logger = logger;
            this.env = env;
            this.options = options.Value;
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
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}