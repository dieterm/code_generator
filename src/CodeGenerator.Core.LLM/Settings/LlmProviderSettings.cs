using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeGenerator.Core.LLM.Settings
{
    public class LlmProviderSettings
    {
        public LlmProviderSettings()
        {
            LlmProviderId = string.Empty;
        }

        public LlmProviderSettings(string llmProviderId)
        {
            LlmProviderId = llmProviderId;
        }

        /// <summary>
        /// Unique identifier for the LLM provider these settings belong to
        /// </summary>
        [JsonPropertyName("llmProviderId")]
        public string LlmProviderId { get; set; }

        /// <summary>
        /// Generator-specific parameters stored as key-value pairs
        /// </summary>
        [JsonPropertyName("parameters")]
        public Dictionary<string, object?> Parameters { get; set; } = new Dictionary<string, object?>();

        /// <summary>
        /// Get a parameter value by key
        /// </summary>
        public T? GetParameter<T>(string key, T? defaultValue = default)
        {
            if (Parameters.TryGetValue(key, out var value))
            {
                if (value is T typedValue)
                {
                    return typedValue;
                }

                // Try to convert from JsonElement if loaded from JSON
                if (value is System.Text.Json.JsonElement jsonElement)
                {
                    try
                    {
                        return System.Text.Json.JsonSerializer.Deserialize<T>(jsonElement.GetRawText());
                    }
                    catch
                    {
                        return defaultValue;
                    }
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Set a parameter value
        /// </summary>
        public void SetParameter<T>(string key, T? value)
        {
            Parameters[key] = value;
        }
    }
}
