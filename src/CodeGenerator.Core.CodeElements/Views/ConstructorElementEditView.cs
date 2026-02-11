using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.Views;

public partial class ConstructorElementEditView : UserControl, IView<ConstructorElementEditViewModel>
{
    private ConstructorElementEditViewModel? _viewModel;

    public ConstructorElementEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(ConstructorElementEditViewModel? viewModel)
    {
        if (_viewModel != null)
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;

        _viewModel = viewModel;
        if (_viewModel == null) return;

        codeElementEditView.BindViewModel(_viewModel);
        chkIsPrimary.BindViewModel(_viewModel.IsPrimaryField);
        chkIsStatic.BindViewModel(_viewModel.IsStaticField);

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e) { }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((ConstructorElementEditViewModel)(object)viewModel);
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
