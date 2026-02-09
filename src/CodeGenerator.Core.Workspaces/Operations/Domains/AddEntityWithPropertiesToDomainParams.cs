using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Core.Workspaces.Operations.Domains
{
    public class AddEntityWithPropertiesToDomainParams
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

        [Required]
        [Description("Comma-separated list of property definitions in format 'Name:DataType:IsNullable' (e.g. 'Name:varchar:false,Population:int:false,Area:decimal:true'). Supported data types: varchar, int, bigint, decimal, float, bool, datetime, guid, text.")]
        public string PropertiesDefinition { get; set; } = string.Empty;
    }
}
