using CodeGenerator.Core.Templates;
using CodeGenerator.Shared.ViewModels;
using System.Collections.ObjectModel;

namespace CodeGenerator.Application.ViewModels.Template;

/// <summary>
/// ViewModel for template parameters edit view - displays template metadata and parameter list for editing
/// </summary>
public class TemplateParametersEditViewModel : ViewModelBase
{
    private string _editableTemplateId = string.Empty;
    private string _editableDisplayName = string.Empty;
    private string _editableDescription = string.Empty;
    private ObservableCollection<TemplateParameterEditModel> _parameterDefinitions = new();
    private TemplateParameterEditModel? _selectedParameterDefinition;
    private bool _hasUnsavedChanges;
    private TemplateParameterEditViewModel _parameterEditViewModel = new();

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
        set
        {
            if (SetProperty(ref _selectedParameterDefinition, value))
            {
                // Update the parameter edit view model
                ParameterEditViewModel.Parameter = value;
            }
        }
    }

    /// <summary>
    /// ViewModel for editing the selected parameter
    /// </summary>
    public TemplateParameterEditViewModel ParameterEditViewModel
    {
        get => _parameterEditViewModel;
        set => SetProperty(ref _parameterEditViewModel, value);
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
    /// Whether the up button should be enabled
    /// </summary>
    public bool CanMoveUp => SelectedParameterDefinition != null && 
                             ParameterDefinitions.IndexOf(SelectedParameterDefinition) > 0;

    /// <summary>
    /// Whether the down button should be enabled
    /// </summary>
    public bool CanMoveDown => SelectedParameterDefinition != null && 
                               ParameterDefinitions.IndexOf(SelectedParameterDefinition) < ParameterDefinitions.Count - 1;

    /// <summary>
    /// Event raised when save is requested
    /// </summary>
    public event EventHandler? SaveRequested;

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
            OnPropertyChanged(nameof(CanMoveUp));
            OnPropertyChanged(nameof(CanMoveDown));
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
            OnPropertyChanged(nameof(CanMoveUp));
            OnPropertyChanged(nameof(CanMoveDown));
        }
    }

    /// <summary>
    /// Request save of parameter definitions
    /// </summary>
    public void RequestSave()
    {
        SaveRequested?.Invoke(this, EventArgs.Empty);
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
    /// Load from template parameters
    /// </summary>
    public void LoadFromParameters(IEnumerable<TemplateParameter> parameters, string templateId, string displayName, string? description)
    {
        EditableTemplateId = templateId;
        EditableDisplayName = displayName;
        EditableDescription = description ?? string.Empty;

        var editModels = new ObservableCollection<TemplateParameterEditModel>();
        foreach (var param in parameters.OrderBy(p => p.DisplayOrder))
        {
            var editModel = TemplateParameterEditModel.FromTemplateParameter(param);
            editModel.PropertyChanged += (s, e) => HasUnsavedChanges = true;
            editModels.Add(editModel);
        }
        ParameterDefinitions = editModels;
        SelectedParameterDefinition = editModels.FirstOrDefault();
        HasUnsavedChanges = false;
    }

    /// <summary>
    /// Get parameter definitions as TemplateParameter objects
    /// </summary>
    public List<TemplateParameter> GetTemplateParameters()
    {
        return ParameterDefinitions.Select(p => p.ToTemplateParameter()).ToList();
    }
}
