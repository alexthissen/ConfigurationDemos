using System;
using System.ComponentModel.DataAnnotations;

namespace HostStartup;

public class WorkerOptions
{
    public int DelayInMilliSeconds { get; set; } = 10_000;
    public string WelcomeText { get; init; } = "Hello, World!";
    internal string AdminPassword { get; init; } = String.Empty;
}
