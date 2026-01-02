namespace CodeGenerator.Core.Models.Output;

/// <summary>
/// Represents the output of a code generation operation
/// </summary>
public class GenerationResult
{
    /// <summary>
    /// Whether the generation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// List of generated files
    /// </summary>
    public List<GeneratedFile> Files { get; set; } = new();

    /// <summary>
    /// List of generated projects
    /// </summary>
    public List<GeneratedProject> Projects { get; set; } = new();

    /// <summary>
    /// Error messages if any
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Warning messages
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Informational messages
    /// </summary>
    public List<string> Messages { get; set; } = new();

    /// <summary>
    /// Duration of the generation
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Timestamp of generation
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}
