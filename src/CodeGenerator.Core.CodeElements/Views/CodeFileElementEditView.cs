using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.Views;

/// <summary>
/// View for editing CodeFileElementArtifact properties
/// </summary>
public partial class CodeFileElementEditView : UserControl, IView<CodeFileElementEditViewModel>
{
    private CodeFileElementEditViewModel? _viewModel;

    public CodeFileElementEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(CodeFileElementEditViewModel? viewModel)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        _viewModel = viewModel;

        if (_viewModel == null) return;

        codeElementEditView.BindViewModel(_viewModel);
        txtFileHeader.BindViewModel(_viewModel.FileHeaderField);
        chkNullableContext.BindViewModel(_viewModel.NullableContextField);
        chkUseImplicitUsings.BindViewModel(_viewModel.UseImplicitUsingsField);
        cbxLanguage.BindViewModel(_viewModel.LanguageField);

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((CodeFileElementEditViewModel)(object)viewModel);
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
