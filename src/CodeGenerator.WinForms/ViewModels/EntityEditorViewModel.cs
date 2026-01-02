using CodeGenerator.Core.Models.Schema;

namespace CodeGenerator.WinForms.ViewModels;

/// <summary>
/// ViewModel for EntityEditorForm
/// </summary>
public class EntityEditorViewModel : ViewModelBase
{
    private string _entityName = string.Empty;
    private string _title = string.Empty;
    private string _description = string.Empty;
    private string _tableName = string.Empty;
    private string _schema = "dbo";
    private string _baseClass = string.Empty;
    private bool _isAbstract;
    private bool _isSealed;
    private bool _isOwnedType;
    private bool _generateRepository = true;
    private bool _generateController = true;
    private bool _generateViewModel = true;
    private bool _generateView = true;
    private bool _isValueObject;
    private bool _isAggregateRoot;
    private bool _isHierarchical;

    public bool IsEditMode { get; }

    public string EntityName
    {
        get => _entityName;
        set => SetProperty(ref _entityName, value);
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public string TableName
    {
        get => _tableName;
        set => SetProperty(ref _tableName, value);
    }

    public string Schema
    {
        get => _schema;
        set => SetProperty(ref _schema, value);
    }

    public string BaseClass
    {
        get => _baseClass;
        set => SetProperty(ref _baseClass, value);
    }

    public bool IsAbstract
    {
        get => _isAbstract;
        set => SetProperty(ref _isAbstract, value);
    }

    public bool IsSealed
    {
        get => _isSealed;
        set => SetProperty(ref _isSealed, value);
    }

    public bool IsOwnedType
    {
        get => _isOwnedType;
        set => SetProperty(ref _isOwnedType, value);
    }

    public bool GenerateRepository
    {
        get => _generateRepository;
        set => SetProperty(ref _generateRepository, value);
    }

    public bool GenerateController
    {
        get => _generateController;
        set => SetProperty(ref _generateController, value);
    }

    public bool GenerateViewModel
    {
        get => _generateViewModel;
        set => SetProperty(ref _generateViewModel, value);
    }

    public bool GenerateView
    {
        get => _generateView;
        set => SetProperty(ref _generateView, value);
    }

    public bool IsValueObject
    {
        get => _isValueObject;
        set => SetProperty(ref _isValueObject, value);
    }

    public bool IsAggregateRoot
    {
        get => _isAggregateRoot;
        set => SetProperty(ref _isAggregateRoot, value);
    }

    public bool IsHierarchical
    {
        get => _isHierarchical;
        set => SetProperty(ref _isHierarchical, value);
    }

    /// <summary>
    /// Original entity properties (preserved during edit)
    /// </summary>
    public Dictionary<string, PropertyDefinition>? OriginalProperties { get; private set; }
    public List<string>? OriginalRequired { get; private set; }

    public EntityEditorViewModel(string? existingName = null, EntityDefinition? existingEntity = null)
    {
        IsEditMode = existingEntity != null;

        if (existingName != null)
        {
            EntityName = existingName;
        }

        if (existingEntity != null)
        {
            LoadFromEntity(existingEntity);
        }
    }

    private void LoadFromEntity(EntityDefinition entity)
    {
        Title = entity.Title ?? string.Empty;
        Description = entity.Description ?? string.Empty;
        TableName = entity.DatabaseMetadata?.TableName ?? string.Empty;
        Schema = entity.DatabaseMetadata?.Schema ?? "dbo";
        BaseClass = entity.CodeGenMetadata?.BaseClass ?? string.Empty;
        IsAbstract = entity.CodeGenMetadata?.IsAbstract ?? false;
        IsSealed = entity.CodeGenMetadata?.IsSealed ?? false;
        IsOwnedType = entity.CodeGenMetadata?.IsOwnedType ?? false;
        GenerateRepository = entity.CodeGenMetadata?.GenerateRepository ?? true;
        GenerateController = entity.CodeGenMetadata?.GenerateController ?? true;
        GenerateViewModel = entity.CodeGenMetadata?.GenerateViewModel ?? true;
        GenerateView = entity.CodeGenMetadata?.GenerateView ?? true;
        IsValueObject = entity.DomainDrivenDesignMetadata?.ValueObject ?? false;
        IsAggregateRoot = entity.DomainDrivenDesignMetadata?.AggregateRoot ?? false;
        IsHierarchical = entity.DomainDrivenDesignMetadata?.Hierarchical ?? false;

        // Preserve original properties
        OriginalProperties = entity.Properties;
        OriginalRequired = entity.Required;
    }

    public EntityDefinition ToEntityDefinition()
    {
        return new EntityDefinition
        {
            Type = "object",
            Title = Title,
            Description = Description,
            Properties = OriginalProperties ?? new Dictionary<string, PropertyDefinition>(),
            Required = OriginalRequired,
            DatabaseMetadata = new EntityDatabaseMetadata
            {
                TableName = string.IsNullOrEmpty(TableName) ? EntityName : TableName,
                Schema = Schema
            },
            CodeGenMetadata = new EntityCodeGenMetadata
            {
                ClassName = EntityName,
                BaseClass = string.IsNullOrEmpty(BaseClass) ? null : BaseClass,
                IsAbstract = IsAbstract,
                IsSealed = IsSealed,
                IsOwnedType = IsOwnedType,
                GenerateRepository = GenerateRepository,
                GenerateController = GenerateController,
                GenerateViewModel = GenerateViewModel,
                GenerateView = GenerateView
            },
            DomainDrivenDesignMetadata = new EntityDomainDrivenDesignMetadata
            {
                ValueObject = IsValueObject,
                AggregateRoot = IsAggregateRoot,
                Hierarchical = IsHierarchical
            }
        };
    }

    public override bool Validate()
    {
        ClearValidationErrors();

        if (string.IsNullOrWhiteSpace(EntityName))
        {
            AddValidationError(nameof(EntityName), "Entity name is required.");
        }

        return IsValid;
    }
}
