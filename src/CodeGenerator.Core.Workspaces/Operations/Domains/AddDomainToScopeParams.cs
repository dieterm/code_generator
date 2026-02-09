using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Core.Workspaces.Operations.Domains
{
    public class AddDomainToScopeParams
    {
        [Required]
        [Description("The ID of the scope")]
        public string ScopeId { get; set; } = string.Empty;

        [Required]
        [Description("The name for the new domain")]
        public string DomainName { get; set; } = string.Empty;

        public DomainArtifact? CreatedDomain;
        public OnionDomainLayerArtifact? ParentContainer;

    }
}
