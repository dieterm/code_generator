using CodeGenerator.Core.Settings.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Templates.Settings
{
    public class AllTemplateEngineSettings : LocalApplicationDataSettingsBase
    {
        /// <summary>
        /// Dictionary of generator settings, keyed by generator ID
        /// </summary>
        [JsonPropertyName("templateEngines")]
        public Dictionary<string, TemplateEngineSettings> TemplateEngines { get; set; } = new Dictionary<string, TemplateEngineSettings>();

        /// <summary>
        /// Get settings for a specific generator
        /// </summary>
        public TemplateEngineSettings GetTemplateEngineSettings(string templateEngineId)
        {
            if (!TemplateEngines.TryGetValue(templateEngineId, out var settings))
            {
                settings = new TemplateEngineSettings(templateEngineId);
                TemplateEngines[templateEngineId] = settings;
            }
            return settings;
        }

        /// <summary>
        /// Set settings for a specific generator
        /// </summary>
        public void SetTemplateEngineSettings(TemplateEngineSettings settings)
        {
            TemplateEngines[settings.TemplateEngineId] = settings;
            LastModified = DateTime.UtcNow;
        }

        /// <summary>
        /// Check if settings exist for a generator
        /// </summary>
        public bool HasTemplateEngineSettings(string templateEngineId)
        {
            return TemplateEngines.ContainsKey(templateEngineId);
        }

        /// <summary>
        /// Remove settings for a template engine
        /// </summary>
        public bool RemoveTemplateEngineSettings(string templateEngineId)
        {
            return TemplateEngines.Remove(templateEngineId);
        }
    }
}
