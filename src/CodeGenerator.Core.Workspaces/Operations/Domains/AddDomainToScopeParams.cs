using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Core.Workspaces.Operations.Domains
{
    public class AddDomainToScopeParams
    {
        [Required]
        [Description("The name of the scope (e.g. 'Shared' or 'Application')")]
        public string ScopeName { get; set; } = string.Empty;

        [Required]
        [Description("The name for the new domain")]
        public string DomainName { get; set; } = string.Empty;
    }
}
