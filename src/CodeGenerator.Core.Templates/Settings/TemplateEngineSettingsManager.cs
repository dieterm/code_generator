using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Settings;
using CodeGenerator.Core.Settings.Interfaces;
using CodeGenerator.Core.Settings.Models;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Models;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Templates.Settings
{
    public class TemplateEngineSettingsManager : LocalApplicationDataSettingsManager<AllTemplateEngineSettings>
    {
        private readonly List<TemplateEngineSettingsDescription> _templateEngineSettingsDescriptions = new List<TemplateEngineSettingsDescription>();

        public TemplateEngineSettingsManager(ILogger<TemplateEngineSettingsManager> logger)
            : base(GetDefaultSettingsFilePath("CodeGenerator", "template-engine-settings.json"), logger)
        {
            
        }

        public override AllTemplateEngineSettings CreateDefaultSettings()
        {
            return new AllTemplateEngineSettings();
        }

        public void DiscoverAndRegisterTemplateEngines()
        {
            try
            {
                var settingsDescriptions = ServiceProviderHolder.ServiceProvider
                    .GetServices<ITemplateEngine>()
                    .Select(g => g.SettingsDescription)
                    .ToList();
                _templateEngineSettingsDescriptions.AddRange(settingsDescriptions);
                _logger?.LogInformation("Discovered {Count} template engine settings providers", settingsDescriptions.Count);

                foreach (var settingsDescription in settingsDescriptions)
                {
                    try
                    {
                        RegisterTemplateEngine(settingsDescription);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Failed to register template engine: {Id}", settingsDescription.Id);
                        //throw;
                    }
                    
                }

                // Register generator templates with the TemplateManager
                RegisterTemplateEngineTemplatesWithTemplateManager();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to discover template engine from service provider");
            }
        }

        private void RegisterTemplateEngineTemplatesWithTemplateManager()
        {
            try
            {
                var templateManager = ServiceProviderHolder.GetRequiredService<TemplateManager>();

                foreach (var settingsDescription in _templateEngineSettingsDescriptions)
                {
                    foreach (var template in settingsDescription.Templates)
                    {
                        // Register the template ID to auto-detect special folders

                        templateManager.RegisterRequiredTemplate(template.TemplateId);
                    }
                }

                _logger?.LogDebug("Registered template engine templates with TemplateManager");
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to register template engine templates with TemplateManager");
            }
        }

        private void RegisterTemplateEngine(TemplateEngineSettingsDescription settingsDescription)
        {
            var templateEngineId = settingsDescription.Id;

            // If no settings exist for this generator, create defaults from provider
            if (!Settings.HasTemplateEngineSettings(templateEngineId))
            {
                var settings = settingsDescription.GetDefaultSettings();
                settings.Name = settingsDescription.Name;
                settings.Description = settingsDescription.Description;
                settings.Category = settingsDescription.Category;

                Settings.SetTemplateEngineSettings(settings);

                _logger?.LogInformation("Registered new template engine: {Id} ({Name})", templateEngineId, settingsDescription.Name);
            }
            else
            {
                // Update metadata but keep user settings
                var settings = Settings.GetTemplateEngineSettings(templateEngineId);
                settings.Name = settingsDescription.Name;
                settings.Description = settingsDescription.Description;
                settings.Category = settingsDescription.Category;

                // Merge any new templates from the provider
                MergeTemplatesFromSettingsDescription(templateEngineId, settingsDescription);

                _logger?.LogDebug("Template engine already registered: {Id}", templateEngineId);
            }
        }

        private void MergeTemplatesFromSettingsDescription(string templateEngineId, TemplateEngineSettingsDescription settingsDescription)
        {
            var settings = Settings.GetTemplateEngineSettings(templateEngineId);
            var defaultSettings = settingsDescription.GetDefaultSettings();

            foreach (var template in defaultSettings.Templates)
            {
                if (settings.GetTemplate(template.TemplateId) == null)
                {
                    settings.Templates.Add(new TemplateRequirementSettings
                    {
                        TemplateId = template.TemplateId,
                        Description = template.Description,
                        OutputFileNamePattern = template.OutputFileNamePattern,
                        TemplateType = template.TemplateType,
                        TemplateFilePath = template.TemplateFilePath,
                        Enabled = template.Enabled
                    });

                    _logger?.LogDebug("Added new template {TemplateId} to template engine {TemplateEngineId}",
                        template.TemplateId, templateEngineId);
                }
            }
        }

        public override SettingSection GetSettingsViewModelSection()
        {
            var templateEnginesSection = new SettingSection("templateEngines", "Template Engines Settings") { IconKey = "settings" };
            var settingsDescriptions = _templateEngineSettingsDescriptions.Where(p => p.DependingTemplateIds.Count == 0).ToList();
            foreach (var settingsDescription in settingsDescriptions)
            {
                CreateTemplateEngineSettings(templateEnginesSection, settingsDescription);
            }

            return templateEnginesSection;
        }

        private void CreateTemplateEngineSettings(SettingSection templateEnginesSection, TemplateEngineSettingsDescription settingsDescription)
        {
            var templateEngineId = settingsDescription.Id;
            var templateEngineSettings = GetTemplateEngineSettings(templateEngineId);
            var templateEngineSection = new SettingSection(templateEngineId, settingsDescription.Name)
            {
                IconKey = "generator"
            };
            PropertyChangedEventHandler fieldViewModelPropertyChangedValueSetter = (sender, e) =>
            {
                if (e.PropertyName == nameof(FieldViewModelBase.Value))
                {
                    if (sender is FieldViewModelBase fieldViewModel)
                    {
                        var value = fieldViewModel.Value;
                        SetParameter<object>(templateEngineId, fieldViewModel.Name, value);
                    }
                }
            };
            var enabledFieldSettingsItem = CreateEnabledFieldSettingsItem(templateEngineId, templateEngineSettings.Enabled, templateEngineSection);
            templateEngineSection.Items.Add(enabledFieldSettingsItem);

            foreach (var parameterDefinition in settingsDescription.ParameterDefinitions)
            {
                object parameterValue = templateEngineSettings.GetParameter<object>(parameterDefinition.Name, parameterDefinition.DefaultValue);
                if (parameterValue is System.Text.Json.JsonElement jsonParameterValue)
                {
                    if (jsonParameterValue.ValueKind == JsonValueKind.String)
                    {
                        parameterValue = jsonParameterValue.GetString() ?? string.Empty;
                    }
                    else if (jsonParameterValue.ValueKind == JsonValueKind.Number)
                    {
                        parameterValue = jsonParameterValue.GetInt32();
                    }
                    else if (jsonParameterValue.ValueKind == JsonValueKind.True || jsonParameterValue.ValueKind == JsonValueKind.False)
                    {
                        parameterValue = jsonParameterValue.GetBoolean();
                    }
                    else
                    {
                        throw new InvalidOperationException("Unsupported JsonElement value kind for parameter");
                    }
                }
                if (parameterDefinition.Type == ParameterDefinitionTypes.Template)
                {
                    var templateManager = ServiceProviderHolder.GetRequiredService<TemplateManager>();
                    var allowedEngines = parameterDefinition.PossibleValues?
                        .OfType<TemplateType>()
                        .ToList() ?? new List<TemplateType>();
                    var templates = templateManager.GetTemplatesByType(allowedEngines);
                    var items = templates.Select(t => new ComboboxItem
                    {
                        Value = t.TemplateId,
                        DisplayName = $"{t.TemplateId} ({t.TemplateType})"
                    }).ToList();
                    var comboBoxField = new ComboboxFieldModel
                    {
                        Label = parameterDefinition.Name,
                        Name = parameterDefinition.Name,
                        IsRequired = parameterDefinition.Required,
                        Items = items,
                        Value = parameterValue
                    };
                    comboBoxField.PropertyChanged += fieldViewModelPropertyChangedValueSetter;
                    var comboBoxSettingsItem = new SettingsItem<ComboboxFieldModel>(comboBoxField, parameterDefinition.Name, parameterDefinition.Name, $"Parameter: {parameterDefinition.Name} : {parameterDefinition.Type}");
                    templateEngineSection.Items.Add(comboBoxSettingsItem);
                }
                else if (parameterDefinition.Type != ParameterDefinitionTypes.ParameterisedString && parameterDefinition.PossibleValues != null && parameterDefinition.PossibleValues.Count > 0)
                {
                    var possibleValues = parameterDefinition.PossibleValues;
                    List<ComboboxItem> items;
                    if (possibleValues.All(v => v is string))
                        items = parameterDefinition.PossibleValues.Select(stringValue => new ComboboxItem { Value = stringValue, DisplayName = (string)stringValue }).ToList();
                    else if (possibleValues.All(obj => obj is ComboboxItem))
                        items = parameterDefinition.PossibleValues.Cast<ComboboxItem>().ToList();
                    else
                        items = parameterDefinition.PossibleValues.Select(objV => new ComboboxItem { Value = objV, DisplayName = objV.ToString() }).ToList();

                    var comboBoxField = new ComboboxFieldModel
                    {
                        Label = parameterDefinition.Name,
                        Name = parameterDefinition.Name,
                        IsRequired = parameterDefinition.Required,
                        Items = items,
                        Value = parameterValue
                    };
                    comboBoxField.PropertyChanged += fieldViewModelPropertyChangedValueSetter;
                    var comboBoxSettingsItem = new SettingsItem<ComboboxFieldModel>(comboBoxField, parameterDefinition.Name, parameterDefinition.Name, $"Parameter: {parameterDefinition.Name} : {parameterDefinition.Type}");
                    templateEngineSection.Items.Add(comboBoxSettingsItem);
                }
                else
                {
                    switch (parameterDefinition.Type)
                    {
                        case ParameterDefinitionTypes.Integer:
                            // IntegerField
                            var intField = new IntegerFieldModel
                            {
                                Label = parameterDefinition.Name,
                                Name = parameterDefinition.Name,
                                IsRequired = parameterDefinition.Required,
                                Value = parameterValue != null ? Convert.ToInt32(parameterValue) : 0
                            };
                            intField.PropertyChanged += fieldViewModelPropertyChangedValueSetter;
                            var intSettingsItem = new SettingsItem<IntegerFieldModel>(intField, parameterDefinition.Name, parameterDefinition.Name, $"Parameter: {parameterDefinition.Name} : {parameterDefinition.Type}");
                            templateEngineSection.Items.Add(intSettingsItem);
                            break;
                        case ParameterDefinitionTypes.String:
                            var parameterField = new SingleLineTextFieldModel
                            {
                                Label = parameterDefinition.Name,
                                Name = parameterDefinition.Name,
                                IsRequired = parameterDefinition.Required,
                                Value = parameterValue ?? string.Empty
                            };
                            parameterField.PropertyChanged += fieldViewModelPropertyChangedValueSetter;
                            var parameterSettingsItem = new SettingsItem<SingleLineTextFieldModel>(parameterField, parameterDefinition.Name, parameterDefinition.Name, $"Parameter: {parameterDefinition.Name} : {parameterDefinition.Type}");
                            templateEngineSection.Items.Add(parameterSettingsItem);
                            break;
                        case ParameterDefinitionTypes.Boolean:
                            // BooleanField
                            var boolField = new BooleanFieldModel
                            {
                                Label = parameterDefinition.Name,
                                Name = parameterDefinition.Name,
                                IsRequired = parameterDefinition.Required,
                                Value = parameterValue != null ? Convert.ToBoolean(parameterValue) : false
                            };
                            boolField.PropertyChanged += fieldViewModelPropertyChangedValueSetter;
                            var boolSettingsItem = new SettingsItem<BooleanFieldModel>(boolField, parameterDefinition.Name, parameterDefinition.Name, $"Parameter: {parameterDefinition.Name} : {parameterDefinition.Type}");
                            templateEngineSection.Items.Add(boolSettingsItem);
                            break;
                        case ParameterDefinitionTypes.ParameterisedString:
                            var parameterizedStringField = new ParameterizedStringFieldModel();

                            if (parameterValue is string stringValue)
                            {
                                parameterizedStringField.Value = stringValue;
                            }
                            else if (parameterValue is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.String)
                            {
                                parameterizedStringField.Value = jsonElement.GetString();// ?? string.Empty;
                            }
                            else
                            {
                                throw new InvalidOperationException("Invalid parameter value type for ParameterisedString");
                            }
                            parameterizedStringField.Label = parameterDefinition.Name;
                            parameterizedStringField.Name = parameterDefinition.Name;
                            parameterizedStringField.IsRequired = parameterDefinition.Required;
                            if (parameterDefinition.PossibleValues != null)
                            {
                                var possibleParameters = parameterDefinition.PossibleValues
                                    .OfType<ParameterizedStringParameter>()
                                    .ToList();
                                foreach (var param in possibleParameters)
                                {
                                    parameterizedStringField.Parameters.Add(param);
                                }
                            }
                            parameterizedStringField.RefreshParameterizedString();
                            parameterizedStringField.PropertyChanged += fieldViewModelPropertyChangedValueSetter;
                            var parameterizedStringSettingsItem = new SettingsItem<ParameterizedStringFieldModel>(parameterizedStringField, parameterDefinition.Name, parameterDefinition.Name, $"Parameter: {parameterDefinition.Name} : {parameterDefinition.Type}");
                            templateEngineSection.Items.Add(parameterizedStringSettingsItem);
                            break;
                        default:
                            throw new NotImplementedException($"Unknown parameterdefinitiontype {parameterDefinition.Type}");
                            break;
                    }
                }
            }

            if (templateEngineSettings.Templates.Count > 0)
            {
                // Add template settings
                var templatesSection = new SettingSection("templates", "Templates")
                {
                    IconKey = "template"
                };
                foreach (var templateDefinition in settingsDescription.Templates)
                {
                    var template = templateEngineSettings.GetTemplate(templateDefinition.TemplateId);

                    if (template == null)
                        template = templateDefinition.ToSettings();

                    var templateSection = new SettingSection(template.TemplateId, template.Description)
                    {
                        IconKey = "file",
                        Items = new SettingsItemCollection()
                    };
                    switch (template.TemplateType)
                    {
                        case TemplateType.Scriban:

                        case TemplateType.T4:

                        case TemplateType.TextFile:

                        case TemplateType.BinaryFile:

                        case TemplateType.ImageFile:

                            AddTemplateFileFieldSettingsItem(templateEngineId, template, templateSection);
                            break;
                        case TemplateType.Folder:
                            AddTemplateFolderFieldSettingsItem(templateEngineId, template, templateSection);
                            break;
                        default:
                            break;
                    }


                    templatesSection.Sections.Add(templateSection);
                }
                templateEngineSection.Sections.Add(templatesSection);
            }
            // add depending generators
            foreach (var dependingProvider in _templateEngineSettingsDescriptions.Where(p => p.DependingTemplateIds.Contains((string)settingsDescription.Id)))
            {
                CreateTemplateEngineSettings(templateEngineSection, dependingProvider);
            }
            templateEnginesSection.Sections.Add(templateEngineSection);
        }

        private void SetParameter<T>(string templateEngineId, string paramName, T value)
        {
            Settings.GetTemplateEngineSettings(templateEngineId).SetParameter(paramName, value);
        }

        public TemplateEngineSettings GetTemplateEngineSettings(string templateEngineId)
        {
            return Settings.GetTemplateEngineSettings(templateEngineId);
        }

        private void AddTemplateFolderFieldSettingsItem(string templateEngineId, TemplateRequirementSettings template, SettingSection templateSection)
        {
            var templateFolderField = new FolderFieldModel
            {
                Label = template.TemplateId,
                Name = template.TemplateId,
                IsRequired = false,
                Value = template.Enabled
            };
            templateFolderField.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(FieldViewModelBase.Value))
                {
                    if (sender is FieldViewModelBase fieldViewModel)
                    {
                        var value = fieldViewModel.Value as string;
                        SetTemplateFilePath(templateEngineId, template.TemplateId, value);
                    }
                }
            };
            var settingsItem = new SettingsItem<FolderFieldModel>(templateFolderField, template.TemplateId, "Template Folder", "Path to the template folder");
            templateSection.Items.Add(settingsItem);
        }
        /// <summary>
        /// Set the file path for a template
        /// </summary>
        public void SetTemplateFilePath(string templateEngineId, string templateId, string filePath)
        {
            var settings = Settings.GetTemplateEngineSettings(templateEngineId);
            var template = settings.GetTemplate(templateId);

            if (template != null)
            {
                template.TemplateFilePath = filePath;
            }
        }

        private void AddTemplateFileFieldSettingsItem(string templateEngineId, TemplateRequirementSettings template, SettingSection templateSection)
        {
            string filter = template.TemplateType switch
            {
                TemplateType.Scriban => "Scriban Template Files|*.scriban;*.txt|All Files|*.*",
                TemplateType.T4 => "T4 Template Files|*.t4;*.tt|All Files|*.*",
                TemplateType.TextFile => "Text Files|*.txt;*.template|All Files|*.*",
                TemplateType.BinaryFile => "Binary Files|*.bin;*.dat|All Files|*.*",
                TemplateType.ImageFile => "Image Files|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All Files|*.*",
                _ => "All Files|*.*"
            };
            var templateFileField = new FileFieldModel
            {
                Label = template.TemplateId,
                Name = template.TemplateId,
                IsRequired = false,
                Value = template.TemplateFilePath,
                SelectionMode = FileSelectionMode.Open,
                Filter = filter,
            };
            templateFileField.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(FieldViewModelBase.Value))
                {
                    if (sender is FieldViewModelBase fieldViewModel)
                    {
                        var value = fieldViewModel.Value as string;
                        SetTemplateFilePath(templateEngineId, template.TemplateId, value);
                    }
                }
            };
            var settingsItem = new SettingsItem<FileFieldModel>(templateFileField, template.TemplateId, "Template File", "Path to the template file");
            templateSection.Items.Add(settingsItem);
        }

        private SettingsItem<BooleanFieldModel> CreateEnabledFieldSettingsItem(string templateEngineId, bool initialValue, SettingSection generatorSection)
        {
            var enabledField = new BooleanFieldModel
            {
                Label = "Enabled",
                Name = "enabled",
                IsRequired = false,
                Value = initialValue
            };
            enabledField.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(FieldViewModelBase.Value))
                {
                    if (sender is FieldViewModelBase fieldViewModel)
                    {
                        var value = Convert.ToBoolean(fieldViewModel.Value);
                        SetTemplateEngineEnabled(templateEngineId, value);
                    }
                }
            };
            var enabledSettingsItem = new SettingsItem<BooleanFieldModel>(enabledField, "enabled", "Enabled", "Enable or disable this generator");
            return enabledSettingsItem;
        }

        private void SetTemplateEngineEnabled(string templateEngineId, bool enabled)
        {
            var settings = Settings.GetTemplateEngineSettings(templateEngineId);
            settings.Enabled = enabled;
        }
    }
}
