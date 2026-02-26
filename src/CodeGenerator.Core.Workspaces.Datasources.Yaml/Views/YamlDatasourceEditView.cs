using CodeGenerator.Core.Workspaces.Datasources.Yaml.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.Workspaces.Datasources.Yaml.Views;

/// <summary>
/// View for editing YAML datasource properties
/// </summary>
public partial class YamlDatasourceEditView : UserControl, IView<YamlDatasourceEditViewModel>
{
    private YamlDatasourceEditViewModel? _viewModel;

    public YamlDatasourceEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(YamlDatasourceEditViewModel viewModel)
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
        BindViewModel((YamlDatasourceEditViewModel)(object)viewModel);
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
