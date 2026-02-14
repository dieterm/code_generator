using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Core.Workspaces.Operations.Domains
{
    public class AddSubDomainParams
    {
        [Required]
        [Description("The ID of the parent domain")]
        public string ParentDomainId { get; set; } = string.Empty;

        [Required]
        [Description("The name for the new sub-domain")]
        public string NewDomainName { get; set; } = string.Empty;

        public DomainArtifact? ParentDomain;
        public DomainArtifact? CreatedDomain;
    }
}
