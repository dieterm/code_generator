using CodeGenerator.Core.Workspaces.Datasources.Json.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.Workspaces.Datasources.Json.Views;

/// <summary>
/// View for editing JSON datasource properties
/// </summary>
public partial class JsonDatasourceEditView : UserControl, IView<JsonDatasourceEditViewModel>
{
    private JsonDatasourceEditViewModel? _viewModel;

    public JsonDatasourceEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(JsonDatasourceEditViewModel viewModel)
    {
        _viewModel = viewModel;

        if (_viewModel == null) return;

        // Bind fields
        txtName.BindViewModel(_viewModel.NameField);
        fileField.BindViewModel(_viewModel.FilePathField);
        objectImportField.BindViewModel(_viewModel.ObjectImportField);
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : IViewModel
    {
        BindViewModel((JsonDatasourceEditViewModel)(object)viewModel);
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
