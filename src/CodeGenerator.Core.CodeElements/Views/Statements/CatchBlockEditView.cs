using CodeGenerator.Core.CodeElements.ViewModels.Statements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.Views.Statements;

public partial class CatchBlockEditView : UserControl, IView<CatchBlockEditViewModel>
{
    private CatchBlockEditViewModel? _viewModel;

    public CatchBlockEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(CatchBlockEditViewModel? viewModel)
    {
        if (_viewModel != null)
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;

        _viewModel = viewModel;
        if (_viewModel == null) return;

        txtExceptionVariable.BindViewModel(_viewModel.ExceptionVariableField);
        txtWhenFilter.BindViewModel(_viewModel.WhenFilterField);

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e) { }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((CatchBlockEditViewModel)(object)viewModel);
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
