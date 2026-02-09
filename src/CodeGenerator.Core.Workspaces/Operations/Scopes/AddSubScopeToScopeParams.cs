using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Core.Workspaces.Operations.Scopes
{
    public class AddSubScopeToScopeParams
    {
        [Required]
        [Description("The name of the parent scope")]
        public string ParentScopeName { get; set; } = string.Empty;

        [Required]
        [Description("The name of the new sub-scope")]
        public string NewScopeName { get; set; } = string.Empty;
    }
}
