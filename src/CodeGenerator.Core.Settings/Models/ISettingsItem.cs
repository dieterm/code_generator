using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.Core.Settings.Models
{
    public interface ISettingsItem
    {
        object? DefaultValue { get; set; }
        string? Description { get; }
        IFieldViewModel FieldViewModel { get; }
        string Key { get; }
        string Name { get; }
        object? Value { get; set; }
    }
}