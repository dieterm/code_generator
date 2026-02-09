using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Core.Workspaces.Operations.Domains
{
    public class AddValueTypeToDomainParams
    {
        [Required]
        [Description("The name of the scope")]
        public string ScopeName { get; set; } = string.Empty;

        [Required]
        [Description("The name of the domain")]
        public string DomainName { get; set; } = string.Empty;

        [Required]
        [Description("The name for the new value type")]
        public string ValueTypeName { get; set; } = string.Empty;
    }
}
