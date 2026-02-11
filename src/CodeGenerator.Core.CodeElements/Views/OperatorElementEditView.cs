using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.Views;

/// <summary>
/// View for editing OperatorElementArtifact properties
/// </summary>
public partial class OperatorElementEditView : UserControl, IView<OperatorElementEditViewModel>
{
    private OperatorElementEditViewModel? _viewModel;

    public OperatorElementEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(OperatorElementEditViewModel? viewModel)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        _viewModel = viewModel;

        if (_viewModel == null) return;

        codeElementEditView.BindViewModel(_viewModel);
        cbxOperatorType.BindViewModel(_viewModel.OperatorTypeField);
        txtReturnType.BindViewModel(_viewModel.ReturnTypeField);

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((OperatorElementEditViewModel)(object)viewModel);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
