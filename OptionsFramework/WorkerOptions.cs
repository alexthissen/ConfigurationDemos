using System;
using System.ComponentModel.DataAnnotations;

namespace OptionsFramework;

public class WorkerOptions
{
    [Required, Range(500, 10000)]
    public int DelayInMilliSeconds { get; set; } = 10_000;

    [Required, MinLength(1), MaxLength(100)]
    public string WelcomeText { get; init; } = "Hello, World!";

    [Required]
    internal string AdminPassword { get; init; } = String.Empty;
}
