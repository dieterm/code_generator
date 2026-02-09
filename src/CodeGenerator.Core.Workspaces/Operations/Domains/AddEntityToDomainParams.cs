using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Core.Workspaces.Operations.Domains
{
    public class AddEntityToDomainParams
    {
        [Required]
        [Description("The name of the scope")]
        public string ScopeName { get; set; } = string.Empty;

        [Required]
        [Description("The name of the domain")]
        public string DomainName { get; set; } = string.Empty;

        [Required]
        [Description("The name for the new entity")]
        public string EntityName { get; set; } = string.Empty;
    }
}
