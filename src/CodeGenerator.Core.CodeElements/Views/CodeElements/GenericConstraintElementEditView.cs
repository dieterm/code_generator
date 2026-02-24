using CodeGenerator.Core.CodeElements.ViewModels.CodeElements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.Views;

public partial class GenericConstraintElementEditView : UserControl, IView<GenericConstraintElementEditViewModel>
{
    private GenericConstraintElementEditViewModel? _viewModel;

    public GenericConstraintElementEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(GenericConstraintElementEditViewModel? viewModel)
    {
        if (_viewModel != null)
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;

        _viewModel = viewModel;
        if (_viewModel == null) return;

        codeElementEditView.BindViewModel(_viewModel);
        txtTypeParameterName.BindViewModel(_viewModel.TypeParameterNameField);
        msfConstraintKind.BindViewModel(_viewModel.ConstraintKindField);

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e) { }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : IViewModel
    {
        BindViewModel((GenericConstraintElementEditViewModel)(object)viewModel);
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
