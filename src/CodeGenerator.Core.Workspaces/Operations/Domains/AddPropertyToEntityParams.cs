using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Core.Workspaces.Operations.Domains
{
    public class AddPropertyToEntityParams
    {
        [Required]
        [Description("The name of the scope")]
        public string ScopeName { get; set; } = string.Empty;

        [Required]
        [Description("The name of the domain")]
        public string DomainName { get; set; } = string.Empty;

        [Required]
        [Description("The name of the entity")]
        public string EntityName { get; set; } = string.Empty;

        [Required]
        [Description("The property name")]
        public string PropertyName { get; set; } = string.Empty;

        [Required]
        [Description("The data type (e.g. varchar, int, decimal, bool, datetime, guid, text)")]
        public string DataType { get; set; } = "varchar";

        [Description("Whether the property is nullable")]
        public bool IsNullable { get; set; }
    }
}
