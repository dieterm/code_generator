using CodeGenerator.Core.Workspaces.Datasources.Excel.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.Workspaces.Datasources.Excel.Views;

/// <summary>
/// View for editing Excel datasource properties
/// </summary>
public partial class ExcelDatasourceEditView : UserControl, IView<ExcelDatasourceEditViewModel>
{
    private ExcelDatasourceEditViewModel? _viewModel;

    public ExcelDatasourceEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(ExcelDatasourceEditViewModel viewModel)
    {
        _viewModel = viewModel;

        if (_viewModel == null) return;

        // Bind fields
        txtName.BindViewModel(_viewModel.NameField);
        fileField.BindViewModel(_viewModel.FilePathField);
        chkFirstRowIsHeader.BindViewModel(_viewModel.FirstRowIsHeaderField);
        objectImportField.BindViewModel(_viewModel.ObjectImportField);
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : IViewModel
    {
        BindViewModel((ExcelDatasourceEditViewModel)(object)viewModel);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
