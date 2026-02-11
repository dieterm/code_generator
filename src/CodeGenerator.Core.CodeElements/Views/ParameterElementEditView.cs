using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.Views;

public partial class ParameterElementEditView : UserControl, IView<ParameterElementEditViewModel>
{
    private ParameterElementEditViewModel? _viewModel;

    public ParameterElementEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(ParameterElementEditViewModel? viewModel)
    {
        if (_viewModel != null)
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;

        _viewModel = viewModel;
        if (_viewModel == null) return;

        codeElementEditView.BindViewModel(_viewModel);
        txtTypeName.BindViewModel(_viewModel.TypeNameField);
        cbxModifier.BindViewModel(_viewModel.ModifierField);
        txtDefaultValue.BindViewModel(_viewModel.DefaultValueField);
        chkIsExtensionMethodThis.BindViewModel(_viewModel.IsExtensionMethodThisField);

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e) { }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((ParameterElementEditViewModel)(object)viewModel);
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
