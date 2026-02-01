using CodeGenerator.Core.Artifacts;

namespace CodeGenerator.Core.Artifacts;

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
    /// Gets the name of the group associated with this instance.
    /// This is used to categorize commands in the UI, Context menus, etc.
    /// </summary>
    public string GroupName { get; init; }

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

    public ArtifactTreeNodeCommand(string groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
            throw new ArgumentNullException(nameof(groupName), "Group name cannot be null or whitespace.");
        
        GroupName = groupName;
    }

    /// <summary>
    /// Is this a separator
    /// </summary>
    //public bool IsSeparator { get; init; }

    /// <summary>
    /// Create a separator command
    /// </summary>
    //public static ArtifactTreeNodeCommand Separator => new() { IsSeparator = true };
}
