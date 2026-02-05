using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;

namespace CodeGenerator.Core.Workspaces.Datasources.Directory.Artifacts;

/// <summary>
/// Represents a Directory datasource that scans a folder for files and subdirectories
/// </summary>
public class DirectoryDatasourceArtifact : DatasourceArtifact
{
    public const string TYPE_ID = "Directory";
    public const string DirectoryTemplateDatasourceProviderDecorator_ID = "DirectoryTemplateDatasourceProviderDecorator";

    public DirectoryDatasourceArtifact(string name, string? description = null)
        : base(name, description)
    {
        DirectoryPath = string.Empty;
        SearchPattern = "*.*";
    }

    public DirectoryDatasourceArtifact(ArtifactState state)
        : base(state)
    {
    }

    public TemplateDatasourceProviderDecorator CreateTableArtifactTemplateDatasourceProviderDecorator()
    {
        return new DirectoryTemplateDatasourceProviderDecorator(DirectoryTemplateDatasourceProviderDecorator_ID);
    }

    public override T AddChild<T>(T child)
    {
        base.AddChild(child);
        if (child is TableArtifact tableArtifact)
        {
            // First check if it already has the decorator (from memento state restore)
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

    public override string DatasourceCategory => "File System";

    protected override string IconKey => "folder-open";

    /// <summary>
    /// Path to the directory to scan
    /// </summary>
    public string DirectoryPath
    {
        get => GetValue<string>(nameof(DirectoryPath));
        set => SetValue(nameof(DirectoryPath), value);
    }

    /// <summary>
    /// Search pattern for filtering files (e.g., "*.cs", "*.txt", "*.*")
    /// </summary>
    public string SearchPattern
    {
        get => GetValue<string>(nameof(SearchPattern)) ?? "*.*";
        set => SetValue(nameof(SearchPattern), value);
    }

    /// <summary>
    /// Whether to include subdirectories in the scan
    /// </summary>
    public bool IncludeSubdirectories
    {
        get => GetValue<bool>(nameof(IncludeSubdirectories));
        set => SetValue(nameof(IncludeSubdirectories), value);
    }

    public override async Task<bool> ValidateAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(DirectoryPath))
                return false;

            return await Task.FromResult(System.IO.Directory.Exists(DirectoryPath));
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
    /// Get the table (single table for Directory datasource)
    /// </summary>
    public TableArtifact? GetTable()
    {
        return Children.OfType<TableArtifact>().FirstOrDefault();
    }
}
