using CodeGenerator.Core.Settings.Application;
using CodeGenerator.Core.Settings.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Settings.Interfaces
{
    public abstract class LocalApplicationDataSettingsManager<TSettings> : ISettingsManager where TSettings : LocalApplicationDataSettingsBase, new()
    {
        public event EventHandler? SettingsSaved;
        private TSettings _settings;
        public TSettings Settings => _settings;

        private readonly string _settingsFilePath;
        protected readonly ILogger _logger;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        protected void RaiseSettingsSavedEvent()
        {
            SettingsSaved?.Invoke(this, EventArgs.Empty);
        }

        protected LocalApplicationDataSettingsManager(string settingsFilePath, ILogger logger)
        {
            _settingsFilePath = settingsFilePath;
            _logger = logger;
            _settings = CreateDefaultSettings();
        }
        protected virtual string SettingsModelName => typeof(TSettings).Name;
        #region File Path

        /// <summary>
        /// Get the default settings file path (in user's AppData folder)
        /// </summary>
        /// <param name="applicationName">The name of the application. eg. "CodeGenerator"</param>
        /// <param name="settingsFileName">The name of the settings file. eg. "generator-settings.json"</param>
        protected static string GetDefaultSettingsFilePath(string applicationName, string settingsFileName)
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(appDataPath, applicationName);

            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            return Path.Combine(appFolder, settingsFileName);
        }

    
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
                    _settings = JsonSerializer.Deserialize<TSettings>(json, _jsonOptions)
                        ?? CreateDefaultSettings();

                    _logger?.LogInformation("{SettingsModelName} settings loaded from {Path}", SettingsModelName, _settingsFilePath);
                }
                else
                {
                    _settings = CreateDefaultSettings();
                    _logger?.LogInformation("No existing {SettingsModelName} settings found, using defaults", SettingsModelName);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to load {SettingsModelName} settings from {Path}", SettingsModelName, _settingsFilePath);
                _settings = CreateDefaultSettings();
            }
        }

        /// <summary>
        /// Save all generator settings to JSON file
        /// </summary>
        public void SaveSettings()
        {
            try
            {
                _settings.LastModified = DateTime.UtcNow;
                var json = JsonSerializer.Serialize(_settings, _jsonOptions);

                // Ensure directory exists
                var directory = Path.GetDirectoryName(_settingsFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(_settingsFilePath, json);
                
                _logger?.LogInformation("{SettingsModelName} settings saved to {Path}", SettingsModelName, _settingsFilePath);

                RaiseSettingsSavedEvent();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to save {SettingsModelName} settings to {Path}", SettingsModelName, _settingsFilePath);
                throw;
            }
        }

        #endregion
        /// <summary>
        /// Get the current settings file path
        /// </summary>
        public string GetSettingsFilePath()
        {
            return _settingsFilePath;
        }

        public abstract SettingSection GetSettingsViewModelSection();

        public abstract TSettings CreateDefaultSettings();

        /// <summary>
        /// Restore default settings (but do not save to file)
        /// </summary>
        public void ResetSettings()
        {
            _settings = CreateDefaultSettings();
        }

    }
}
