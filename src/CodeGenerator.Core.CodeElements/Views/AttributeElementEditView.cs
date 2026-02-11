using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.Views;

/// <summary>
/// View for editing AttributeElementArtifact properties
/// </summary>
public partial class AttributeElementEditView : UserControl, IView<AttributeElementEditViewModel>
{
    private AttributeElementEditViewModel? _viewModel;

    public AttributeElementEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(AttributeElementEditViewModel? viewModel)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        _viewModel = viewModel;

        if (_viewModel == null) return;

        codeElementEditView.BindViewModel(_viewModel);
        txtAttributeName.BindViewModel(_viewModel.AttributeNameField);
        cbxTarget.BindViewModel(_viewModel.TargetField);
        lstArguments.BindViewModel(_viewModel.ArgumentsField);
        lstNamedArguments.BindViewModel(_viewModel.NamedArgumentsField);

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((AttributeElementEditViewModel)(object)viewModel);
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
