namespace AzureAppConfiguration
{
    public record WorkerSettings
    {
        public int DelayInMilliSeconds { get; init; }
        public string WelcomeText { get; set; }
    }
}
