using CodeGenerator.Core.Artifacts;

namespace CodeGenerator.Application.Controllers.Base;

/// <summary>
/// Command definition for artifact tree node context menus
/// </summary>
public class ArtifactTreeNodeCommand
{
    /// <summary>
    /// Unique identifier for the command
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Display text for the command
    /// </summary>
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Icon key for the command
    /// </summary>
    public string? IconKey { get; init; }

    /// <summary>
    /// Execute the command
    /// </summary>
    public Func<IArtifact, Task>? Execute { get; init; }

    /// <summary>
    /// Check if the command can execute
    /// </summary>
    public Func<IArtifact, bool>? CanExecute { get; init; }

    /// <summary>
    /// Sub-commands (for hierarchical menus)
    /// </summary>
    public List<ArtifactTreeNodeCommand>? SubCommands { get; init; }

    /// <summary>
    /// Is this a separator
    /// </summary>
    public bool IsSeparator { get; init; }

    /// <summary>
    /// Create a separator command
    /// </summary>
    public static ArtifactTreeNodeCommand Separator => new() { IsSeparator = true };
}
