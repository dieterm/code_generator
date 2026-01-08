using CodeGenerator.Core.Generators.Settings;

namespace CodeGenerator.Core.Settings.Generators
{
    /// <summary>
    /// Interface for objects that provide generator settings description.
    /// This interface should be implemented by generators to provide their settings.
    /// </summary>
    public interface IGeneratorSettingsProvider
    {
        /// <summary>
        /// Get the unique identifier for this generator
        /// </summary>
        string GeneratorId { get; }

        /// <summary>
        /// Get the display name for this generator
        /// </summary>
        string GeneratorName { get; }

        /// <summary>
        /// Get the description of this generator
        /// </summary>
        string GeneratorDescription { get; }

        /// <summary>
        /// Get the category for grouping (e.g., "Domain", "Infrastructure")
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Get the default settings for this generator
        /// </summary>
        GeneratorSettings GetDefaultSettings();

        List<ParameterDefinition> ParameterDefinitions { get; }
        Dictionary<string, object?> Parameters { get; }
        List<TemplateRequirement> Templates { get; }
        List<string> DependingGeneratorIds { get; }
    }
}
