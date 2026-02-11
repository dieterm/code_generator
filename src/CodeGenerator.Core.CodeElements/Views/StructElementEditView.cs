using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.Views;

/// <summary>
/// View for editing StructElementArtifact properties
/// </summary>
public partial class StructElementEditView : UserControl, IView<StructElementEditViewModel>
{
    private StructElementEditViewModel? _viewModel;

    public StructElementEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(StructElementEditViewModel? viewModel)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        _viewModel = viewModel;

        if (_viewModel == null) return;

        codeElementEditView.BindViewModel(_viewModel);
        chkIsRecord.BindViewModel(_viewModel.IsRecordField);
        chkIsReadonly.BindViewModel(_viewModel.IsReadonlyField);
        chkIsRef.BindViewModel(_viewModel.IsRefField);

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((StructElementEditViewModel)(object)viewModel);
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
