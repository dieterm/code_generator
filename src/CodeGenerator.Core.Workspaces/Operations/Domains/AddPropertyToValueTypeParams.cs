using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.ValueTypes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Core.Workspaces.Operations.Domains
{
    public class AddPropertyToValueTypeParams
    {
        [Required]
        [Description("The ID of the value type")]
        public string ValueTypeId { get; set; } = string.Empty;

        [Required]
        [Description("The property name")]
        public string PropertyName { get; set; } = string.Empty;

        [Required]
        [Description("The data type (e.g. varchar, int, decimal, bool, datetime, guid, text)")]
        public string DataType { get; set; } = "varchar";

        [Description("Whether the property is nullable")]
        public bool IsNullable { get; set; }

        public PropertyArtifact? CreatedProperty;
        public ValueTypeArtifact? ParentValueType;
    }
}
