using CodeGenerator.Core.Settings.Models;
using CodeGenerator.UserControls.ViewModels;
using System.Collections.Specialized;
using System.Configuration;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace CodeGenerator.Core.Settings.Application
{
    /// <summary>
    /// Application-wide settings using System.Configuration.ApplicationSettingsBase
    /// Settings are automatically persisted to user.config file
    /// </summary>
    public sealed class ApplicationSettings : ApplicationSettingsBase
    {
        // Singleton pattern for easy access
        private static ApplicationSettings? _instance;
        private static readonly object _lock = new object();

        public static ApplicationSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ApplicationSettings();
                        }
                    }
                }
                return _instance;
            }
        }

        #region Language Settings

        /// <summary>
        /// Interface language (e.g., "nl-NL", "en-US", "fr-FR")
        /// </summary>
        [UserScopedSetting]
        [DefaultSettingValue("en-US")]
        public string InterfaceLanguage
        {
            get { return (string)this[nameof(InterfaceLanguage)]; }
            set { this[nameof(InterfaceLanguage)] = value; }
        }

        #endregion

        #region Theme Settings

        /// <summary>
        /// Syncfusion theme name (e.g., "Office2019Colorful", "HighContrastBlack", "MaterialLight")
        /// </summary>
        [UserScopedSetting]
        [DefaultSettingValue("Office2019Colorful")]
        public string Theme
        {
            get { return (string)this[nameof(Theme)]; }
            set { this[nameof(Theme)] = value; }
        }

        #endregion

        #region Recent Files

        /// <summary>
        /// List of recently opened files (max 10 items)
        /// Stored as StringCollection
        /// </summary>
        [UserScopedSetting]
        [DefaultSettingValue("")]
        public StringCollection RecentFiles
        {
            get
            {
                var collection = (StringCollection?)this[nameof(RecentFiles)];
                if (collection == null)
                {
                    collection = new StringCollection();
                    this[nameof(RecentFiles)] = collection;
                }
                return collection;
            }
            set { this[nameof(RecentFiles)] = value; }
        }

        #endregion

        #region Favorite Files

        /// <summary>
        /// List of favorite files
        /// Stored as StringCollection
        /// </summary>
        [UserScopedSetting]
        [DefaultSettingValue("")]
        public StringCollection FavoriteFiles
        {
            get
            {
                var collection = (StringCollection?)this[nameof(FavoriteFiles)];
                if (collection == null)
                {
                    collection = new StringCollection();
                    this[nameof(FavoriteFiles)] = collection;
                }
                return collection;
            }
            set { this[nameof(FavoriteFiles)] = value; }
        }

        #endregion

        #region Window State Settings

        /// <summary>
        /// Main window width
        /// </summary>
        [UserScopedSetting]
        [DefaultSettingValue("1280")]
        public int WindowWidth
        {
            get { return (int)this[nameof(WindowWidth)]; }
            set { this[nameof(WindowWidth)] = value; }
        }

        /// <summary>
        /// Main window height
        /// </summary>
        [UserScopedSetting]
        [DefaultSettingValue("720")]
        public int WindowHeight
        {
            get { return (int)this[nameof(WindowHeight)]; }
            set { this[nameof(WindowHeight)] = value; }
        }

        /// <summary>
        /// Is main window maximized
        /// </summary>
        [UserScopedSetting]
        [DefaultSettingValue("false")]
        public bool WindowMaximized
        {
            get { return (bool)this[nameof(WindowMaximized)]; }
            set { this[nameof(WindowMaximized)] = value; }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Add a file to recent files list (max 10 items)
        /// </summary>
        public void AddRecentFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            var recentFiles = RecentFiles;

            // Remove if already exists (to move it to top)
            if (recentFiles.Contains(filePath))
            {
                recentFiles.Remove(filePath);
            }

            // Insert at beginning
            recentFiles.Insert(0, filePath);

            // Keep only last 10 items
            while (recentFiles.Count > 10)
            {
                recentFiles.RemoveAt(recentFiles.Count - 1);
            }

            Save();
        }

        /// <summary>
        /// Add a file to favorites
        /// </summary>
        public void AddFavoriteFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            var favorites = FavoriteFiles;

            if (!favorites.Contains(filePath))
            {
                favorites.Add(filePath);
                Save();
            }
        }

        /// <summary>
        /// Remove a file from favorites
        /// </summary>
        public void RemoveFavoriteFile(string filePath)
        {
            var favorites = FavoriteFiles;

            if (favorites.Contains(filePath))
            {
                favorites.Remove(filePath);
                Save();
            }
        }

        /// <summary>
        /// Clear all recent files
        /// </summary>
        public void ClearRecentFiles()
        {
            RecentFiles.Clear();
            Save();
        }

        /// <summary>
        /// Reset all settings to default values
        /// </summary>
        public void ResetToDefaults()
        {
            Reset();
            Save();
        }

        #endregion

    }
}
