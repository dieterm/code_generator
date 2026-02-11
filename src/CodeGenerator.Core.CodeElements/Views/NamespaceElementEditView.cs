using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.Views;

/// <summary>
/// View for editing NamespaceElementArtifact properties
/// </summary>
public partial class NamespaceElementEditView : UserControl, IView<NamespaceElementEditViewModel>
{
    private NamespaceElementEditViewModel? _viewModel;

    public NamespaceElementEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(NamespaceElementEditViewModel? viewModel)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        _viewModel = viewModel;

        if (_viewModel == null) return;

        codeElementEditView.BindViewModel(_viewModel);
        chkIsFileScoped.BindViewModel(_viewModel.IsFileScopedField);

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((NamespaceElementEditViewModel)(object)viewModel);
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
