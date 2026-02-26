using CodeGenerator.Core.Workspaces.Datasources.Directory.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.Workspaces.Datasources.Directory.Views;

/// <summary>
/// View for editing Directory datasource properties
/// </summary>
public partial class DirectoryDatasourceEditView : UserControl, IView<DirectoryDatasourceEditViewModel>
{
    private DirectoryDatasourceEditViewModel? _viewModel;

    public DirectoryDatasourceEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(DirectoryDatasourceEditViewModel viewModel)
    {
        _viewModel = viewModel;

        if (_viewModel == null) return;

        // Bind fields
        txtName.BindViewModel(_viewModel.NameField);
        folderField.BindViewModel(_viewModel.DirectoryPathField);
        txtSearchPattern.BindViewModel(_viewModel.SearchPatternField);
        chkIncludeSubdirectories.BindViewModel(_viewModel.IncludeSubdirectoriesField);
        objectImportField.BindViewModel(_viewModel.ObjectImportField);
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : IViewModel
    {
        BindViewModel((DirectoryDatasourceEditViewModel)(object)viewModel);
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
