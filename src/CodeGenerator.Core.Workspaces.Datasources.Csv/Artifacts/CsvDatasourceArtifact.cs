using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;

namespace CodeGenerator.Core.Workspaces.Datasources.Csv.Artifacts;

/// <summary>
/// Represents a CSV file datasource
/// </summary>
public class CsvDatasourceArtifact : DatasourceArtifact
{
    public const string TYPE_ID = "Csv";
    public const string CsvTemplateDatasourceProviderDecorator_ID = "CsvTemplateDatasourceProviderDecorator";

    public CsvDatasourceArtifact(string name, string? description = null)
        : base(name, description)
    {
        FilePath = string.Empty;
        FirstRowIsHeader = true;
        FieldDelimiter = ",";
        RowTerminator = "\\n";
    }

    public CsvDatasourceArtifact(ArtifactState state)
        : base(state)
    {
    }

    public TemplateDatasourceProviderDecorator CreateTableArtifactTemplateDatasourceProviderDecorator()
    {
        return new CsvTableTemplateDatasourceProviderDecorator(CsvTemplateDatasourceProviderDecorator_ID);
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

    protected override string IconKey => "file-text";

    /// <summary>
    /// Path to the CSV file
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

    /// <summary>
    /// Field delimiter character(s) (e.g., "," or "|" or "\t")
    /// </summary>
    public string FieldDelimiter
    {
        get => GetValue<string>(nameof(FieldDelimiter));
        set => SetValue(nameof(FieldDelimiter), value);
    }

    /// <summary>
    /// Row terminator character(s) (e.g., "\n" or "\r\n")
    /// </summary>
    public string RowTerminator
    {
        get => GetValue<string>(nameof(RowTerminator));
        set => SetValue(nameof(RowTerminator), value);
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
    /// Get the table (single table for CSV file)
    /// </summary>
    public TableArtifact? GetTable()
    {
        return Children.OfType<TableArtifact>().FirstOrDefault();
    }
}
