using CodeGenerator.Core.Artifacts.CodeGeneration;
using CodeGenerator.Core.Workspaces.Artifacts;

namespace CodeGenerator.Core.Generators;

/// <summary>
/// Represents the output of a code generation operation
/// </summary>
public class GenerationResult
{
    public GenerationResult(RootArtifact rootArtifact, WorkspaceArtifact workspace)
    {
        this.RootArtifact = rootArtifact;
        this.Workspace = workspace;
    }
    /// <summary>
    /// Whether the generation was successful
    /// </summary>
    public bool Success { get; set; } = false;

    /// <summary>
    /// The Root artifact representing all generated artifacts
    /// </summary>
    public RootArtifact RootArtifact { get; }

    /// <summary>
    /// If domain schema context is provided, generated code is based on this schema
    /// </summary>
    //public DomainSchema.Schema.DomainSchema? DomainSchema { get; }
    public WorkspaceArtifact Workspace { get; }

    /// <summary>
    /// Error messages if any
    /// </summary>
    public List<string> Errors { get; } = new();

    /// <summary>
    /// Warning messages
    /// </summary>
    public List<string> Warnings { get; } = new();

    /// <summary>
    /// Informational messages
    /// </summary>
    public List<string> Messages { get; } = new();

    /// <summary>
    /// Duration of the generation
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Timestamp of generation
    /// </summary>
    public DateTime GeneratedAt { get; } = DateTime.UtcNow;
}
