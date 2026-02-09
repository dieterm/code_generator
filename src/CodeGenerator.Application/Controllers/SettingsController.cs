using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Services;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Core.Settings.Application;
using CodeGenerator.Core.Settings.Generators;
using CodeGenerator.Core.Settings.ViewModels;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Templates.Settings;
using CodeGenerator.Core.Workspaces.Settings;
using CodeGenerator.Shared.Operations;
using CodeGenerator.Shared.Ribbon;
using Microsoft.Extensions.Logging;
using System;
using System.Configuration;

namespace CodeGenerator.Application.Controllers
{
	public class SettingsController : CoreControllerBase
	{
        private readonly SettingsViewModel _settingsViewModel;
        private readonly ApplicationSettingsManager _applicationSettingsManager;
		private readonly WorkspaceSettingsManager _workspaceSettingsManager;
        private readonly GeneratorSettingsManager _generatorSettingsManager;
        private readonly TemplateManager _templateManager;
        private readonly TemplateEngineSettingsManager _templateEngineSettingsManager;

		public SettingsController(OperationExecutor operationExecutor, TemplateEngineSettingsManager templateEngineSettingsManager, TemplateManager templateManager, ApplicationSettingsManager applicationSettingsManager, WorkspaceSettingsManager workspaceSettingsManager, GeneratorSettingsManager generatorSettingsManager, SettingsViewModel settingsViewModel, IWindowManagerService windowManagerService, RibbonBuilder ribbonBuilder, IMessageBoxService messageBoxService, IFileSystemDialogService fileSystemDialogService, ApplicationMessageBus messageBus, ILogger<SettingsController> logger)
			: base(operationExecutor, windowManagerService, ribbonBuilder, messageBus, messageBoxService, fileSystemDialogService, logger)
		{
			_templateEngineSettingsManager = templateEngineSettingsManager;
			_settingsViewModel = settingsViewModel;
			_applicationSettingsManager = applicationSettingsManager;
			_workspaceSettingsManager = workspaceSettingsManager;
			_generatorSettingsManager = generatorSettingsManager;
            _templateManager = templateManager;
        }
		private string? WorkspaceSettingsDefaultTemplateFolder { get; set; }
        /// <summary>
        /// Initialize generator settings by discovering all registered generators
        /// </summary>
        public override void Initialize()
		{
			_logger.LogInformation("SettingsController starting...");
			_applicationSettingsManager.LoadSettings();
            _workspaceSettingsManager.LoadSettings();
            _templateEngineSettingsManager.LoadSettings();
            _templateEngineSettingsManager.DiscoverAndRegisterTemplateEngines();
            _generatorSettingsManager.LoadSettings();
            _generatorSettingsManager.DiscoverAndRegisterGenerators();


            if (Directory.Exists(_workspaceSettingsManager.Settings.DefaultTemplateFolder)) {
                WorkspaceSettingsDefaultTemplateFolder = _workspaceSettingsManager.Settings.DefaultTemplateFolder;
                _templateManager.RegisterTemplateFolder(_workspaceSettingsManager.Settings.DefaultTemplateFolder);
            }
            _workspaceSettingsManager.Settings.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == nameof(WorkspaceSettings.DefaultTemplateFolder))
				{
					var oldFolder = WorkspaceSettingsDefaultTemplateFolder;
					if (!string.IsNullOrEmpty(oldFolder))
					{
						_templateManager.UnregisterTemplateFolder(oldFolder);
                    }

                    var newFolder = _workspaceSettingsManager.Settings.DefaultTemplateFolder;
					if (Directory.Exists(newFolder))
					{
						_templateManager.RegisterTemplateFolder(newFolder);
					}
					else
					{
						_logger.LogWarning("Default template folder does not exist: {Folder}", newFolder);
					}
				}
			};
            _settingsViewModel.OkRequested += OnSaveSettings;
			_settingsViewModel.CancelRequested += OnLoadSettings;
        }

        private void OnLoadSettings(object? sender, EventArgs e)
        {
            // Reload settings to discard changes
            LoadSettings();
        }

        private void OnSaveSettings(object? sender, EventArgs e)
        {
            SaveSettings();
        }

        public void ShowSettings()
		{
			LoadSettings();

            _windowManagerService.ShowSettingsWindow(_settingsViewModel);
		}

		public void LoadSettings()
		{
			_logger.LogInformation("Loading settings...");
			_settingsViewModel.SettingsSections.Clear();
            _settingsViewModel.SettingsSections.Add(_applicationSettingsManager.GetSettingsViewModelSection());
            _settingsViewModel.SettingsSections.Add(_workspaceSettingsManager.GetSettingsViewModelSection());
            _settingsViewModel.SettingsSections.Add(_templateEngineSettingsManager.GetSettingsViewModelSection());
            _settingsViewModel.SettingsSections.Add(_generatorSettingsManager.GetSettingsViewModelSection());
            _logger.LogInformation("Settings loaded.");
        }

		public void SaveSettings()
		{
            _applicationSettingsManager.SaveSettings();
			_workspaceSettingsManager.SaveSettings();
			_templateEngineSettingsManager.SaveSettings();
            _generatorSettingsManager.SaveSettings();
        }

        public void CreateRibbon()
		{
			// Build Ribbon Model
			_ribbonBuilder
				.AddTab("tabSettings", "Settings")
					.AddToolStrip("toolstripSettings", "Settings")
						.AddButton("btnShowSettings", "ShowSettings")
							.WithSize(RibbonButtonSize.Large)
							.WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
							.WithImage("settings")
							.OnClick((e) => ShowSettings()).Build();
		}

        public override void Dispose()
		{
			_logger.LogInformation("SettingsController disposing...");
		}
	}
}