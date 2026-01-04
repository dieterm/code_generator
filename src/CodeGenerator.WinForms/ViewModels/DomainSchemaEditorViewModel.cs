using CodeGenerator.Core.Models.Schema;
using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.WinForms.ViewModels;

public class DomainSchemaEditorViewModel : ValidationViewModelBase
{
    private DomainSchema? _schema;
    private object? _selectedItem;

    public DomainSchema? Schema
    {
        get => _schema;
        set
        {
            if (SetProperty(ref _schema, value))
            {
                OnPropertyChanged(nameof(HasSchema));
            }
        }
    }

    public object? SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }

    public bool HasSchema => Schema != null;

    public event EventHandler? SchemaChanged;
    public event EventHandler<EntityDefinition>? EntityAdded;
    public event EventHandler<(EntityDefinition Entity, PropertyDefinition Property)>? PropertyAdded;
    public event EventHandler<string>? ItemDeleted;

    public void RefreshSchema()
    {
        SchemaChanged?.Invoke(this, EventArgs.Empty);
    }

    public void NotifyEntityAdded(EntityDefinition entity)
    {
        EntityAdded?.Invoke(this, entity);
        SchemaChanged?.Invoke(this, EventArgs.Empty);
    }

    public void NotifyPropertyAdded(EntityDefinition entity, PropertyDefinition property)
    {
        PropertyAdded?.Invoke(this, (entity, property));
        SchemaChanged?.Invoke(this, EventArgs.Empty);
    }

    public void NotifyItemDeleted(string itemName)
    {
        ItemDeleted?.Invoke(this, itemName);
        SchemaChanged?.Invoke(this, EventArgs.Empty);
    }

    public override bool Validate()
    {
        ClearValidationErrors();
        return IsValid;
    }
}
