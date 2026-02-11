using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.Views;

public partial class PropertyElementEditView : UserControl, IView<PropertyElementEditViewModel>
{
    private PropertyElementEditViewModel? _viewModel;

    public PropertyElementEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(PropertyElementEditViewModel? viewModel)
    {
        if (_viewModel != null)
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;

        _viewModel = viewModel;
        if (_viewModel == null) return;

        codeElementEditView.BindViewModel(_viewModel);
        txtTypeName.BindViewModel(_viewModel.TypeNameField);
        chkHasGetter.BindViewModel(_viewModel.HasGetterField);
        chkHasSetter.BindViewModel(_viewModel.HasSetterField);
        chkIsInitOnly.BindViewModel(_viewModel.IsInitOnlyField);
        chkIsAutoImplemented.BindViewModel(_viewModel.IsAutoImplementedField);
        txtInitialValue.BindViewModel(_viewModel.InitialValueField);

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e) { }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((PropertyElementEditViewModel)(object)viewModel);
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
