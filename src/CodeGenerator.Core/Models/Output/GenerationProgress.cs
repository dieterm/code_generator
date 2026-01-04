namespace CodeGenerator.Core.Models.Output;

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

/// <summary>
/// Phases of the code generation process
/// </summary>
public enum GenerationPhase
{
    /// <summary>
    /// Initializing and parsing schema
    /// </summary>
    Initializing,

    /// <summary>
    /// Creating solution structure
    /// </summary>
    CreatingSolution,

    /// <summary>
    /// Creating projects
    /// </summary>
    CreatingProjects,

    /// <summary>
    /// Adding NuGet packages
    /// </summary>
    AddingPackages,

    /// <summary>
    /// Adding project references
    /// </summary>
    AddingReferences,

    /// <summary>
    /// Generating files
    /// </summary>
    GeneratingFiles,

    /// <summary>
    /// Running generators
    /// </summary>
    RunningGenerators,

    /// <summary>
    /// Finalizing
    /// </summary>
    Finalizing,

    /// <summary>
    /// Completed
    /// </summary>
    Completed
}
