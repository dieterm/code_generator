using CodeGenerator.Core.CodeElements.ViewModels.Statements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.Views.Statements;

public partial class SwitchCaseEditView : UserControl, IView<SwitchCaseEditViewModel>
{
    private SwitchCaseEditViewModel? _viewModel;

    public SwitchCaseEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(SwitchCaseEditViewModel? viewModel)
    {
        if (_viewModel != null)
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;

        _viewModel = viewModel;
        if (_viewModel == null) return;

        txtPattern.BindViewModel(_viewModel.PatternField);
        txtWhenClause.BindViewModel(_viewModel.WhenClauseField);

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e) { }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((SwitchCaseEditViewModel)(object)viewModel);
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
