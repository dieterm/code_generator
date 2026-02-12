using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.Views;

/// <summary>
/// Base View for editing CodeElementArtifactBase properties.
/// Provides Name, AccessModifier, Modifiers, Documentation and RawCode fields.
/// </summary>
public partial class CodeElementEditView : UserControl, IView<CodeElementEditViewModel>
{
    private CodeElementEditViewModel? _viewModel;

    public CodeElementEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(CodeElementEditViewModel? viewModel)
    {
        if (_viewModel != null)
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;

        _viewModel = viewModel;

        if (_viewModel == null) return;

        txtName.BindViewModel(_viewModel.NameField);
        cbxAccessModifier.BindViewModel(_viewModel.AccessModifierField);
        msfModifiers.BindViewModel(_viewModel.ModifiersField);
        txtDocumentation.BindViewModel(_viewModel.DocumentationField);
        txtRawCode.BindViewModel(_viewModel.RawCodeField);

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel(viewModel as CodeElementEditViewModel);
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
