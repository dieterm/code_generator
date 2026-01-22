using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;

namespace CodeGenerator.Application.ViewModels.Template;

/// <summary>
/// ViewModel for template execution view - displays parameter input fields and execute button
/// </summary>
public class TemplateExecutionViewModel : ViewModelBase
{
    private List<FieldViewModelBase> _parameterFields = new();
    private bool _canExecute;
    private bool _isExecuting;
    private bool _isScribanTemplate;
    private string? _templateFilePath;

    /// <summary>
    /// List of parameter field view models for the UI
    /// </summary>
    public List<FieldViewModelBase> ParameterFields
    {
        get => _parameterFields;
        set
        {
            if (SetProperty(ref _parameterFields, value))
            {
                SubscribeToFieldChanges();
                ValidateParameters();
            }
        }
    }

    /// <summary>
    /// Whether the template can be executed (all required parameters are set)
    /// </summary>
    public bool CanExecute
    {
        get => _canExecute;
        set => SetProperty(ref _canExecute, value);
    }

    /// <summary>
    /// Whether the template is currently being executed
    /// </summary>
    public bool IsExecuting
    {
        get => _isExecuting;
        set
        {
            if (SetProperty(ref _isExecuting, value))
            {
                OnPropertyChanged(nameof(CanExecute));
            }
        }
    }

    /// <summary>
    /// Whether the current template is a Scriban template (determines if Edit Template button is visible)
    /// </summary>
    public bool IsScribanTemplate
    {
        get => _isScribanTemplate;
        set => SetProperty(ref _isScribanTemplate, value);
    }

    /// <summary>
    /// Path to the template file (used for editing)
    /// </summary>
    public string? TemplateFilePath
    {
        get => _templateFilePath;
        set => SetProperty(ref _templateFilePath, value);
    }

    /// <summary>
    /// Event raised when the user requests to execute the template
    /// </summary>
    public event EventHandler? ExecuteRequested;

    /// <summary>
    /// Event raised when the user requests to edit the template
    /// </summary>
    public event EventHandler? EditTemplateRequested;

    /// <summary>
    /// Raises the ExecuteRequested event
    /// </summary>
    public void RequestExecute()
    {
        if (CanExecute && !IsExecuting)
        {
            ExecuteRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Raises the EditTemplateRequested event
    /// </summary>
    public void RequestEditTemplate()
    {
        if (IsScribanTemplate && !string.IsNullOrEmpty(TemplateFilePath))
        {
            EditTemplateRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Subscribe to property changes on all fields to validate when values change
    /// </summary>
    private void SubscribeToFieldChanges()
    {
        foreach (var field in _parameterFields)
        {
            field.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(FieldViewModelBase.Value))
                {
                    ValidateParameters();
                }
            };
        }
    }

    /// <summary>
    /// Validate that all required parameters have values
    /// </summary>
    public void ValidateParameters()
    {
        foreach (var field in ParameterFields)
        {
            if (field.IsRequired)
            {
                if (field is ComboboxFieldModel comboField)
                {
                    if (comboField.SelectedItem == null)
                    {
                        CanExecute = false;
                        return;
                    }
                }
                else if (field.Value == null || string.IsNullOrEmpty(field.Value?.ToString()))
                {
                    CanExecute = false;
                    return;
                }
            }
        }

        CanExecute = true;
    }

    /// <summary>
    /// Get parameter values as a dictionary for template execution
    /// </summary>
    public Dictionary<string, object?> GetParameterValues()
    {
        var values = new Dictionary<string, object?>();

        foreach (var field in ParameterFields)
        {
            if (field is ComboboxFieldModel comboField)
            {
                if (comboField.SelectedItem is TemplateDatasourceArtifactItem tableItem)
                {
                    values[field.Name] = tableItem;
                    continue;
                }
            }
            values[field.Name] = field.Value;
        }

        return values;
    }
}
