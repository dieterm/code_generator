using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Core.Workspaces.Operations.Scopes
{
    /// <summary>
    /// Parameters for adding a scope to the workspace.
    /// </summary>
    public class AddScopeToWorkspaceParams
    {
        [Required]
        [Description("The name of the new scope (e.g. 'Infrastructure', 'Presentation')")]
        public string ScopeName { get; set; } = string.Empty;
    }
}
