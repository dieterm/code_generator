using CodeGenerator.Core.CodeElements.ViewModels.CodeElements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.Views;

public partial class TypeReferenceEditView : UserControl, IView<TypeReferenceEditViewModel>
{
    private TypeReferenceEditViewModel? _viewModel;

    public TypeReferenceEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(TypeReferenceEditViewModel? viewModel)
    {
        if (_viewModel != null)
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;

        _viewModel = viewModel;
        if (_viewModel == null) return;

        txtTypeName.BindViewModel(_viewModel.TypeNameField);
        txtNamespace.BindViewModel(_viewModel.NamespaceField);
        chkIsNullable.BindViewModel(_viewModel.IsNullableField);
        chkIsArray.BindViewModel(_viewModel.IsArrayField);
        numArrayRank.BindViewModel(_viewModel.ArrayRankField);

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e) { }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((TypeReferenceEditViewModel)(object)viewModel);
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
