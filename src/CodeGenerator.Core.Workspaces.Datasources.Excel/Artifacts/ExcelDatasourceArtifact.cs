using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;

namespace CodeGenerator.Core.Workspaces.Datasources.Excel.Artifacts;

/// <summary>
/// Represents an Excel file datasource
/// </summary>
public class ExcelDatasourceArtifact : DatasourceArtifact
{
    public const string TYPE_ID = "Excel";
    public const string ExcelTemplateDatasourceProviderDecorator_ID = "ExcelTemplateDatasourceProviderDecorator";

    public ExcelDatasourceArtifact(string name, string? description = null)
        : base(name, description)
    {
        FilePath = string.Empty;
        FirstRowIsHeader = true;
    }

    public ExcelDatasourceArtifact(ArtifactState state)
        : base(state)
    {
    }

    public TemplateDatasourceProviderDecorator CreateTableArtifactTemplateDatasourceProviderDecorator()
    {
        return new ExcelTableTemplateDatasourceProviderDecorator(ExcelTemplateDatasourceProviderDecorator_ID);
    }

    public override T AddChild<T>(T child)
    {
        base.AddChild(child);
        if (child is TableArtifact tableArtifact)
        {
            // first check if it already has the decorator (from memento state restore)
            if (tableArtifact.GetTemplateDatasourceProviderDecorator() != null)
                return child;
            tableArtifact.AddDecorator(CreateTableArtifactTemplateDatasourceProviderDecorator());
        }
        return child;
    }

    public override void RemoveChild(IArtifact child)
    {
        base.RemoveChild(child);
        if (child is TableArtifact tableArtifact)
        {
            var decorator = tableArtifact.GetTemplateDatasourceProviderDecorator();
            if (decorator != null)
            {
                tableArtifact.RemoveDecorator(decorator);
            }
        }
    }

    public override string DatasourceType => TYPE_ID;

    public override string DatasourceCategory => "File";

    protected override string IconKey => "file-spreadsheet";

    /// <summary>
    /// Path to the Excel file
    /// </summary>
    public string FilePath
    {
        get => GetValue<string>(nameof(FilePath));
        set => SetValue(nameof(FilePath), value);
    }

    /// <summary>
    /// Whether the first row contains column headers
    /// </summary>
    public bool FirstRowIsHeader
    {
        get => GetValue<bool>(nameof(FirstRowIsHeader));
        set => SetValue(nameof(FirstRowIsHeader), value);
    }

    public override async Task<bool> ValidateAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(FilePath))
                return false;

            return await Task.FromResult(File.Exists(FilePath));
        }
        catch
        {
            return false;
        }
    }

    public override async Task RefreshSchemaAsync(CancellationToken cancellationToken = default)
    {
        // Schema refresh is handled separately through the edit view
        await Task.CompletedTask;
    }

    public override bool CanBeginEdit()
    {
        return true;
    }

    public override bool Validating(string newName)
    {
        return !string.IsNullOrWhiteSpace(newName);
    }

    public override void EndEdit(string oldName, string newName)
    {
        Name = newName;
    }

    /// <summary>
    /// Get all tables (sheets) in the datasource
    /// </summary>
    public IEnumerable<TableArtifact> GetTables()
    {
        return Children.OfType<TableArtifact>();
    }
}
