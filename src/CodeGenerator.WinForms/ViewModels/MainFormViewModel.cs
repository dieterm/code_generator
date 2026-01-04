using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;
using CodeGenerator.Core.Models.Schema;
using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.WinForms.ViewModels;

/// <summary>
/// ViewModel for MainForm
/// </summary>
public class MainFormViewModel : ValidationViewModelBase
{
    private DomainSchema? _currentSchema;
    private DomainContext? _currentContext;
    private GeneratorSettings _settings = new();
    private GenerationPreview? _currentGenerationPreview;
    private string? _currentFilePath;
    private bool _isDirty;
    private string _statusMessage = "Ready";
    private bool _isBusy;

    public DomainSchema? CurrentSchema
    {
        get => _currentSchema;
        set => SetProperty(ref _currentSchema, value);
    }

    public DomainContext? CurrentContext
    {
        get => _currentContext;
        set => SetProperty(ref _currentContext, value);
    }

    public GeneratorSettings Settings
    {
        get => _settings;
        set => SetProperty(ref _settings, value);
    }

    public GenerationPreview? CurrentGenerationPreview
    {
        get => _currentGenerationPreview;
        set => SetProperty(ref _currentGenerationPreview, value);
    }

    public string? CurrentFilePath
    {
        get => _currentFilePath;
        set
        {
            if (SetProperty(ref _currentFilePath, value))
            {
                OnPropertyChanged(nameof(WindowTitle));
                OnPropertyChanged(nameof(FileName));
            }
        }
    }

    public bool IsDirty
    {
        get => _isDirty;
        set
        {
            if (SetProperty(ref _isDirty, value))
            {
                OnPropertyChanged(nameof(WindowTitle));
            }
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public string FileName => string.IsNullOrEmpty(CurrentFilePath) ? "Untitled" : Path.GetFileName(CurrentFilePath);

    public string WindowTitle => $"Code Generator - {FileName}{(IsDirty ? " *" : "")}";

    public bool HasSchema => CurrentSchema != null;

    public bool HasFilePath => !string.IsNullOrEmpty(CurrentFilePath);

    /// <summary>
    /// Create a new empty schema
    /// </summary>
    public void CreateNewSchema()
    {
        CurrentSchema = new DomainSchema
        {
            Schema = "https://json-schema.org/draft/2020-12/schema",
            Title = "New Domain Schema",
            Description = "Domain-driven design schema for code generation",
            Definitions = new Dictionary<string, EntityDefinition>(),
            CodeGenMetadata = new CodeGenMetadata
            {
                Namespace = "MyCompany.MyProject",
                TargetLanguage = "CSharp",
                DataLayerTechnology = "EntityFrameworkCore"
            },
            DatabaseMetadata = new DatabaseMetadata
            {
                DatabaseName = "MyDatabase",
                Schema = "dbo",
                Provider = "SqlServer"
            }
        };

        CurrentFilePath = null;
        IsDirty = false;
    }

    /// <summary>
    /// Add an entity to the schema
    /// </summary>
    public void AddEntity(string entityName, EntityDefinition entity)
    {
        if (CurrentSchema == null) return;

        CurrentSchema.Definitions ??= new Dictionary<string, EntityDefinition>();
        CurrentSchema.Definitions[entityName] = entity;
        IsDirty = true;
    }

    /// <summary>
    /// Remove an entity from the schema
    /// </summary>
    public bool RemoveEntity(string entityName)
    {
        if (CurrentSchema?.Definitions == null) return false;

        var result = CurrentSchema.Definitions.Remove(entityName);
        if (result) IsDirty = true;
        return result;
    }

    /// <summary>
    /// Add a property to an entity
    /// </summary>
    public void AddProperty(EntityDefinition entity, string propertyName, PropertyDefinition property)
    {
        entity.Properties ??= new Dictionary<string, PropertyDefinition>();
        entity.Properties[propertyName] = property;
        IsDirty = true;
    }

    /// <summary>
    /// Remove a property from an entity
    /// </summary>
    public bool RemoveProperty(EntityDefinition entity, string propertyName)
    {
        if (entity.Properties == null) return false;

        var result = entity.Properties.Remove(propertyName);
        if (result) IsDirty = true;
        return result;
    }

    public override bool Validate()
    {
        ClearValidationErrors();
        return IsValid;
    }
}
