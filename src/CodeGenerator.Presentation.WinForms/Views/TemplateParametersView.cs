using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views;

/// <summary>
/// Main view for template parameters - shows header with toggle button
/// and switches between execution view and edit view
/// </summary>
public partial class TemplateParametersView : UserControl, IView<TemplateParametersViewModel>
{
    private TemplateParametersViewModel? _viewModel;

    public TemplateParametersView()
    {
        InitializeComponent();
        btnToggleEditMode.Click += BtnToggleEditMode_Click;
    }

    public void BindViewModel(TemplateParametersViewModel viewModel)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        _viewModel = viewModel;

        if (_viewModel != null)
        {
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            
            // Bind child views
            editView.BindViewModel(_viewModel.EditViewModel);
            executionView.BindViewModel(_viewModel.ExecutionViewModel);
            
            UpdateFromViewModel();
        }
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((TemplateParametersViewModel)(object)viewModel);
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
            case nameof(TemplateParametersViewModel.TemplateName):
                lblTemplateName.Text = _viewModel?.TemplateName ?? string.Empty;
                break;
            case nameof(TemplateParametersViewModel.TemplateDescription):
                lblTemplateDescription.Text = _viewModel?.TemplateDescription ?? string.Empty;
                break;
            case nameof(TemplateParametersViewModel.IsEditMode):
                UpdateEditModeDisplay();
                break;
        }
    }

    private void UpdateFromViewModel()
    {
        if (_viewModel == null) return;

        lblTemplateName.Text = _viewModel.TemplateName;
        lblTemplateDescription.Text = _viewModel.TemplateDescription ?? string.Empty;
        UpdateEditModeDisplay();
    }

    private void UpdateEditModeDisplay()
    {
        if (_viewModel == null) return;

        var isEditMode = _viewModel.IsEditMode;
        editView.Visible = isEditMode;
        executionView.Visible = !isEditMode;
        btnToggleEditMode.Text = isEditMode ? "Run" : "Edit";
    }

    private void BtnToggleEditMode_Click(object? sender, EventArgs e)
    {
        _viewModel?.ToggleEditMode();
    }
}
