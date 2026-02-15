using CodeGenerator.Core.Settings.Generators;
using CodeGenerator.Core.Settings.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeGenerator.Core.LLM.Settings
{
    public class AllLlmProvidersSettings : LocalApplicationDataSettingsBase
    {
        [JsonPropertyName("llmProviders")]
        public Dictionary<string, LlmProviderSettings> LlmProviders { get; set; } = new Dictionary<string, LlmProviderSettings>();

        /// <summary>
        /// Get settings for a specific LLM provider
        /// </summary>
        public LlmProviderSettings GetLlmProviderSettings(string llmProviderId)
        {
            if (!LlmProviders.TryGetValue(llmProviderId, out var settings))
            {
                settings = new LlmProviderSettings(llmProviderId);
                LlmProviders[llmProviderId] = settings;
            }
            return settings;
        }
    }
}
