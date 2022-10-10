namespace AdvancedConfiguration;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> logger;
    private readonly WorkerSettings settings;

    public Worker(ILogger<Worker> logger, WorkerSettings settings)
    {
        this.logger = logger;
        this.settings = settings;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogWarning(settings.WelcomeText);
        logger.LogWarning("Delay of {delay}ms", settings.DelayInMilliSeconds);
        logger.LogWarning($"Admin password is '{settings.AdminPassword}'");

        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(settings.DelayInMilliSeconds, stoppingToken);
        }
    }
}