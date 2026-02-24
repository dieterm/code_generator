using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views;

/// <summary>
/// View for editing template parameters and metadata
/// </summary>
public partial class TemplateParametersEditView : UserControl, IView<TemplateParametersEditViewModel>
{
    private TemplateParametersEditViewModel? _viewModel;
    private bool _isBinding;

    public TemplateParametersEditView()
    {
        InitializeComponent();
        SetupEventHandlers();
    }

    private void SetupEventHandlers()
    {
        btnAddParameter.Click += BtnAddParameter_Click;
        btnRemoveParameter.Click += BtnRemoveParameter_Click;
        btnMoveUp.Click += BtnMoveUp_Click;
        btnMoveDown.Click += BtnMoveDown_Click;
        btnSave.Click += BtnSave_Click;
        lstParameters.SelectedIndexChanged += LstParameters_SelectedIndexChanged;

        txtEditTemplateId.TextChanged += TemplateMetadataChanged;
        txtEditDisplayName.TextChanged += TemplateMetadataChanged;
        txtEditDescription.TextChanged += TemplateMetadataChanged;
    }

    public void BindViewModel(TemplateParametersEditViewModel viewModel)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            _viewModel.ParameterDefinitions.CollectionChanged -= ParameterDefinitions_CollectionChanged;
        }

        _viewModel = viewModel;

        if (_viewModel != null)
        {
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            _viewModel.ParameterDefinitions.CollectionChanged += ParameterDefinitions_CollectionChanged;
            
            // Bind the parameter edit view
            parameterEditView.BindViewModel(_viewModel.ParameterEditViewModel);
            
            BindFromViewModel();
            ObserveSelectedParameterDefinition();
        }
    }

    private TemplateParameterEditModel? _currentObservedParameterEditModel;
    private void ObserveSelectedParameterDefinition()
    {
        if (_currentObservedParameterEditModel != null)
        {
            _currentObservedParameterEditModel.PropertyChanged -= SelectedParameterDefinition_PropertyChanged;
        }

        _currentObservedParameterEditModel = _viewModel?.SelectedParameterDefinition;

        if (_currentObservedParameterEditModel != null)
        {
            _currentObservedParameterEditModel.PropertyChanged += SelectedParameterDefinition_PropertyChanged;
        }
    }

    private void SelectedParameterDefinition_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(e.PropertyName== nameof(TemplateParameterEditModel.Name) )
        {
            // If any of the main properties of the parameter change, we need to refresh the list to update the display
            //RefreshParameterList();
            //lstParameters.Invalidate(); // Invalidate the list to trigger a redraw without resetting selection
            
            //lstParameters.Items[lstParameters.SelectedIndex] = lstParameters.SelectedItem; // Force the ListBox to update the display of the selected item
        }
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : IViewModel
    {
        BindViewModel((TemplateParametersEditViewModel)(object)viewModel);
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (InvokeRequired)
        {
            Invoke(() => ViewModel_PropertyChanged(sender, e));
            return;
        }

        switch (e.PropertyName)
        {
            case nameof(TemplateParametersEditViewModel.EditableTemplateId):
            case nameof(TemplateParametersEditViewModel.EditableDisplayName):
            case nameof(TemplateParametersEditViewModel.EditableDescription):
                BindTemplateMetadata();
                break;
            case nameof(TemplateParametersEditViewModel.ParameterDefinitions):
                RefreshParameterList();
                break;
            case nameof(TemplateParametersEditViewModel.SelectedParameterDefinition):
                UpdateButtonStates();
                ObserveSelectedParameterDefinition();
                break;
            case nameof(TemplateParametersEditViewModel.HasUnsavedChanges):
                btnSave.Enabled = _viewModel?.HasUnsavedChanges ?? false;
                break;
        }
    }

    private void ParameterDefinitions_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RefreshParameterList();
    }

    private void BindFromViewModel()
    {
        BindTemplateMetadata();
        RefreshParameterList();
        UpdateButtonStates();
        btnSave.Enabled = _viewModel?.HasUnsavedChanges ?? false;
    }

    private void BindTemplateMetadata()
    {
        _isBinding = true;
        try
        {
            txtEditTemplateId.Text = _viewModel?.EditableTemplateId ?? string.Empty;
            txtEditDisplayName.Text = _viewModel?.EditableDisplayName ?? string.Empty;
            txtEditDescription.Text = _viewModel?.EditableDescription ?? string.Empty;
        }
        finally
        {
            _isBinding = false;
        }
    }

    private void TemplateMetadataChanged(object? sender, EventArgs e)
    {
        if (_isBinding || _viewModel == null) return;

        _viewModel.EditableTemplateId = txtEditTemplateId.Text;
        _viewModel.EditableDisplayName = txtEditDisplayName.Text;
        _viewModel.EditableDescription = txtEditDescription.Text;
    }

    private void RefreshParameterList()
    {
        if (_viewModel == null) return;

        lstParameters.BeginUpdate();
        try
        {
            lstParameters.DataSource = null;
            lstParameters.DataSource = new BindingList<TemplateParameterEditModel>(_viewModel.ParameterDefinitions);
            lstParameters.DisplayMember = nameof(TemplateParameterEditModel.Name);

            if (_viewModel.SelectedParameterDefinition != null)
            {
                lstParameters.SelectedItem = _viewModel.SelectedParameterDefinition;
            }
        }
        finally
        {
            lstParameters.EndUpdate();
        }
    }

    private void UpdateButtonStates()
    {
        var hasSelection = _viewModel?.SelectedParameterDefinition != null;
        btnRemoveParameter.Enabled = hasSelection;
        btnMoveUp.Enabled = _viewModel?.CanMoveUp ?? false;
        btnMoveDown.Enabled = _viewModel?.CanMoveDown ?? false;
    }

    private void LstParameters_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_viewModel != null && lstParameters.SelectedItem is TemplateParameterEditModel param)
        {
            _viewModel.SelectedParameterDefinition = param;
        }
    }

    private void BtnAddParameter_Click(object? sender, EventArgs e)
    {
        _viewModel?.AddParameter();
        RefreshParameterList();
        if (_viewModel?.SelectedParameterDefinition != null)
        {
            lstParameters.SelectedItem = _viewModel.SelectedParameterDefinition;
        }
    }

    private void BtnRemoveParameter_Click(object? sender, EventArgs e)
    {
        _viewModel?.RemoveSelectedParameter();
    }

    private void BtnMoveUp_Click(object? sender, EventArgs e)
    {
        _viewModel?.MoveParameterUp();
        RefreshParameterList();
        if (_viewModel?.SelectedParameterDefinition != null)
        {
            lstParameters.SelectedItem = _viewModel.SelectedParameterDefinition;
        }
    }

    private void BtnMoveDown_Click(object? sender, EventArgs e)
    {
        _viewModel?.MoveParameterDown();
        RefreshParameterList();
        if (_viewModel?.SelectedParameterDefinition != null)
        {
            lstParameters.SelectedItem = _viewModel.SelectedParameterDefinition;
        }
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        _viewModel?.RequestSave();
    }
}
