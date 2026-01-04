namespace CodeGenerator.Core.Generators;

/// <summary>
/// Represents the progress of a code generation operation
/// </summary>
public class GenerationProgress
{
    /// <summary>
    /// Current step in the generation process
    /// </summary>
    public string CurrentStep { get; set; } = string.Empty;

    /// <summary>
    /// Detailed message about what is currently happening
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Current progress percentage (0-100)
    /// </summary>
    public int PercentComplete { get; set; }

    /// <summary>
    /// Total number of items to process
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Number of items completed
    /// </summary>
    public int CompletedItems { get; set; }

    /// <summary>
    /// Current phase of generation
    /// </summary>
    public GenerationPhase Phase { get; set; }

    /// <summary>
    /// Whether this is an indeterminate progress (unknown total)
    /// </summary>
    public bool IsIndeterminate { get; set; }
}
