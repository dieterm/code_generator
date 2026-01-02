using System.Text.Json.Serialization;
using CodeGenerator.Core.Enums;

namespace CodeGenerator.Core.Models.Schema;

/// <summary>
/// Root-level database metadata
/// </summary>
public class DatabaseMetadata
{
    [JsonPropertyName("databaseName")]
    public string? DatabaseName { get; set; }

    [JsonPropertyName("schema")]
    public string Schema { get; set; } = "dbo";

    [JsonPropertyName("provider")]
    public string Provider { get; set; } = "SqlServer";

    [JsonPropertyName("connectionStringName")]
    public string? ConnectionStringName { get; set; }

    [JsonPropertyName("useMigrations")]
    public bool UseMigrations { get; set; } = true;

    [JsonPropertyName("seedData")]
    public bool SeedData { get; set; }

    [JsonPropertyName("auditSettings")]
    public AuditSettings? AuditSettings { get; set; }
}
