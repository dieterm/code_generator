using CodeGenerator.Core.LLM.Providers;
using CodeGenerator.Core.Settings.Generators;
using CodeGenerator.Core.Settings.Interfaces;
using CodeGenerator.Core.Settings.Models;
using CodeGenerator.Core.Workspaces.Settings;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeGenerator.Core.LLM.Settings
{
    public class LlmSettingsManager : LocalApplicationDataSettingsManager<AllLlmProvidersSettings>
    {
        public LlmSettingsManager(ILogger<LlmSettingsManager> logger)
            : base(GetDefaultSettingsFilePath("CodeGenerator", "llmProviders-settings.json"), logger)
        {
        }

        public LlmSettingsManager(string settingsFilePath, ILogger<LlmSettingsManager> logger) 
            : base(settingsFilePath, logger)
        {
        }
        /// <summary>
        /// Get a parameter value for a generator
        /// </summary>
        public T? GetParameter<T>(string generatorId, string parameterName, T? defaultValue = default)
        {
            return Settings.GetLlmProviderSettings(generatorId).GetParameter(parameterName, defaultValue);
        }

        /// <summary>
        /// Set a parameter value for a generator
        /// </summary>
        public void SetParameter<T>(string generatorId, string parameterName, T? value)
        {
            Settings.GetLlmProviderSettings(generatorId).SetParameter(parameterName, value);
        }
        private Dictionary<IFieldViewModel, PropertyChangedEventHandler> _fieldViewModelEventHandlers = new();
        public override SettingSection GetSettingsViewModelSection()
        {
            // remove existing event handlers to avoid memory leaks and duplicate handlers
            foreach(var kvp in _fieldViewModelEventHandlers)
            {
                kvp.Key.PropertyChanged -= kvp.Value;
            }

            // generate settings section for LLM settings
            var section = new SettingSection("llm", "LLM Settings");

            // foreach LLM Provider add a settings item
            ServiceProviderHolder.GetServices<ILlmProvider>().ToList().ForEach(provider =>
            {
                var llmProviderSection = new SettingSection(provider.ProviderId, provider.DisplayName);
                section.Sections.Add(llmProviderSection);
                PropertyChangedEventHandler fieldViewModelPropertyChangedValueSetter = (sender, e) =>
                {
                    if (e.PropertyName == nameof(FieldViewModelBase.Value))
                    {
                        if (sender is FieldViewModelBase fieldViewModel)
                        {
                            var value = fieldViewModel.Value;
                            SetParameter<object>(provider.ProviderId, fieldViewModel.Name, value);
                        }
                    }
                };
                foreach (var settingsItem in provider.GetSettingsItems(Settings.GetLlmProviderSettings(provider.ProviderId)))
                {
                    llmProviderSection.Items.Add(settingsItem);
                    // add callback to update settings when the value changes
                    settingsItem.FieldViewModel.PropertyChanged += fieldViewModelPropertyChangedValueSetter;
                    _fieldViewModelEventHandlers.Add(settingsItem.FieldViewModel, fieldViewModelPropertyChangedValueSetter);
                }
                
            });
            return section;
        }

        public override AllLlmProvidersSettings CreateDefaultSettings()
        {
            var allItems = new AllLlmProvidersSettings();
            ServiceProviderHolder.GetServices<ILlmProvider>().ToList().ForEach(provider =>
            {
                var providerSettings = new LlmProviderSettings(provider.ProviderId);
                foreach(var settingsItem in provider.GetSettingsItems(providerSettings))
                {
                    providerSettings.SetParameter(settingsItem.Key, settingsItem.Value);
                }

                allItems.LlmProviders.Add(provider.ProviderId, providerSettings);
            });
            return allItems;
        }
    }
}
