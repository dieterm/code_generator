using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CodeGenerator.WinForms.ViewModels;

/// <summary>
/// Base class for all ViewModels implementing INotifyPropertyChanged
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// Validation errors for the ViewModel
    /// </summary>
    public Dictionary<string, string> ValidationErrors { get; } = new();

    /// <summary>
    /// Check if ViewModel is valid
    /// </summary>
    public virtual bool IsValid => ValidationErrors.Count == 0;

    /// <summary>
    /// Validate the ViewModel
    /// </summary>
    public abstract bool Validate();

    /// <summary>
    /// Clear all validation errors
    /// </summary>
    protected void ClearValidationErrors()
    {
        ValidationErrors.Clear();
        OnPropertyChanged(nameof(IsValid));
    }

    /// <summary>
    /// Add a validation error
    /// </summary>
    protected void AddValidationError(string propertyName, string error)
    {
        ValidationErrors[propertyName] = error;
        OnPropertyChanged(nameof(IsValid));
    }
}
