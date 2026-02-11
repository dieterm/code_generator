using CodeGenerator.Core.CodeElements.ViewModels.Statements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.Views.Statements;

public partial class CompositeStatementEditView : UserControl, IView<CompositeStatementEditViewModel>
{
    private CompositeStatementEditViewModel? _viewModel;

    public CompositeStatementEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(CompositeStatementEditViewModel? viewModel)
    {
        if (_viewModel != null)
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;

        _viewModel = viewModel;
        if (_viewModel == null) return;

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e) { }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((CompositeStatementEditViewModel)(object)viewModel);
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
