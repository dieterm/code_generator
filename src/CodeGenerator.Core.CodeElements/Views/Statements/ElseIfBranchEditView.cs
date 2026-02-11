using CodeGenerator.Core.CodeElements.ViewModels.Statements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.Views.Statements;

public partial class ElseIfBranchEditView : UserControl, IView<ElseIfBranchEditViewModel>
{
    private ElseIfBranchEditViewModel? _viewModel;

    public ElseIfBranchEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(ElseIfBranchEditViewModel? viewModel)
    {
        if (_viewModel != null)
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;

        _viewModel = viewModel;
        if (_viewModel == null) return;

        txtCondition.BindViewModel(_viewModel.ConditionField);

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e) { }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((ElseIfBranchEditViewModel)(object)viewModel);
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
