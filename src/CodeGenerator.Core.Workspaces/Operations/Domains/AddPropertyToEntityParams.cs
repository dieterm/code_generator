using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Core.Workspaces.Operations.Domains
{
    public class AddPropertyToEntityParams
    {
        [Required]
        [Description("The ID of the entity")]
        public string EntityId { get; set; } = string.Empty;

        [Required]
        [Description("The property name")]
        public string PropertyName { get; set; } = string.Empty;

        [Required]
        [Description("The data type (e.g. varchar, int, decimal, bool, datetime, guid, text)")]
        public string DataType { get; set; } = "varchar";

        [Description("Whether the property is nullable")]
        public bool IsNullable { get; set; }
        
        public EntityArtifact? Entity { get; internal set; }
        public PropertyArtifact? CreatedProperty;
    }
}
