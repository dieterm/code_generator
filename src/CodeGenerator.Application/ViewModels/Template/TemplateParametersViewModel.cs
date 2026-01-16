using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
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
    private string _editableTemplateId = string.Empty;
    private string _editableDisplayName = string.Empty;
    private string _editableDescription = string.Empty;
    private List<FieldViewModelBase> _parameterFields = new();
    private ObservableCollection<TemplateParameterEditModel> _parameterDefinitions = new();
    private TemplateParameterEditModel? _selectedParameterDefinition;
    private bool _canExecute;
    private bool _isExecuting;
    private bool _isEditMode;
    private bool _hasUnsavedChanges;
    private WorkspaceTreeViewController? _workspaceController;

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
    /// Display name of the template (read-only, for display in run mode)
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
    /// Editable Template ID for the definition file
    /// </summary>
    public string EditableTemplateId
    {
        get => _editableTemplateId;
        set
        {
            if (SetProperty(ref _editableTemplateId, value))
            {
                HasUnsavedChanges = true;
            }
        }
    }

    /// <summary>
    /// Editable Display Name for the definition file
    /// </summary>
    public string EditableDisplayName
    {
        get => _editableDisplayName;
        set
        {
            if (SetProperty(ref _editableDisplayName, value))
            {
                HasUnsavedChanges = true;
            }
        }
    }

    /// <summary>
    /// Editable Description for the definition file
    /// </summary>
    public string EditableDescription
    {
        get => _editableDescription;
        set
        {
            if (SetProperty(ref _editableDescription, value))
            {
                HasUnsavedChanges = true;
            }
        }
    }

    /// <summary>
    /// List of parameter field view models for the UI (execution mode)
    /// </summary>
    public List<FieldViewModelBase> ParameterFields
    {
        get => _parameterFields;
        set => SetProperty(ref _parameterFields, value);
    }

    /// <summary>
    /// List of parameter definitions for editing
    /// </summary>
    public ObservableCollection<TemplateParameterEditModel> ParameterDefinitions
    {
        get => _parameterDefinitions;
        set => SetProperty(ref _parameterDefinitions, value);
    }

    /// <summary>
    /// Currently selected parameter definition for editing
    /// </summary>
    public TemplateParameterEditModel? SelectedParameterDefinition
    {
        get => _selectedParameterDefinition;
        set => SetProperty(ref _selectedParameterDefinition, value);
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
        set => SetProperty(ref _isExecuting, value);
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
    /// Whether there are unsaved changes to parameter definitions
    /// </summary>
    public bool HasUnsavedChanges
    {
        get => _hasUnsavedChanges;
        set => SetProperty(ref _hasUnsavedChanges, value);
    }

    /// <summary>
    /// Event raised when the user requests to execute the template
    /// </summary>
    public event EventHandler? ExecuteRequested;

    /// <summary>
    /// Raises the ExecuteRequested event
    /// </summary>
    public void RequestExecute()
    {
        ExecuteRequested?.Invoke(this, EventArgs.Empty);
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
    /// Add a new parameter definition
    /// </summary>
    public void AddParameter()
    {
        var newParam = new TemplateParameterEditModel
        {
            Name = $"Parameter{ParameterDefinitions.Count + 1}",
            DisplayOrder = ParameterDefinitions.Count
        };
        newParam.PropertyChanged += (s, e) => HasUnsavedChanges = true;
        ParameterDefinitions.Add(newParam);
        SelectedParameterDefinition = newParam;
        HasUnsavedChanges = true;
    }

    /// <summary>
    /// Remove the selected parameter definition
    /// </summary>
    public void RemoveSelectedParameter()
    {
        if (SelectedParameterDefinition != null)
        {
            ParameterDefinitions.Remove(SelectedParameterDefinition);
            SelectedParameterDefinition = ParameterDefinitions.LastOrDefault();
            HasUnsavedChanges = true;
            ReorderParameters();
        }
    }

    /// <summary>
    /// Move the selected parameter up in the list
    /// </summary>
    public void MoveParameterUp()
    {
        if (SelectedParameterDefinition == null) return;
        var index = ParameterDefinitions.IndexOf(SelectedParameterDefinition);
        if (index > 0)
        {
            ParameterDefinitions.Move(index, index - 1);
            HasUnsavedChanges = true;
            ReorderParameters();
        }
    }

    /// <summary>
    /// Move the selected parameter down in the list
    /// </summary>
    public void MoveParameterDown()
    {
        if (SelectedParameterDefinition == null) return;
        var index = ParameterDefinitions.IndexOf(SelectedParameterDefinition);
        if (index < ParameterDefinitions.Count - 1)
        {
            ParameterDefinitions.Move(index, index + 1);
            HasUnsavedChanges = true;
            ReorderParameters();
        }
    }

    /// <summary>
    /// Reorder parameters based on their position in the collection
    /// </summary>
    private void ReorderParameters()
    {
        for (int i = 0; i < ParameterDefinitions.Count; i++)
        {
            ParameterDefinitions[i].DisplayOrder = i;
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

        // Update template metadata
        definition.TemplateId = EditableTemplateId;
        definition.DisplayName = EditableDisplayName;
        definition.Description = string.IsNullOrWhiteSpace(EditableDescription) ? null : EditableDescription;
        
        // Update the parameters from the edit models
        definition.Parameters = ParameterDefinitions
            .Select(p => p.ToTemplateParameter())
            .ToList();

        // Save to file
        definition.SaveForTemplate(_templateArtifact.FilePath);
        
        HasUnsavedChanges = false;

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
            EditableTemplateId = string.Empty;
            EditableDisplayName = string.Empty;
            EditableDescription = string.Empty;
            ParameterFields = new List<FieldViewModelBase>();
            ParameterDefinitions = new ObservableCollection<TemplateParameterEditModel>();
            CanExecute = false;
            return;
        }

        TemplateName = _templateArtifact.DisplayName;
        TemplateDescription = _templateArtifact.Description;

        // Load editable template metadata
        var definition = _templateArtifact.Definition;
        EditableTemplateId = definition?.TemplateId ?? Path.GetFileNameWithoutExtension(_templateArtifact.FilePath);
        EditableDisplayName = definition?.DisplayName ?? _templateArtifact.FileName;
        EditableDescription = definition?.Description ?? string.Empty;

        // Load parameter definitions for edit mode
        var editModels = new ObservableCollection<TemplateParameterEditModel>();
        foreach (var param in _templateArtifact.Parameters.OrderBy(p => p.DisplayOrder))
        {
            var editModel = TemplateParameterEditModel.FromTemplateParameter(param);
            editModel.PropertyChanged += (s, e) => HasUnsavedChanges = true;
            editModels.Add(editModel);
        }
        ParameterDefinitions = editModels;

        // Create field view models for each parameter (execution mode)
        var fields = new List<FieldViewModelBase>();
        foreach (var parameter in _templateArtifact.Parameters.OrderBy(p => p.DisplayOrder))
        {
            var field = CreateFieldForParameter(parameter);
            if (field != null)
            {
                field.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(FieldViewModelBase.Value))
                    {
                        ValidateParameters();
                    }
                };
                fields.Add(field);
            }
        }

        ParameterFields = fields;
        HasUnsavedChanges = false;
        ValidateParameters();
    }

    /// <summary>
    /// Get all available TableArtifacts from the workspace
    /// </summary>
    private List<TableArtifactItem> GetAvailableTableArtifacts()
    {
        var items = new List<TableArtifactItem>();

        if (_workspaceController?.CurrentWorkspace == null)
            return items;
        // TODO: visit complete workspace tree recursively to find datasources
        // by checking if an artifact has a TemplateDatasourceProviderDecorator

        // Iterate through all datasources
        foreach (var datasource in _workspaceController.CurrentWorkspace.Datasources.GetDatasources())
        {
            // Find all TableArtifacts in this datasource
            foreach (var child in datasource.Children)
            {
                if (child is TableArtifact tableArtifact)
                {
                    var datasourceProvider = tableArtifact.GetTemplateDatasourceProviderDecorator();
                    if (datasourceProvider == null)
                        continue;

                    items.Add(new TableArtifactItem
                    {
                        TableArtifact = tableArtifact,
                        DatasourceArtifact = datasource,
                        DisplayName = datasourceProvider.DisplayName,
                        FullPath = datasourceProvider.FullPath
                    });
                }
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
        if (parameter.IsTableArtifactData)
        {
            var tableField = new TableArtifactFieldModel
            {
                Name = parameter.Name,
                Label = parameter.GetDisplayLabel(),
                Tooltip = parameter.Description ?? parameter.Tooltip ?? "Select a table from the workspace to load data from",
                IsRequired = parameter.Required,
                AvailableTables = GetAvailableTableArtifacts()
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
                comboField.Value = parameter.DefaultValue;
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
    /// Validate that all required parameters have values
    /// </summary>
    private void ValidateParameters()
    {
        if (_templateArtifact == null)
        {
            CanExecute = false;
            return;
        }

        // Check all required parameters have values
        foreach (var field in ParameterFields)
        {
            if (field.IsRequired)
            {
                if (field is TableArtifactFieldModel tableField)
                {
                    if (tableField.SelectedTable == null)
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
            values[field.Name] = field.Value;
        }

        return values;
    }

    /// <summary>
    /// Get the template parameters definitions (for accessing TableDataFilter, etc.)
    /// </summary>
    public IReadOnlyList<TemplateParameter> GetTemplateParameters()
    {
        return _templateArtifact?.Parameters ?? new List<TemplateParameter>();
    }
}

/// <summary>
/// Edit model for a template parameter definition
/// </summary>
public class TemplateParameterEditModel : ViewModelBase
{
    private string _name = string.Empty;
    private string? _description;
    private string _typeName = "System.String";
    private bool _required;
    private string? _defaultValue;
    private string? _allowedValuesText;
    private string? _label;
    private string? _tooltip;
    private int _displayOrder;
    private string? _tableDataFilter;
    private int? _tableDataMaxRows;

    /// <summary>
    /// Parameter name (used as key in template)
    /// </summary>
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    /// <summary>
    /// Description of the parameter
    /// </summary>
    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    /// <summary>
    /// Type name (e.g., "System.String", "System.Int32", "System.Boolean")
    /// </summary>
    public string TypeName
    {
        get => _typeName;
        set => SetProperty(ref _typeName, value);
    }

    /// <summary>
    /// Whether this parameter is required
    /// </summary>
    public bool Required
    {
        get => _required;
        set => SetProperty(ref _required, value);
    }

    /// <summary>
    /// Default value for the parameter
    /// </summary>
    public string? DefaultValue
    {
        get => _defaultValue;
        set => SetProperty(ref _defaultValue, value);
    }

    /// <summary>
    /// Allowed values as semicolon-separated text
    /// </summary>
    public string? AllowedValuesText
    {
        get => _allowedValuesText;
        set => SetProperty(ref _allowedValuesText, value);
    }

    /// <summary>
    /// Display label for the parameter
    /// </summary>
    public string? Label
    {
        get => _label;
        set => SetProperty(ref _label, value);
    }

    /// <summary>
    /// Tooltip for the parameter
    /// </summary>
    public string? Tooltip
    {
        get => _tooltip;
        set => SetProperty(ref _tooltip, value);
    }

    /// <summary>
    /// Display order
    /// </summary>
    public int DisplayOrder
    {
        get => _displayOrder;
        set => SetProperty(ref _displayOrder, value);
    }

    /// <summary>
    /// SQL WHERE clause filter for TableArtifactData
    /// </summary>
    public string? TableDataFilter
    {
        get => _tableDataFilter;
        set => SetProperty(ref _tableDataFilter, value);
    }

    /// <summary>
    /// Maximum rows to load for TableArtifactData
    /// </summary>
    public int? TableDataMaxRows
    {
        get => _tableDataMaxRows;
        set => SetProperty(ref _tableDataMaxRows, value);
    }

    /// <summary>
    /// Available type options for the dropdown
    /// </summary>
    public static List<string> AvailableTypes => new()
    {
        "System.String",
        "System.Int32",
        "System.Int64",
        "System.Boolean",
        "System.DateTime",
        "System.DateOnly",
        "System.Decimal",
        "System.Double",
        TemplateParameter.TableArtifactDataTypeName
    };

    /// <summary>
    /// Convert to TemplateParameter
    /// </summary>
    public TemplateParameter ToTemplateParameter()
    {
        return new TemplateParameter
        {
            Name = Name,
            Description = Description,
            FullyQualifiedAssemblyTypeName = TypeName,
            Required = Required,
            DefaultValue = DefaultValue,
            AllowedValues = string.IsNullOrWhiteSpace(AllowedValuesText) 
                ? null 
                : AllowedValuesText.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
            Label = Label,
            Tooltip = Tooltip,
            DisplayOrder = DisplayOrder,
            TableDataFilter = TableDataFilter,
            TableDataMaxRows = TableDataMaxRows
        };
    }

    /// <summary>
    /// Create from TemplateParameter
    /// </summary>
    public static TemplateParameterEditModel FromTemplateParameter(TemplateParameter param)
    {
        return new TemplateParameterEditModel
        {
            Name = param.Name,
            Description = param.Description,
            TypeName = param.FullyQualifiedAssemblyTypeName,
            Required = param.Required,
            DefaultValue = param.DefaultValue,
            AllowedValuesText = param.AllowedValues != null ? string.Join("; ", param.AllowedValues) : null,
            Label = param.Label,
            Tooltip = param.Tooltip,
            DisplayOrder = param.DisplayOrder,
            TableDataFilter = param.TableDataFilter,
            TableDataMaxRows = param.TableDataMaxRows
        };
    }
}
