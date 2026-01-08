using CodeGenerator.Core.Settings.Models;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;
using System.Configuration;

namespace CodeGenerator.Core.Settings.Application
{
    /// <summary>
    /// Service for managing application settings
    /// Provides easy access to load, save, and manage settings
    /// </summary>
    public class ApplicationSettingsManager
    {
        private readonly ApplicationSettings _settings;

        public ApplicationSettingsManager()
        {
            _settings = ApplicationSettings.Instance;
        }

        #region Load & Save

        /// <summary>
        /// Load settings from user.config file
        /// Settings are automatically loaded on first access, but this can be called explicitly
        /// </summary>
        public void LoadSettings()
        {
            _settings.Reload();
        }

        /// <summary>
        /// Save all settings to user.config file
        /// </summary>
        public void SaveSettings()
        {
            _settings.Save();
        }

        /// <summary>
        /// Reset all settings to their default values
        /// </summary>
        public void ResetSettings()
        {
            _settings.ResetToDefaults();
        }

        #endregion

        #region Language

        /// <summary>
        /// Get or set the interface language
        /// </summary>
        public string InterfaceLanguage
        {
            get => _settings.InterfaceLanguage;
            set
            {
                _settings.InterfaceLanguage = value;
                _settings.Save();
            }
        }

        /// <summary>
        /// Get available languages
        /// </summary>
        public static string[] GetAvailableLanguages()
        {
            return new[]
            {
                "nl-NL", // Nederlands
                "en-US", // English (US)
                "en-GB", // English (UK)
                "fr-FR", // Français
                "de-DE", // Deutsch
                "es-ES", // Español
            };
        }

        #endregion

        #region Theme

        /// <summary>
        /// Get or set the Syncfusion theme
        /// </summary>
        public string Theme
        {
            get => _settings.Theme;
            set
            {
                _settings.Theme = value;
                _settings.Save();
            }
        }

        /// <summary>
        /// Get available Syncfusion themes
        /// </summary>
        public static string[] GetAvailableThemes()
        {
            return new[]
            {
                "Office2019Colorful",
                "Office2019Black",
                "Office2019White",
                "Office2016Colorful",
                "Office2016Black",
                "Office2016White",
                "HighContrastBlack",
                "MaterialLight",
                "MaterialDark",
            };
        }

        #endregion

        #region Recent Files

        /// <summary>
        /// Get list of recent files
        /// </summary>
        public List<string> GetRecentFiles()
        {
            var result = new List<string>();
            foreach (string file in _settings.RecentFiles)
            {
                // Only include files that still exist
                if (File.Exists(file))
                {
                    result.Add(file);
                }
            }
            return result;
        }

        /// <summary>
        /// Add a file to recent files list
        /// </summary>
        public void AddRecentFile(string filePath)
        {
            _settings.AddRecentFile(filePath);
        }

        /// <summary>
        /// Clear all recent files
        /// </summary>
        public void ClearRecentFiles()
        {
            _settings.ClearRecentFiles();
        }

        #endregion

        #region Favorite Files

        /// <summary>
        /// Get list of favorite files
        /// </summary>
        public List<string> GetFavoriteFiles()
        {
            var result = new List<string>();
            foreach (string file in _settings.FavoriteFiles)
            {
                result.Add(file);
            }
            return result;
        }

        /// <summary>
        /// Add a file to favorites
        /// </summary>
        public void AddFavoriteFile(string filePath)
        {
            _settings.AddFavoriteFile(filePath);
        }

        /// <summary>
        /// Remove a file from favorites
        /// </summary>
        public void RemoveFavoriteFile(string filePath)
        {
            _settings.RemoveFavoriteFile(filePath);
        }

        /// <summary>
        /// Check if a file is in favorites
        /// </summary>
        public bool IsFavorite(string filePath)
        {
            return _settings.FavoriteFiles.Contains(filePath);
        }

        #endregion

        #region Window State

        /// <summary>
        /// Save window state (size and position)
        /// </summary>
        public void SaveWindowState(int width, int height, bool maximized)
        {
            _settings.WindowWidth = width;
            _settings.WindowHeight = height;
            _settings.WindowMaximized = maximized;
            _settings.Save();
        }

        /// <summary>
        /// Get saved window state
        /// </summary>
        public (int Width, int Height, bool Maximized) GetWindowState()
        {
            return (_settings.WindowWidth, _settings.WindowHeight, _settings.WindowMaximized);
        }

        #endregion

        #region Settings Info

        /// <summary>
        /// Get the path to the user.config file where settings are stored
        /// </summary>
        public string GetSettingsFilePath()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            return config.FilePath;
        }

        /// <summary>
        /// Get all settings as a dictionary for debugging
        /// </summary>
        public Dictionary<string, object?> GetAllSettings()
        {
            var result = new Dictionary<string, object?>();

            foreach (SettingsProperty property in _settings.Properties)
            {
                result[property.Name] = _settings[property.Name];
            }

            return result;
        }

        public SettingSection GetSettingsViewModelSection()
        {
            var applicationSection = new SettingSection("application", "Application Settings") { IconKey = "settings" };

            var interfaceLanguageField = new ComboboxFieldModel {
                Items = GetAvailableLanguages().Select(language => new { Id = language, DisplayName = language }).ToArray(),
                Label = "Interface Language",
                Name = "interfaceLanguage",
                IsRequired = true,
                Value = _settings.InterfaceLanguage
            };
            interfaceLanguageField.PropertyChanged += InterfaceLanguageField_PropertyChanged;
            interfaceLanguageField.Disposed += InterfaceLanguageField_Disposed;
            var interfaceLanguageSetting = new SettingsItem<ComboboxFieldModel>(interfaceLanguageField, "interfaceLanguage", "Interface Language", _settings.InterfaceLanguage);
            applicationSection.Items.Add(interfaceLanguageSetting);

            var themeField = new ComboboxFieldModel { 
                Items = GetAvailableThemes().Select(theme => new { Id=theme, DisplayName=theme }).ToArray() ,
                Label = "Theme",
                Name = "theme",
                IsRequired = true,
                Value = _settings.Theme
            };
            themeField.PropertyChanged += ThemeField_PropertyChanged;
            themeField.Disposed += ThemeField_Disposed;
            var themeSetting = new SettingsItem<ComboboxFieldModel>(themeField, "theme", "Theme", _settings.Theme);
            
            applicationSection.Items.Add(themeSetting);

            return applicationSection;
        }

        private void InterfaceLanguageField_Disposed(object? sender, EventArgs e)
        {
            ((ComboboxFieldModel)sender!).PropertyChanged -= InterfaceLanguageField_PropertyChanged;
            ((ComboboxFieldModel)sender!).Disposed -= InterfaceLanguageField_Disposed;
        }

        private void InterfaceLanguageField_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _settings.InterfaceLanguage = ((ComboboxFieldModel)sender!).Value as string;
        }

        private void ThemeField_Disposed(object? sender, EventArgs e)
        {
            ((ComboboxFieldModel)sender!).PropertyChanged -= ThemeField_PropertyChanged;
            ((ComboboxFieldModel)sender!).Disposed -= ThemeField_Disposed;
        }

        private void ThemeField_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _settings.Theme = ((ComboboxFieldModel)sender!).Value as string;
        }

        #endregion
    }
}
