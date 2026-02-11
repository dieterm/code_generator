using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.Views;

public partial class MethodElementEditView : UserControl, IView<MethodElementEditViewModel>
{
    private MethodElementEditViewModel? _viewModel;

    public MethodElementEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(MethodElementEditViewModel? viewModel)
    {
        if (_viewModel != null)
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;

        _viewModel = viewModel;
        if (_viewModel == null) return;

        codeElementEditView.BindViewModel(_viewModel);
        txtReturnType.BindViewModel(_viewModel.ReturnTypeField);
        chkIsExpressionBodied.BindViewModel(_viewModel.IsExpressionBodiedField);
        txtExpressionBody.BindViewModel(_viewModel.ExpressionBodyField);
        chkIsExtensionMethod.BindViewModel(_viewModel.IsExtensionMethodField);

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e) { }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((MethodElementEditViewModel)(object)viewModel);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_viewModel != null)
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
