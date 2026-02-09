using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Core.Workspaces.Operations.Scopes
{
    public class AddSubScopeToScopeParams
    {
        [Required]
        [Description("The ID of the parent scope")]
        public string ParentScopeId { get; set; } = string.Empty;

        [Required]
        [Description("The name of the new sub-scope")]
        public string NewScopeName { get; set; } = string.Empty;

        public ScopeArtifact? CreatedScope;
        public SubScopesContainerArtifact? ParentContainer;

    }
}
