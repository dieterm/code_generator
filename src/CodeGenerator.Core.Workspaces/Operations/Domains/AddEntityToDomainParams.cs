using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Core.Workspaces.Operations.Domains
{
    public class AddEntityToDomainParams
    {
        [Required]
        [Description("The ID of the domain's entities container")]
        public string EntitiesContainerId { get; set; } = string.Empty;
        [Required]
        [Description("The name for the new entity")]
        public string EntityName { get; set; } = string.Empty;

        public EntityArtifact? CreatedEntity;
        public DomainArtifact? ParentDomain;
    }
}
