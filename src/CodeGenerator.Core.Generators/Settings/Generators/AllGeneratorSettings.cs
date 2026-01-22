using System.Text.Json.Serialization;

namespace CodeGenerator.Core.Settings.Generators
{
    /// <summary>
    /// Container for all generator settings.
    /// This is the root object that gets serialized to JSON.
    /// </summary>
    public class AllGeneratorSettings
    {
        public AllGeneratorSettings()
        {
            Generators = new Dictionary<string, GeneratorSettings>();
        }

        /// <summary>
        /// Version of the settings format for migration purposes
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// Last modified timestamp
        /// </summary>
        [JsonPropertyName("lastModified")]
        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Dictionary of generator settings, keyed by generator ID
        /// </summary>
        [JsonPropertyName("generators")]
        public Dictionary<string, GeneratorSettings> Generators { get; set; }

        /// <summary>
        /// Get settings for a specific generator
        /// </summary>
        public GeneratorSettings GetGeneratorSettings(string generatorId)
        {
            if (!Generators.TryGetValue(generatorId, out var settings))
            {
                settings = new GeneratorSettings(generatorId);
                Generators[generatorId] = settings;
            }
            return settings;
        }

        /// <summary>
        /// Set settings for a specific generator
        /// </summary>
        public void SetGeneratorSettings(GeneratorSettings settings)
        {
            Generators[settings.GeneratorId] = settings;
            LastModified = DateTime.UtcNow;
        }

        /// <summary>
        /// Check if settings exist for a generator
        /// </summary>
        public bool HasGeneratorSettings(string generatorId)
        {
            return Generators.ContainsKey(generatorId);
        }

        /// <summary>
        /// Remove settings for a generator
        /// </summary>
        public bool RemoveGeneratorSettings(string generatorId)
        {
            return Generators.Remove(generatorId);
        }
    }
}
