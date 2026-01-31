using CodeGenerator.Core.Settings.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Settings.Interfaces
{
    public interface ISettingsManager
    {
        event EventHandler? SettingsSaved;
        /// <summary>
        /// Load settings from <settingscope>.config file
        /// Settings are automatically loaded on first access, but this can be called explicitly
        /// </summary>
        void LoadSettings();

        /// <summary>
        /// Save all settings to <settingscope>.config file
        /// </summary>
        void SaveSettings();

        /// <summary>
        /// Reset all settings to their default values
        /// </summary>
        void ResetSettings();

        /// <summary>
        /// Get the path to the <settingscope>.config file where settings are stored
        /// </summary>
        string GetSettingsFilePath();

        SettingSection GetSettingsViewModelSection();
    }
}
