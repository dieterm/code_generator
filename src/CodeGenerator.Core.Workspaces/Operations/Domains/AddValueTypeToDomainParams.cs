using CodeGenerator.Core.Workspaces.Artifacts.Domains.ValueTypes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Core.Workspaces.Operations.Domains
{
    public class AddValueTypeToDomainParams
    {
        [Required]
        [Description("The ID of the domain")]
        public string DomainId { get; set; } = string.Empty;

        [Required]
        [Description("The name for the new value type")]
        public string ValueTypeName { get; set; } = string.Empty;

        public ValueTypeArtifact? CreatedValueType;
        public ValueTypesContainerArtifact? ParentContainer;
    }
}
