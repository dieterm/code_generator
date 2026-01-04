using System.Text.Json.Serialization;

namespace CodeGenerator.Core.Models.Schema;

/// <summary>
/// Display settings for UI generation
/// </summary>
public class DisplaySettings
{
    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; }

    [JsonPropertyName("tooltip")]
    public string? Tooltip { get; set; }

    [JsonPropertyName("controlType")]
    public string? ControlType { get; set; }

    [JsonPropertyName("order")]
    public int Order { get; set; }

    [JsonPropertyName("groupName")]
    public string? GroupName { get; set; }

    [JsonPropertyName("isVisible")]
    public bool IsVisible { get; set; } = true;

    [JsonPropertyName("isEditable")]
    public bool IsEditable { get; set; } = true;

    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonPropertyName("format")]
    public string? Format { get; set; }
}
