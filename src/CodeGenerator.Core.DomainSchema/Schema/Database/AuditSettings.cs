using System.Text.Json.Serialization;

namespace CodeGenerator.Core.DomainSchema.Schema;

/// <summary>
/// Audit settings for automatic tracking
/// </summary>
public class AuditSettings
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("createdByColumn")]
    public string CreatedByColumn { get; set; } = "CreatedBy";

    [JsonPropertyName("createdAtColumn")]
    public string CreatedAtColumn { get; set; } = "CreatedAt";

    [JsonPropertyName("modifiedByColumn")]
    public string ModifiedByColumn { get; set; } = "ModifiedBy";

    [JsonPropertyName("modifiedAtColumn")]
    public string ModifiedAtColumn { get; set; } = "ModifiedAt";

    [JsonPropertyName("softDelete")]
    public bool SoftDelete { get; set; }

    [JsonPropertyName("deletedAtColumn")]
    public string DeletedAtColumn { get; set; } = "DeletedAt";

    [JsonPropertyName("deletedByColumn")]
    public string DeletedByColumn { get; set; } = "DeletedBy";

    [JsonPropertyName("isDeletedColumn")]
    public string IsDeletedColumn { get; set; } = "IsDeleted";
}
