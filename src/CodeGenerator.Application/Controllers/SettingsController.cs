using CodeGenerator.Application.MessageBus;
using CodeGenerator.Application.Services;
using CodeGenerator.Core.Settings.Application;
using CodeGenerator.Core.Settings.Generators;
using CodeGenerator.Core.Settings.ViewModels;
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
        private readonly GeneratorSettingsManager _generatorSettingsManager;

		public SettingsController(ApplicationSettingsManager applicationSettingsManager, GeneratorSettingsManager generatorSettingsManager, SettingsViewModel settingsViewModel, IWindowManagerService windowManagerService, RibbonBuilder ribbonBuilder, IMessageBoxService messageBoxService, IFileSystemDialogService fileSystemDialogService, ApplicationMessageBus messageBus, ILogger<SettingsController> logger)
			: base(windowManagerService, ribbonBuilder, messageBus, messageBoxService, fileSystemDialogService, logger)
		{
			_settingsViewModel = settingsViewModel;
			_applicationSettingsManager = applicationSettingsManager;
			_generatorSettingsManager = generatorSettingsManager;
        }

        /// <summary>
        /// Initialize generator settings by discovering all registered generators
        /// </summary>
        public override void Initialize()
		{
			_logger.LogInformation("SettingsController starting...");
			_applicationSettingsManager.LoadSettings();
            _generatorSettingsManager.LoadSettings();
            _generatorSettingsManager.DiscoverAndRegisterGenerators();
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
			_settingsViewModel.SettingsSections.Add(_generatorSettingsManager.GetSettingsViewModelSection());
            _logger.LogInformation("Settings loaded.");
        }

		public void SaveSettings()
		{
            _applicationSettingsManager.SaveSettings();
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