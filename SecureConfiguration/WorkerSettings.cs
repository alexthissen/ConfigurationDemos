namespace SecureConfiguration;

public record WorkerSettings
{
    public int DelayInMilliSeconds { get; init; } = 10_000;
    public string WelcomeText { get; init; } = "Hello, World!";
    public string AdminPassword { get; init; } = String.Empty;
}