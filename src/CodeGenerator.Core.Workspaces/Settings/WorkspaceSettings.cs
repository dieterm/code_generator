using CodeGenerator.Core.Settings.Application;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Settings
{
    /// <summary>
    /// Workspace-specific settings using System.Configuration.ApplicationSettingsBase
    /// Settings are automatically persisted to user.config file
    /// </summary>
    public sealed class WorkspaceSettings : ApplicationSettingsBase
    {
        // Singleton pattern for easy access
        private static WorkspaceSettings? _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Event raised when the default template folder changes.
        /// Listeners (like TemplateManager) can subscribe to handle the change.
        /// </summary>
        public static event EventHandler<string?>? DefaultTemplateFolderChanged;

        public static WorkspaceSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new WorkspaceSettings();
                        }
                    }
                }
                return _instance;
            }
        }

        #region Workspace Identity

        /// <summary>
        /// Root namespace for generated code (used in folder names and code namespaces)<br/>
        /// Default is "MyCompany.MyProduct"
        /// </summary>
        [UserScopedSetting]
        [DefaultSettingValue("MyCompany.MyProduct")]
        public string RootNamespace
        {
            get { return (string)this[nameof(RootNamespace)]; }
            set { this[nameof(RootNamespace)] = value; }
        }

        /// <summary>
        /// Default output directory for generated code
        /// </summary>
        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string DefaultOutputDirectory
        {
            get { return (string)this[nameof(DefaultOutputDirectory)]; }
            set { this[nameof(DefaultOutputDirectory)] = value; }
        }

        #endregion

        #region Code Generation Preferences

        /// <summary>
        /// Default target framework for new projects
        /// </summary>
        [UserScopedSetting]
        [DefaultSettingValue("net8.0")]
        public string DefaultTargetFramework
        {
            get { return (string)this[nameof(DefaultTargetFramework)]; }
            set { this[nameof(DefaultTargetFramework)] = value; }
        }

        /// <summary>
        /// Default programming language
        /// </summary>
        [UserScopedSetting]
        [DefaultSettingValue("C#")]
        public string DefaultLanguage
        {
            get { return (string)this[nameof(DefaultLanguage)]; }
            set { this[nameof(DefaultLanguage)] = value; }
        }

        /// <summary>
        /// Use nullable reference types by default
        /// </summary>
        [UserScopedSetting]
        [DefaultSettingValue("true")]
        public bool UseNullableReferenceTypes
        {
            get { return (bool)this[nameof(UseNullableReferenceTypes)]; }
            set { this[nameof(UseNullableReferenceTypes)] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("onion")]
        public string DefaultCodeArchitectureId
        {
            get { return (string)this[nameof(DefaultCodeArchitectureId)]; }
            set { this[nameof(DefaultCodeArchitectureId)] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string DefaultTemplateFolder
        {
            get { return (string)this[nameof(DefaultTemplateFolder)]; }
            set 
            { 
                var oldValue = (string)this[nameof(DefaultTemplateFolder)];
                this[nameof(DefaultTemplateFolder)] = value;
                
                // Notify listeners when the value changes
                if (oldValue != value)
                {
                    DefaultTemplateFolderChanged?.Invoke(this, value);
                }
            }
        }

        #endregion

        #region Helper Methods
        /// <summary>
        /// Reset all settings to default values
        /// </summary>
        public void ResetToDefaults()
        {
            Reset();
            Save();
        }

        public CodeArchitecture GetDefaultCodeArchitecture()
        {
            var manager = ServiceProviderHolder.GetRequiredService<CodeArchitectureManager>();
            var architecture = manager.GetById(DefaultCodeArchitectureId);
            if (architecture == null)
            {
                throw new InvalidOperationException($"Default code architecture with ID '{DefaultCodeArchitectureId}' not found in registry.");
            }
            return architecture;
        }
        #endregion
    }
}
