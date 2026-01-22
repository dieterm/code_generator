using CodeGenerator.Core.Settings.Application;
using CodeGenerator.Core.Settings.Interfaces;
using CodeGenerator.Core.Settings.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Settings
{
    public class WorkspaceSettingsManager : ISettingsManager
    {
        private readonly WorkspaceSettings _settings;
        private readonly WorkspaceSettingsViewModelGenerator _viewModelGenerator;
        public WorkspaceSettingsManager()
        {
            _settings = WorkspaceSettings.Instance;
            _viewModelGenerator = new WorkspaceSettingsViewModelGenerator(_settings);
        }

        public WorkspaceSettings Settings { get { return _settings; } }


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

        public string GetSettingsFilePath()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            return config.FilePath;
        }

        public SettingSection GetSettingsViewModelSection()
        {
            return _viewModelGenerator.GetSection();
        }

        #endregion
    }
}
