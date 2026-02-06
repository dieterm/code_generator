using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.Windows.Input;

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

    public TemplateExecutionViewModel()
    {
        ExecuteCommand = new RelayCommand(() => ExecuteRequested?.Invoke(this, EventArgs.Empty), () => CanExecute && !IsExecuting);
        EditTemplateCommand = new RelayCommand(() => EditTemplateRequested?.Invoke(this, EventArgs.Empty), () => IsScribanTemplate && !string.IsNullOrEmpty(TemplateFilePath));
        SetDefaultsCommand = new RelayCommand(() => SetDefaultsRequested?.Invoke(this, EventArgs.Empty), () => ParameterFields.Any() && !IsExecuting);
    }

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
                SetDefaultsCommand.RaiseCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// Whether the template can be executed (all required parameters are set)
    /// </summary>
    public bool CanExecute
    {
        get => _canExecute;
        set
        {
            if (SetProperty(ref _canExecute, value))
            {
                ExecuteCommand.RaiseCanExecuteChanged();
            }
        }
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
                ExecuteCommand.RaiseCanExecuteChanged();
                SetDefaultsCommand.RaiseCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// Whether the current template is a Scriban template (determines if Edit Template button is visible)
    /// </summary>
    public bool IsScribanTemplate
    {
        get => _isScribanTemplate;
        set 
        { 
            if(SetProperty(ref _isScribanTemplate, value))
            {
                EditTemplateCommand.RaiseCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// Path to the template file (used for editing)
    /// </summary>
    public string? TemplateFilePath
    {
        get => _templateFilePath;
        set 
        { 
            if(SetProperty(ref _templateFilePath, value))
            {
                EditTemplateCommand.RaiseCanExecuteChanged();
            }
        }
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
    /// Event raised when the user requests to set default values for parameters
    /// </summary>
    public event EventHandler? SetDefaultsRequested;
    
    /// <summary>
    /// Raises the ExecuteRequested event
    /// </summary>
    public RelayCommand ExecuteCommand { get; }
    
    /// <summary>
    /// Raises the EditTemplateRequested event
    /// </summary>
    public RelayCommand EditTemplateCommand { get; }

    /// <summary>
    /// Command to set default values for all parameters
    /// </summary>
    public RelayCommand SetDefaultsCommand { get; }

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
