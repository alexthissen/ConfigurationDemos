using Microsoft.Extensions.Options;

namespace WorkerService
{
    internal record WorkerSettings(int DelayInMilliSeconds);

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IOptionsMonitor<SlicksOptions> options2;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IOptions<WorkerOptions> options,
            IOptionsMonitor<SlicksOptions> options2)
        {
            this.logger = logger;
            this.options2 = options2;
            Console.WriteLine(configuration["slicks:listenaddress"]);

            // Binding to non-public default constructor works in .NET 7
            var settings = configuration.GetSection(nameof(Worker)).Get<WorkerSettings>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                options2.OnChange((options, _) => {
                    logger.LogError("Change");
                });
                int delay = options2.CurrentValue.DelayInMilliSeconds;
                logger.LogInformation("# Worker running at: {time}, delay {delay}", DateTimeOffset.Now, delay);
                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}