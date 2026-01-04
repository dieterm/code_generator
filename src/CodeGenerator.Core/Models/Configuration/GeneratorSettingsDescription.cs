using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Models.Configuration
{
    public class GeneratorSettingsDescription
    {
        /// <summary>
        /// Unique identifier for this generator
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Display name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Description of what this generator produces
        /// </summary>
        string Description { get; }

        public IEnumerable<TemplateRequirement> TemplateRequirements { get; }

        public GeneratorSettingsDescription(string id, string name, string description, IEnumerable<TemplateRequirement> templateRequirements)
        {
            Id = id;
            Name = name;
            Description = description;
            TemplateRequirements = templateRequirements;
        }
    }
}
