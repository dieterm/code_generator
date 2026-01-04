using CodeGenerator.Core.Models.Schema;
using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.WinForms.ViewModels;

public class EntityDefinitionEditViewModel : ValidationViewModelBase
{
    private EntityDefinition? _entity;
    private string _title = string.Empty;
    private string _key = string.Empty;
    private string _description = string.Empty;
    private bool _isValueObject;
    private bool _isAggregateRoot;
    private bool _isHierarchical;

    //public EntityDefinitionEditViewModel()
    //{
    //}

    //public EntityDefinitionEditViewModel(EntityDefinition entity)
    //{
    //    LoadFromEntity(entity);
    //}
    public string Key
    {
        get => _key;
        set => SetProperty(ref _key, value);
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

    public void LoadFromEntity(EntityDefinition entity)
    {
        _entity = entity;
        Key = entity.Key ?? string.Empty;
        Title = entity.Title ?? string.Empty;
        Description = entity.Description ?? string.Empty;
        IsValueObject = entity.DomainDrivenDesignMetadata?.ValueObject ?? false;
        IsAggregateRoot = entity.DomainDrivenDesignMetadata?.AggregateRoot ?? false;
        IsHierarchical = entity.DomainDrivenDesignMetadata?.Hierarchical ?? false;
    }

    public void UpdateEntity()
    {
        if (_entity != null)
        {
            _entity.Title = string.IsNullOrWhiteSpace(Title) ? null : Title;
            _entity.Description = string.IsNullOrWhiteSpace(Description) ? null : Description;

            if (_entity.DomainDrivenDesignMetadata == null)
                _entity.DomainDrivenDesignMetadata = new EntityDomainDrivenDesignMetadata();

            _entity.DomainDrivenDesignMetadata.ValueObject = IsValueObject;
            _entity.DomainDrivenDesignMetadata.AggregateRoot = IsAggregateRoot;
            _entity.DomainDrivenDesignMetadata.Hierarchical = IsHierarchical;
        }
    }

    public override bool Validate()
    {
        ClearValidationErrors();
        return IsValid;
    }
}
