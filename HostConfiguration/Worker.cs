namespace HostConfiguration
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IHostEnvironment env;

        public Worker(ILogger<Worker> logger, IHostEnvironment env)
        {
            this.logger = logger;
            this.env = env;
        }
        
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"Running in {env.EnvironmentName}");
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