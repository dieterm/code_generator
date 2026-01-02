using System.Text.Json;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Output;
using CodeGenerator.Core.Models.Schema;
using CodeGenerator.Generators;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.WinForms.Controllers;

/// <summary>
/// Controller for MainForm
/// </summary>
public class MainFormController : ControllerBase<ViewModels.MainFormViewModel>
{
    private readonly ISchemaParser _schemaParser;
    private readonly ISettingsService _settingsService;
    private readonly GeneratorOrchestrator _orchestrator;
    private readonly ILogger<MainFormController> _logger;
    private const string ConfigFileName = "AppConfig.json";

    public MainFormController(
        ViewModels.MainFormViewModel viewModel,
        ISchemaParser schemaParser,
        ISettingsService settingsService,
        GeneratorOrchestrator orchestrator,
        ILogger<MainFormController> logger) 
        : base(viewModel)
    {
        _schemaParser = schemaParser;
        _settingsService = settingsService;
        _orchestrator = orchestrator;
        _logger = logger;
    }

    public override void Initialize()
    {
        base.Initialize();
        _ = LoadSettingsAsync();
    }

    /// <summary>
    /// Load settings from file
    /// </summary>
    public async Task LoadSettingsAsync()
    {
        try
        {
            var configPath = GetConfigFilePath();
            
            if (File.Exists(configPath))
            {
                ViewModel.Settings = await _settingsService.LoadSettingsAsync(configPath);
                Log($"Loaded settings from: {configPath}");
            }
            else
            {
                ViewModel.Settings = _settingsService.GetDefaultSettings();
                Log("Using default settings (no saved configuration found)");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load settings, using defaults");
            ViewModel.Settings = _settingsService.GetDefaultSettings();
            Log("Failed to load settings, using defaults");
        }
    }

    /// <summary>
    /// Create a new schema
    /// </summary>
    public void NewSchema()
    {
        ViewModel.CreateNewSchema();
        Log("Created new schema");
    }

    /// <summary>
    /// Load a schema from file
    /// </summary>
    public async Task<bool> LoadSchemaAsync(string filePath)
    {
        try
        {
            ViewModel.IsBusy = true;
            ViewModel.StatusMessage = "Loading schema...";

            var json = await File.ReadAllTextAsync(filePath);
            var schema = JsonSerializer.Deserialize<DomainSchema>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            });

            ViewModel.CurrentSchema = schema;
            ViewModel.CurrentContext = await _schemaParser.ParseAsync(filePath);
            ViewModel.CurrentFilePath = filePath;
            ViewModel.IsDirty = false;

            Log($"Loaded schema from: {filePath}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load schema");
            throw;
        }
        finally
        {
            ViewModel.IsBusy = false;
            ViewModel.StatusMessage = "Ready";
        }
    }

    /// <summary>
    /// Save the schema to file
    /// </summary>
    public async Task<bool> SaveSchemaAsync(string filePath)
    {
        try
        {
            ViewModel.IsBusy = true;
            ViewModel.StatusMessage = "Saving schema...";

            var json = JsonSerializer.Serialize(ViewModel.CurrentSchema, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(filePath, json);

            ViewModel.CurrentFilePath = filePath;
            ViewModel.IsDirty = false;

            Log($"Saved schema to: {filePath}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save schema");
            throw;
        }
        finally
        {
            ViewModel.IsBusy = false;
            ViewModel.StatusMessage = "Ready";
        }
    }

    /// <summary>
    /// Generate preview for all generators
    /// </summary>
    public async Task<GenerationPreview> PreviewAllAsync()
    {
        if (ViewModel.CurrentFilePath == null)
            throw new InvalidOperationException("Please save the schema first.");

        try
        {
            ViewModel.IsBusy = true;
            ViewModel.StatusMessage = "Generating preview...";
            ViewModel.Settings.SchemaFilePath = ViewModel.CurrentFilePath;

            var preview = await _orchestrator.PreviewAllAsync(ViewModel.Settings);
            ViewModel.CurrentGenerationPreview = preview;

            Log($"Preview generated: {preview.TotalFiles} files, {preview.TotalProjects} projects");
            return preview;
        }
        finally
        {
            ViewModel.IsBusy = false;
            ViewModel.StatusMessage = "Ready";
        }
    }

    /// <summary>
    /// Generate code for all generators
    /// </summary>
    public async Task<GenerationResult> GenerateAllAsync()
    {
        if (ViewModel.CurrentFilePath == null)
            throw new InvalidOperationException("Please save the schema first.");

        try
        {
            ViewModel.IsBusy = true;
            ViewModel.StatusMessage = "Generating code...";
            ViewModel.Settings.SchemaFilePath = ViewModel.CurrentFilePath;

            var result = await _orchestrator.GenerateAllAsync(ViewModel.Settings);

            foreach (var msg in result.Messages) Log(msg);
            foreach (var warn in result.Warnings) Log($"Warning: {warn}");
            foreach (var err in result.Errors) Log($"Error: {err}");

            if (result.Success)
            {
                Log($"Generation completed successfully. {result.Files.Count} files generated.");
            }

            return result;
        }
        finally
        {
            ViewModel.IsBusy = false;
            ViewModel.StatusMessage = "Ready";
        }
    }

    /// <summary>
    /// Generate code for a specific generator
    /// </summary>
    public async Task<GenerationResult> GenerateAsync(string generatorId)
    {
        if (ViewModel.CurrentFilePath == null)
            throw new InvalidOperationException("Please save the schema first.");

        try
        {
            ViewModel.IsBusy = true;
            ViewModel.StatusMessage = "Generating code...";
            ViewModel.Settings.SchemaFilePath = ViewModel.CurrentFilePath;

            return await _orchestrator.GenerateAsync(generatorId, ViewModel.Settings);
        }
        finally
        {
            ViewModel.IsBusy = false;
            ViewModel.StatusMessage = "Ready";
        }
    }

    /// <summary>
    /// Add an entity
    /// </summary>
    public void AddEntity(string entityName, EntityDefinition entity)
    {
        ViewModel.AddEntity(entityName, entity);
        Log($"Added entity: {entityName}");
    }

    /// <summary>
    /// Add a property to an entity
    /// </summary>
    public void AddProperty(EntityDefinition entity, string propertyName, PropertyDefinition property)
    {
        ViewModel.AddProperty(entity, propertyName, property);
        Log($"Added property: {propertyName}");
    }

    /// <summary>
    /// Delete an entity
    /// </summary>
    public bool DeleteEntity(string entityName)
    {
        var result = ViewModel.RemoveEntity(entityName);
        if (result) Log($"Deleted entity: {entityName}");
        return result;
    }

    /// <summary>
    /// Delete a property
    /// </summary>
    public bool DeleteProperty(EntityDefinition entity, string propertyName)
    {
        var result = ViewModel.RemoveProperty(entity, propertyName);
        if (result) Log($"Deleted property: {propertyName}");
        return result;
    }

    /// <summary>
    /// Update settings
    /// </summary>
    public void UpdateSettings(GeneratorSettings settings)
    {
        ViewModel.Settings = settings;
        Log("Settings updated");
    }

    private string GetConfigFilePath()
    {
        var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appDataFolder, "CodeGenerator");
        
        if (!Directory.Exists(appFolder))
        {
            Directory.CreateDirectory(appFolder);
        }
        
        return Path.Combine(appFolder, ConfigFileName);
    }

    private readonly List<string> _logMessages = new();
    public event Action<string>? LogMessageAdded;

    private void Log(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        var logEntry = $"[{timestamp}] {message}";
        _logMessages.Add(logEntry);
        LogMessageAdded?.Invoke(logEntry);
        _logger.LogInformation(message);
    }
}
