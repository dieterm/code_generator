using System.Text.Json.Serialization;

namespace CodeGenerator.Core.DomainSchema.Schema;

/// <summary>
/// Project-level settings for code generation
/// </summary>
public class ProjectSettings
{
    [JsonPropertyName("solutionName")]
    public string? SolutionName { get; set; }

    [JsonPropertyName("domainProjectName")]
    public string? DomainProjectName { get; set; }

    [JsonPropertyName("applicationProjectName")]
    public string? ApplicationProjectName { get; set; }

    [JsonPropertyName("infrastructureProjectName")]
    public string? InfrastructureProjectName { get; set; }

    [JsonPropertyName("presentationProjectName")]
    public string? PresentationProjectName { get; set; }

    [JsonPropertyName("testsProjectName")]
    public string? TestsProjectName { get; set; }

    [JsonPropertyName("sharedProjectName")]
    public string? SharedProjectName { get; set; }
}
