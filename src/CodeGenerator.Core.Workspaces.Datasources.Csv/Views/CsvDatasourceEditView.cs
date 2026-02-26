using CodeGenerator.Core.Workspaces.Datasources.Csv.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.Workspaces.Datasources.Csv.Views;

/// <summary>
/// View for editing CSV datasource properties
/// </summary>
public partial class CsvDatasourceEditView : UserControl, IView<CsvDatasourceEditViewModel>
{
    private CsvDatasourceEditViewModel? _viewModel;

    public CsvDatasourceEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(CsvDatasourceEditViewModel viewModel)
    {
        _viewModel = viewModel;

        if (_viewModel == null) return;

        // Bind fields
        txtName.BindViewModel(_viewModel.NameField);
        fileField.BindViewModel(_viewModel.FilePathField);
        chkFirstRowIsHeader.BindViewModel(_viewModel.FirstRowIsHeaderField);
        txtFieldDelimiter.BindViewModel(_viewModel.FieldDelimiterField);
        txtRowTerminator.BindViewModel(_viewModel.RowTerminatorField);
        objectImportField.BindViewModel(_viewModel.ObjectImportField);
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : IViewModel
    {
        BindViewModel((CsvDatasourceEditViewModel)(object)viewModel);
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
