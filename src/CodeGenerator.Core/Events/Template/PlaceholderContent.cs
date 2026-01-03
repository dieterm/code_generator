namespace CodeGenerator.Core.Events;

/// <summary>
/// Content contribution for a placeholder
/// </summary>
public class PlaceholderContent
{
    public string Content { get; set; } = string.Empty;
    public int Priority { get; set; } = 100;
    public string ContributedBy { get; set; } = string.Empty;
}
