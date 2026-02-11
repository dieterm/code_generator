using CodeGenerator.Core.CodeElements.ViewModels.Statements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.Views.Statements;

public partial class ForStatementEditView : UserControl, IView<ForStatementEditViewModel>
{
    private ForStatementEditViewModel? _viewModel;

    public ForStatementEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(ForStatementEditViewModel? viewModel)
    {
        if (_viewModel != null)
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;

        _viewModel = viewModel;
        if (_viewModel == null) return;

        txtInitializer.BindViewModel(_viewModel.InitializerField);
        txtCondition.BindViewModel(_viewModel.ConditionField);
        txtIncrementer.BindViewModel(_viewModel.IncrementerField);

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e) { }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((ForStatementEditViewModel)(object)viewModel);
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
