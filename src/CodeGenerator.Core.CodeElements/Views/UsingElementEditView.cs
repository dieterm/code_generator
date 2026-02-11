using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.Views;

/// <summary>
/// View for editing UsingElementArtifact properties
/// </summary>
public partial class UsingElementEditView : UserControl, IView<UsingElementEditViewModel>
{
    private UsingElementEditViewModel? _viewModel;

    public UsingElementEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(UsingElementEditViewModel? viewModel)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        _viewModel = viewModel;

        if (_viewModel == null) return;

        codeElementEditView.BindViewModel(_viewModel);
        txtNamespace.BindViewModel(_viewModel.NamespaceField);
        txtAlias.BindViewModel(_viewModel.AliasField);
        chkIsGlobal.BindViewModel(_viewModel.IsGlobalField);

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((UsingElementEditViewModel)(object)viewModel);
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
