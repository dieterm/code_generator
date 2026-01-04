using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Generators.Settings
{
    public class GeneratorSettingsDescription
    {
        /// <summary>
        /// Unique identifier for this generator
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Display name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Description of what this generator produces
        /// </summary>
        public string Description { get; }

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
