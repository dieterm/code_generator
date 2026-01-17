using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.Collections.ObjectModel;

namespace CodeGenerator.Application.ViewModels.Template;

/// <summary>
/// ViewModel for the template parameters view, providing input fields for template parameters
/// </summary>
public class TemplateParametersViewModel : ViewModelBase
{
    private TemplateArtifact? _templateArtifact;
    private string _templateName = string.Empty;
    private string? _templateDescription;
    private bool _isEditMode;
    private WorkspaceTreeViewController? _workspaceController;
    private TemplateParametersEditViewModel _editViewModel = new();
    private TemplateExecutionViewModel _executionViewModel = new();

    public TemplateParametersViewModel()
    {
        // Subscribe to child ViewModel events
        _editViewModel.SaveRequested += (s, e) => SaveParameterDefinitions();
        _executionViewModel.ExecuteRequested += (s, e) => ExecuteRequested?.Invoke(this, EventArgs.Empty);
        _executionViewModel.EditTemplateRequested += (s, e) => EditTemplateRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Set the workspace controller for accessing workspace data (tables, etc.)
    /// </summary>
    public void SetWorkspaceController(WorkspaceTreeViewController workspaceController)
    {
        _workspaceController = workspaceController;
    }

    /// <summary>
    /// The template artifact being configured
    /// </summary>
    public TemplateArtifact? TemplateArtifact
    {
        get => _templateArtifact;
        set
        {
            if (SetProperty(ref _templateArtifact, value))
            {
                UpdateFromTemplate();
            }
        }
    }

    /// <summary>
    /// Display name of the template (read-only, for display in header)
    /// </summary>
    public string TemplateName
    {
        get => _templateName;
        set => SetProperty(ref _templateName, value);
    }

    /// <summary>
    /// Description of the template
    /// </summary>
    public string? TemplateDescription
    {
        get => _templateDescription;
        set => SetProperty(ref _templateDescription, value);
    }

    /// <summary>
    /// Whether we are in edit mode (editing parameter definitions) or execution mode
    /// </summary>
    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    /// <summary>
    /// ViewModel for the edit view (parameter definitions)
    /// </summary>
    public TemplateParametersEditViewModel EditViewModel
    {
        get => _editViewModel;
        set => SetProperty(ref _editViewModel, value);
    }

    /// <summary>
    /// ViewModel for the execution view (parameter input fields)
    /// </summary>
    public TemplateExecutionViewModel ExecutionViewModel
    {
        get => _executionViewModel;
        set => SetProperty(ref _executionViewModel, value);
    }

    /// <summary>
    /// Whether the template can be executed (delegates to ExecutionViewModel)
    /// </summary>
    public bool CanExecute => ExecutionViewModel.CanExecute;

    /// <summary>
    /// Whether the template is currently being executed
    /// </summary>
    public bool IsExecuting
    {
        get => ExecutionViewModel.IsExecuting;
        set => ExecutionViewModel.IsExecuting = value;
    }

    /// <summary>
    /// Whether there are unsaved changes (delegates to EditViewModel)
    /// </summary>
    public bool HasUnsavedChanges => EditViewModel.HasUnsavedChanges;

    /// <summary>
    /// Event raised when the user requests to execute the template
    /// </summary>
    public event EventHandler? ExecuteRequested;
    public event EventHandler EditTemplateRequested;

    /// <summary>
    /// Raises the ExecuteRequested event
    /// </summary>
    public void RequestExecute()
    {
        ExecutionViewModel.RequestExecute();
    }

    /// <summary>
    /// Toggle between edit mode and execution mode
    /// </summary>
    public void ToggleEditMode()
    {
        IsEditMode = !IsEditMode;
        if (!IsEditMode)
        {
            // Switching back to execution mode - refresh the parameter fields
            UpdateFromTemplate();
        }
    }

    /// <summary>
    /// Save parameter definitions to the .def file
    /// </summary>
    public void SaveParameterDefinitions()
    {
        if (_templateArtifact == null) return;

        // Get or create the definition
        var definition = _templateArtifact.GetOrCreateDefinition();

        // Update template metadata from edit view model
        definition.TemplateId = EditViewModel.EditableTemplateId;
        definition.DisplayName = EditViewModel.EditableDisplayName;
        definition.Description = string.IsNullOrWhiteSpace(EditViewModel.EditableDescription) 
            ? null 
            : EditViewModel.EditableDescription;
        
        // Update the parameters from the edit models
        definition.Parameters = EditViewModel.GetTemplateParameters();

        // Save to file
        definition.SaveForTemplate(_templateArtifact.FilePath);
        
        EditViewModel.HasUnsavedChanges = false;

        // Reload the template artifact to pick up changes
        _templateArtifact.ReloadDefinition();
        
        // Update the display name and description shown in the header
        TemplateName = _templateArtifact.DisplayName;
        TemplateDescription = _templateArtifact.Description;
        
        // Refresh the parameter fields for execution mode
        if (!IsEditMode)
        {
            UpdateFromTemplate();
        }
    }

    /// <summary>
    /// Update the view model from the template artifact
    /// </summary>
    private void UpdateFromTemplate()
    {
        if (_templateArtifact == null)
        {
            TemplateName = string.Empty;
            TemplateDescription = null;
            EditViewModel.LoadFromParameters(
                Enumerable.Empty<TemplateParameter>(), 
                string.Empty, 
                string.Empty, 
                null);
            ExecutionViewModel.ParameterFields = new List<FieldViewModelBase>();
            ExecutionViewModel.IsScribanTemplate = false;
            ExecutionViewModel.TemplateFilePath = null;
            return;
        }

        TemplateName = _templateArtifact.DisplayName;
        TemplateDescription = _templateArtifact.Description;

        // Set template type info for execution view - check by file extension
        var fileExtension = Path.GetExtension(_templateArtifact.FilePath)?.ToLowerInvariant();
        ExecutionViewModel.IsScribanTemplate = fileExtension == ".scriban";
        ExecutionViewModel.TemplateFilePath = _templateArtifact.FilePath;

        // Load editable template metadata
        var definition = _templateArtifact.Definition;
        var templateId = definition?.TemplateId ?? Path.GetFileNameWithoutExtension(_templateArtifact.FilePath);
        var displayName = definition?.DisplayName ?? _templateArtifact.FileName;
        var description = definition?.Description;

        // Load parameter definitions for edit mode
        EditViewModel.LoadFromParameters(
            _templateArtifact.Parameters, 
            templateId, 
            displayName, 
            description);

        // Create field view models for each parameter (execution mode)
        var fields = new List<FieldViewModelBase>();
        foreach (var parameter in _templateArtifact.Parameters.OrderBy(p => p.DisplayOrder))
        {
            var field = CreateFieldForParameter(parameter);
            if (field != null)
            {
                fields.Add(field);
            }
        }

        ExecutionViewModel.ParameterFields = fields;
    }

    /// <summary>
    /// Get all available TableArtifacts from the workspace
    /// </summary>
    private List<TemplateDatasourceArtifactItem> GetAvailableTableArtifacts()
    {
        var items = new List<TemplateDatasourceArtifactItem>();

        if (_workspaceController?.CurrentWorkspace == null)
            return items;

        var decorators = _workspaceController?.CurrentWorkspace.FindDescendantDecorators<TemplateDatasourceProviderDecorator>();
        
        foreach (var decorator in decorators)
        {
            var datasourceArtifact = decorator.Artifact.FindAncesterOfType<DatasourceArtifact>();
            
            if (datasourceArtifact != null)
            {
                items.Add(new TemplateDatasourceArtifactItem
                {
                    DatasourceTargetArtifact = decorator.Artifact,
                    DatasourceArtifact = datasourceArtifact,
                    DisplayName = decorator.DisplayName,
                    FullPath = decorator.FullPath
                });
            }
        }

        return items;
    }

    /// <summary>
    /// Create the appropriate field view model for a parameter based on its type
    /// </summary>
    private FieldViewModelBase? CreateFieldForParameter(TemplateParameter parameter)
    {
        // Check for TableArtifactData type first
        if (parameter.IsTemplateDatasourceArtifactData)
        {
            var tableField = new ComboboxFieldModel
            {
                Name = parameter.Name,
                Label = parameter.GetDisplayLabel(),
                Tooltip = parameter.Description ?? parameter.Tooltip ?? "Select a table from the workspace to load data from",
                IsRequired = parameter.Required,
                Items = GetAvailableTableArtifacts().Cast<ComboboxItem>().ToList()                
            };

            return tableField;
        }

        var paramType = parameter.GetParameterType() ?? typeof(string);

        // If there are allowed values, use combobox
        if (parameter.AllowedValues?.Count > 0)
        {
            var comboField = new ComboboxFieldModel
            {
                Name = parameter.Name,
                Label = parameter.GetDisplayLabel(),
                Tooltip = parameter.Description ?? parameter.Tooltip,
                IsRequired = parameter.Required,
                Items = parameter.AllowedValues
                    .Select(v => new ComboboxItem { DisplayName = v, Value = v })
                    .ToList()
            };

            if (!string.IsNullOrEmpty(parameter.DefaultValue))
            {
                var selectedItem = comboField.Items
                    .FirstOrDefault(i => i.Value?.ToString() == parameter.DefaultValue);
                if (selectedItem != null)
                {
                    comboField.Value = selectedItem.Value;
                }
            }

            return comboField;
        }

        // Boolean type
        if (paramType == typeof(bool))
        {
            var boolField = new BooleanFieldModel
            {
                Name = parameter.Name,
                Label = parameter.GetDisplayLabel(),
                Tooltip = parameter.Description ?? parameter.Tooltip,
                IsRequired = parameter.Required
            };

            if (!string.IsNullOrEmpty(parameter.DefaultValue) && bool.TryParse(parameter.DefaultValue, out var defaultBool))
            {
                boolField.Value = defaultBool;
            }

            return boolField;
        }

        // Integer type
        if (paramType == typeof(int) || paramType == typeof(long))
        {
            var intField = new IntegerFieldModel
            {
                Name = parameter.Name,
                Label = parameter.GetDisplayLabel(),
                Tooltip = parameter.Description ?? parameter.Tooltip,
                IsRequired = parameter.Required
            };

            if (!string.IsNullOrEmpty(parameter.DefaultValue) && int.TryParse(parameter.DefaultValue, out var defaultInt))
            {
                intField.Value = defaultInt;
            }

            return intField;
        }

        // DateOnly type
        if (paramType == typeof(DateTime) || paramType == typeof(DateOnly))
        {
            var dateField = new DateOnlyFieldModel
            {
                Name = parameter.Name,
                Label = parameter.GetDisplayLabel(),
                Tooltip = parameter.Description ?? parameter.Tooltip,
                IsRequired = parameter.Required
            };

            return dateField;
        }

        // Default to string/text field
        var textField = new SingleLineTextFieldModel
        {
            Name = parameter.Name,
            Label = parameter.GetDisplayLabel(),
            Tooltip = parameter.Description ?? parameter.Tooltip,
            IsRequired = parameter.Required
        };

        if (!string.IsNullOrEmpty(parameter.DefaultValue))
        {
            textField.Value = parameter.DefaultValue;
        }

        return textField;
    }

    /// <summary>
    /// Get parameter values as a dictionary for template execution
    /// </summary>
    public Dictionary<string, object?> GetParameterValues()
    {
        return ExecutionViewModel.GetParameterValues();
    }

    /// <summary>
    /// Get the template parameters definitions (for accessing TableDataFilter, etc.)
    /// </summary>
    public IReadOnlyList<TemplateParameter> GetTemplateParameters()
    {
        return _templateArtifact?.Parameters ?? new List<TemplateParameter>();
    }

    #region Legacy properties for backward compatibility
    
    /// <summary>
    /// List of parameter field view models for the UI (execution mode)
    /// </summary>
    [Obsolete("Use ExecutionViewModel.ParameterFields instead")]
    public List<FieldViewModelBase> ParameterFields
    {
        get => ExecutionViewModel.ParameterFields;
        set => ExecutionViewModel.ParameterFields = value;
    }

    /// <summary>
    /// List of parameter definitions for editing
    /// </summary>
    [Obsolete("Use EditViewModel.ParameterDefinitions instead")]
    public ObservableCollection<TemplateParameterEditModel> ParameterDefinitions
    {
        get => EditViewModel.ParameterDefinitions;
        set => EditViewModel.ParameterDefinitions = value;
    }

    /// <summary>
    /// Currently selected parameter definition for editing
    /// </summary>
    [Obsolete("Use EditViewModel.SelectedParameterDefinition instead")]
    public TemplateParameterEditModel? SelectedParameterDefinition
    {
        get => EditViewModel.SelectedParameterDefinition;
        set => EditViewModel.SelectedParameterDefinition = value;
    }

    /// <summary>
    /// Editable Template ID
    /// </summary>
    [Obsolete("Use EditViewModel.EditableTemplateId instead")]
    public string EditableTemplateId
    {
        get => EditViewModel.EditableTemplateId;
        set => EditViewModel.EditableTemplateId = value;
    }

    /// <summary>
    /// Editable Display Name
    /// </summary>
    [Obsolete("Use EditViewModel.EditableDisplayName instead")]
    public string EditableDisplayName
    {
        get => EditViewModel.EditableDisplayName;
        set => EditViewModel.EditableDisplayName = value;
    }

    /// <summary>
    /// Editable Description
    /// </summary>
    [Obsolete("Use EditViewModel.EditableDescription instead")]
    public string EditableDescription
    {
        get => EditViewModel.EditableDescription;
        set => EditViewModel.EditableDescription = value;
    }

    [Obsolete("Use EditViewModel.AddParameter instead")]
    public void AddParameter() => EditViewModel.AddParameter();

    [Obsolete("Use EditViewModel.RemoveSelectedParameter instead")]
    public void RemoveSelectedParameter() => EditViewModel.RemoveSelectedParameter();

    [Obsolete("Use EditViewModel.MoveParameterUp instead")]
    public void MoveParameterUp() => EditViewModel.MoveParameterUp();

    [Obsolete("Use EditViewModel.MoveParameterDown instead")]
    public void MoveParameterDown() => EditViewModel.MoveParameterDown();

    #endregion
}
