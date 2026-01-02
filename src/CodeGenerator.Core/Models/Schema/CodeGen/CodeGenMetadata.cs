using System.Text.Json.Serialization;
using CodeGenerator.Core.Enums;

namespace CodeGenerator.Core.Models.Schema;

/// <summary>
/// Root-level code generation metadata
/// </summary>
public class CodeGenMetadata
{
    [JsonPropertyName("namespace")]
    public string? Namespace { get; set; }

    [JsonPropertyName("outputPath")]
    public string? OutputPath { get; set; }

    [JsonPropertyName("targetLanguage")]
    public string TargetLanguage { get; set; } = "CSharp";

    [JsonPropertyName("presentationTechnology")]
    public string? PresentationTechnology { get; set; }

    [JsonPropertyName("dataLayerTechnology")]
    public string DataLayerTechnology { get; set; } = "EntityFrameworkCore";

    [JsonPropertyName("generateLayers")]
    public List<string>? GenerateLayers { get; set; }

    [JsonPropertyName("useDependencyInjection")]
    public bool UseDependencyInjection { get; set; } = true;

    [JsonPropertyName("useLogging")]
    public bool UseLogging { get; set; } = true;

    [JsonPropertyName("useConfiguration")]
    public bool UseConfiguration { get; set; } = true;

    [JsonPropertyName("projectSettings")]
    public ProjectSettings? ProjectSettings { get; set; }
}
