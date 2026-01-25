using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Settings.Models;
using CodeGenerator.Core.Templates;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Models;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Reflection;
using System.Runtime;
using System.Text.Json;

namespace CodeGenerator.Core.Settings.Generators
{
    /// <summary>
    /// Service for managing generator-specific settings.
    /// Stores settings as JSON and integrates with registered IGeneratorSettingsProvider instances.
    /// </summary>
    public class GeneratorSettingsManager
    {
        private readonly ILogger<GeneratorSettingsManager>? _logger;
        private AllGeneratorSettings _allSettings;
        private readonly string _settingsFilePath;
        private readonly List<IGeneratorSettingsProvider> _generatorSettingsProviders = new();
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public GeneratorSettingsManager(ILogger<GeneratorSettingsManager>? logger = null)
        {
            _logger = logger;
            _settingsFilePath = GetDefaultSettingsFilePath();
            _allSettings = new AllGeneratorSettings();
        }

        public GeneratorSettingsManager(string settingsFilePath, ILogger<GeneratorSettingsManager>? logger = null)
        {
            _logger = logger;
            _settingsFilePath = settingsFilePath;
            _allSettings = new AllGeneratorSettings();
        }

        #region File Path

        /// <summary>
        /// Get the default settings file path (in user's AppData folder)
        /// </summary>
        private static string GetDefaultSettingsFilePath()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(appDataPath, "CodeGenerator");
            
            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            return Path.Combine(appFolder, "generator-settings.json");
        }

        /// <summary>
        /// Get the current settings file path
        /// </summary>
        public string SettingsFilePath => _settingsFilePath;

        #endregion

        #region Load & Save

        /// <summary>
        /// Load all generator settings from JSON file
        /// </summary>
        public void LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    var json = File.ReadAllText(_settingsFilePath);
                    _allSettings = JsonSerializer.Deserialize<AllGeneratorSettings>(json, _jsonOptions) 
                        ?? new AllGeneratorSettings();
                    
                    _logger?.LogInformation("Generator settings loaded from {Path}", _settingsFilePath);
                }
                else
                {
                    _allSettings = new AllGeneratorSettings();
                    _logger?.LogInformation("No existing generator settings found, using defaults");
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to load generator settings from {Path}", _settingsFilePath);
                _allSettings = new AllGeneratorSettings();
            }
        }

        /// <summary>
        /// Save all generator settings to JSON file
        /// </summary>
        public void SaveSettings()
        {
            try
            {
                _allSettings.LastModified = DateTime.UtcNow;
                var json = JsonSerializer.Serialize(_allSettings, _jsonOptions);
                
                // Ensure directory exists
                var directory = Path.GetDirectoryName(_settingsFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(_settingsFilePath, json);
                _logger?.LogInformation("Generator settings saved to {Path}", _settingsFilePath);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to save generator settings to {Path}", _settingsFilePath);
                throw;
            }
        }

        #endregion

        #region Generator Registration

        /// <summary>
        /// Discover and register all IGeneratorSettingsProvider instances from the service provider.
        /// Initializes settings for generators that don't have saved settings yet.
        /// </summary>
        public void DiscoverAndRegisterGenerators()
        {
            try
            {
                var providers = ServiceProviderHolder.ServiceProvider
                    .GetServices<IMessageBusAwareGenerator>()
                    .Select(g => g.SettingsDescription as IGeneratorSettingsProvider)
                    .ToList();
                _generatorSettingsProviders.AddRange(providers);
                _logger?.LogInformation("Discovered {Count} generator settings providers", providers.Count);

                foreach (var provider in providers)
                {
                    RegisterGenerator(provider);
                }

                // Register generator templates with the TemplateManager
                RegisterGeneratorTemplatesWithTemplateManager();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to discover generators from service provider");
            }
        }

        /// <summary>
        /// Register generator templates with the TemplateManager for special folder resolution
        /// </summary>
        private void RegisterGeneratorTemplatesWithTemplateManager()
        {
            try
            {
                var templateManager = ServiceProviderHolder.GetRequiredService<TemplateManager>();

                foreach (var provider in _generatorSettingsProviders)
                {
                    if (provider is GeneratorSettingsDescription description)
                    {
                        foreach (var template in description.Templates)
                        {
                            // Register the template ID to auto-detect special folders
                            
                            templateManager.RegisterRequiredTemplate(template.TemplateId);
                        }
                    }
                }

                _logger?.LogDebug("Registered generator templates with TemplateManager");
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to register generator templates with TemplateManager");
            }
        }

        /// <summary>
        /// Register a single generator and initialize its settings if not already present
        /// </summary>
        public void RegisterGenerator(IGeneratorSettingsProvider provider)
        {
            var generatorId = provider.GeneratorId;

            // If no settings exist for this generator, create defaults from provider
            if (!_allSettings.HasGeneratorSettings(generatorId))
            {
                var settings = provider.GetDefaultSettings();
                settings.Name = provider.GeneratorName;
                settings.Description = provider.GeneratorDescription;
                settings.Category = provider.Category;
                
                _allSettings.SetGeneratorSettings(settings);
                
                _logger?.LogInformation("Registered new generator: {Id} ({Name})", generatorId, provider.GeneratorName);
            }
            else
            {
                // Update metadata but keep user settings
                var settings = _allSettings.GetGeneratorSettings(generatorId);
                settings.Name = provider.GeneratorName;
                settings.Description = provider.GeneratorDescription;
                settings.Category = provider.Category;

                // Merge any new templates from the provider
                MergeTemplatesFromProvider(generatorId, provider);
                
                _logger?.LogDebug("Generator already registered: {Id}", generatorId);
            }
        }

        /// <summary>
        /// Merge new templates from provider into existing settings
        /// (adds new templates, doesn't overwrite existing)
        /// </summary>
        private void MergeTemplatesFromProvider(string generatorId, IGeneratorSettingsProvider provider)
        {
            var settings = _allSettings.GetGeneratorSettings(generatorId);
            var defaultSettings = provider.GetDefaultSettings();

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

                    _logger?.LogDebug("Added new template {TemplateId} to generator {GeneratorId}", 
                        template.TemplateId, generatorId);
                }
            }
        }

        #endregion

        #region Get/Set Generator Settings

        /// <summary>
        /// Get all registered generator settings
        /// </summary>
        public AllGeneratorSettings GetAllSettings() => _allSettings;

        /// <summary>
        /// Get settings for a specific generator
        /// </summary>
        public GeneratorSettings GetGeneratorSettings(string generatorId)
        {
            return _allSettings.GetGeneratorSettings(generatorId);
        }

        /// <summary>
        /// Update settings for a specific generator
        /// </summary>
        public void UpdateGeneratorSettings(GeneratorSettings settings)
        {
            _allSettings.SetGeneratorSettings(settings);
        }

        /// <summary>
        /// Get all registered generator IDs
        /// </summary>
        public IEnumerable<string> GetRegisteredGeneratorIds()
        {
            return _allSettings.Generators.Keys;
        }

        /// <summary>
        /// Check if a generator is enabled
        /// </summary>
        public bool IsGeneratorEnabled(string generatorId)
        {
            return _allSettings.GetGeneratorSettings(generatorId).Enabled;
        }

        /// <summary>
        /// Enable or disable a generator
        /// </summary>
        public void SetGeneratorEnabled(string generatorId, bool enabled)
        {
            var settings = _allSettings.GetGeneratorSettings(generatorId);
            settings.Enabled = enabled;
        }

        #endregion

        #region Template Settings

        /// <summary>
        /// Get templates for a specific generator
        /// </summary>
        public List<TemplateRequirementSettings> GetTemplates(string generatorId)
        {
            return _allSettings.GetGeneratorSettings(generatorId).Templates;
        }

        /// <summary>
        /// Update a template for a generator
        /// </summary>
        public void UpdateTemplate(string generatorId, TemplateRequirementSettings template)
        {
            var settings = _allSettings.GetGeneratorSettings(generatorId);
            settings.SetTemplate(template);
        }

        /// <summary>
        /// Set the file path for a template
        /// </summary>
        public void SetTemplateFilePath(string generatorId, string templateId, string filePath)
        {
            var settings = _allSettings.GetGeneratorSettings(generatorId);
            var template = settings.GetTemplate(templateId);
            
            if (template != null)
            {
                template.TemplateFilePath = filePath;
            }
        }

        /// <summary>
        /// Enable or disable a template
        /// </summary>
        public void SetTemplateEnabled(string generatorId, string templateId, bool enabled)
        {
            var settings = _allSettings.GetGeneratorSettings(generatorId);
            var template = settings.GetTemplate(templateId);
            
            if (template != null)
            {
                template.Enabled = enabled;
            }
        }

        #endregion

        #region Parameter Settings

        /// <summary>
        /// Get a parameter value for a generator
        /// </summary>
        public T? GetParameter<T>(string generatorId, string parameterName, T? defaultValue = default)
        {
            return _allSettings.GetGeneratorSettings(generatorId).GetParameter(parameterName, defaultValue);
        }

        /// <summary>
        /// Set a parameter value for a generator
        /// </summary>
        public void SetParameter<T>(string generatorId, string parameterName, T? value)
        {
            _allSettings.GetGeneratorSettings(generatorId).SetParameter(parameterName, value);
        }

        public SettingSection GetSettingsViewModelSection()
        {
            var generatorsSection = new SettingSection("generators", "Generators Settings") { IconKey = "settings" };
            var rootProviders = _generatorSettingsProviders.Where(p => p.DependingGeneratorIds.Count == 0).ToList();
            foreach (var rootProvider in rootProviders)
            {
                CreateGeneratorSettings(generatorsSection, rootProvider);
            }

            return generatorsSection;
        }

        private void CreateGeneratorSettings(SettingSection generatorsSection, IGeneratorSettingsProvider generatorSettingsProvider)
        {
            var generatorId = generatorSettingsProvider.GeneratorId;
            var generatorSettings = GetGeneratorSettings(generatorId);
            var generatorSection = new SettingSection(generatorId, generatorSettingsProvider.GeneratorName)
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
                        SetParameter<object>(generatorId, fieldViewModel.Name, value);
                    }
                }
            };
            var enabledFieldSettingsItem = CreateEnabledFieldSettingsItem(generatorId, generatorSettings.Enabled, generatorSection);
            generatorSection.Items.Add(enabledFieldSettingsItem);

            foreach (var parameterDefinition in generatorSettingsProvider.ParameterDefinitions)
            {
                object parameterValue = generatorSettings.GetParameter<object>(parameterDefinition.Name, parameterDefinition.DefaultValue);
                if( parameterValue is System.Text.Json.JsonElement jsonParameterValue)
                {
                    if(jsonParameterValue.ValueKind == JsonValueKind.String)
                    {
                        parameterValue = jsonParameterValue.GetString() ?? string.Empty;
                    } else if (jsonParameterValue.ValueKind == JsonValueKind.Number)
                    {
                        parameterValue = jsonParameterValue.GetInt32();
                    } else if (jsonParameterValue.ValueKind == JsonValueKind.True || jsonParameterValue.ValueKind == JsonValueKind.False)
                    {
                        parameterValue = jsonParameterValue.GetBoolean();
                    }else
                    {
                        throw new InvalidOperationException("Unsupported JsonElement value kind for parameter");
                    }
                }
                if(parameterDefinition.Type == ParameterDefinitionTypes.Template)
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
                    generatorSection.Items.Add(comboBoxSettingsItem);
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
                    generatorSection.Items.Add(comboBoxSettingsItem);
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
                            generatorSection.Items.Add(intSettingsItem);
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
                            generatorSection.Items.Add(parameterSettingsItem);
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
                            generatorSection.Items.Add(boolSettingsItem);
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
                            generatorSection.Items.Add(parameterizedStringSettingsItem);
                            break;
                        default:
                            throw new NotImplementedException($"Unknown parameterdefinitiontype {parameterDefinition.Type}");
                            break;
                    }
                }
            }

            if (generatorSettings.Templates.Count > 0)
            {
                // Add template settings
                var templatesSection = new SettingSection("templates", "Templates")
                {
                    IconKey = "template"
                };
                foreach (var templateDefinition in generatorSettingsProvider.Templates)
                {
                    var template = generatorSettings.GetTemplate(templateDefinition.TemplateId);

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

                            AddTemplateFileFieldSettingsItem(generatorId, template, templateSection);
                            break;
                        case TemplateType.Folder:
                            AddTemplateFolderFieldSettingsItem(generatorId,template, templateSection);
                            break;
                        default:
                            break;
                    }


                    templatesSection.Sections.Add(templateSection);
                }
                generatorSection.Sections.Add(templatesSection);
            }
            // add depending generators
            foreach (var dependingProvider in _generatorSettingsProviders.Where(p => p.DependingGeneratorIds.Contains(generatorSettingsProvider.GeneratorId)))
            {
                CreateGeneratorSettings(generatorSection, dependingProvider);
            }
            generatorsSection.Sections.Add(generatorSection);
        }


        private void AddTemplateFolderFieldSettingsItem(string generatorId, TemplateRequirementSettings template, SettingSection templateSection)
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
                        SetTemplateFilePath(generatorId, template.TemplateId, value);
                    }
                }
            };
            var settingsItem = new SettingsItem<FolderFieldModel>(templateFolderField, template.TemplateId, "Template Folder", "Path to the template folder");
            templateSection.Items.Add(settingsItem);
        }

        private void AddTemplateFileFieldSettingsItem(string generatorId, TemplateRequirementSettings template, SettingSection templateSection)
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
                        SetTemplateFilePath(generatorId, template.TemplateId, value);
                    }
                }
            };
            var settingsItem = new SettingsItem<FileFieldModel>(templateFileField, template.TemplateId, "Template File", "Path to the template file");
            templateSection.Items.Add(settingsItem);
        }

        private SettingsItem<BooleanFieldModel> CreateEnabledFieldSettingsItem(string generatorId, bool initialValue, SettingSection generatorSection)
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
                        SetGeneratorEnabled(generatorId, value);
                    }
                }
            };
            var enabledSettingsItem = new SettingsItem<BooleanFieldModel>(enabledField, "enabled", "Enabled", "Enable or disable this generator");
            return enabledSettingsItem;
        }

        #endregion
    }
}
