using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Models.Configuration;

namespace CodeGenerator.WinForms.Controllers;

/// <summary>
/// Controller for SettingsForm
/// </summary>
public class SettingsController : ControllerBase<ViewModels.SettingsViewModel>
{
    private readonly ISettingsService _settingsService;
    private const string ConfigFileName = "AppConfig.json";

    public SettingsController(ViewModels.SettingsViewModel viewModel, ISettingsService settingsService) 
        : base(viewModel)
    {
        _settingsService = settingsService;
    }

    /// <summary>
    /// Save settings to file
    /// </summary>
    public async Task<bool> SaveAsync()
    {
        if (!Validate())
        {
            return false;
        }

        var settings = ViewModel.ToGeneratorSettings();
        var configPath = GetConfigFilePath();
        await _settingsService.SaveSettingsAsync(settings, configPath);
        return true;
    }

    /// <summary>
    /// Load default settings
    /// </summary>
    public void LoadDefaults()
    {
        var defaults = _settingsService.GetDefaultSettings();
        ViewModel.LoadFromSettings(defaults);
    }

    /// <summary>
    /// Get settings from ViewModel
    /// </summary>
    public GeneratorSettings GetSettings()
    {
        return ViewModel.ToGeneratorSettings();
    }

    /// <summary>
    /// Get the config file path
    /// </summary>
    public string GetConfigFilePath()
    {
        var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appDataFolder, "CodeGenerator");
        
        if (!Directory.Exists(appFolder))
        {
            Directory.CreateDirectory(appFolder);
        }
        
        return Path.Combine(appFolder, ConfigFileName);
    }

    /// <summary>
    /// Get the first validation error message
    /// </summary>
    public string? GetFirstValidationError()
    {
        return ViewModel.ValidationErrors.Values.FirstOrDefault();
    }
}
