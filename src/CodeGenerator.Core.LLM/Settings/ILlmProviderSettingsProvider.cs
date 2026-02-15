using CodeGenerator.Core.Settings;
using CodeGenerator.Core.Settings.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.LLM.Settings
{
    public interface ILlmProviderSettingsProvider
    {
        /// <summary>
        /// Get the default settings for this generator
        /// </summary>
        LlmProviderSettings GetDefaultSettings();

        List<ParameterDefinition> ParameterDefinitions { get; }
        Dictionary<string, object?> Parameters { get; }
    }
}
