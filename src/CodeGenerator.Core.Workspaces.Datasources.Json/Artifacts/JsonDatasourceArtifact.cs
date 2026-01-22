using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;

namespace CodeGenerator.Core.Workspaces.Datasources.Json.Artifacts;

/// <summary>
/// Represents a JSON file datasource
/// </summary>
public class JsonDatasourceArtifact : DatasourceArtifact
{
    public const string TYPE_ID = "Json";
    public const string JsonTemplateDatasourceProviderDecorator_ID = "JsonTemplateDatasourceProviderDecorator";

    public JsonDatasourceArtifact(string name, string? description = null)
        : base(name, description)
    {
        FilePath = string.Empty;
    }

    public JsonDatasourceArtifact(ArtifactState state)
        : base(state)
    {
    }

    public TemplateDatasourceProviderDecorator CreateTableArtifactTemplateDatasourceProviderDecorator()
    {
        return new JsonTableTemplateDatasourceProviderDecorator(JsonTemplateDatasourceProviderDecorator_ID);
    }

    public override void AddChild(IArtifact child)
    {
        base.AddChild(child);
        if (child is TableArtifact tableArtifact)
        {
            // first check if it already has the decorator (from memento state restore)
            if (tableArtifact.GetTemplateDatasourceProviderDecorator() != null)
                return;
            tableArtifact.AddDecorator(CreateTableArtifactTemplateDatasourceProviderDecorator());
        }
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

    protected override string IconKey => "file-braces";

    /// <summary>
    /// Path to the JSON file
    /// </summary>
    public string FilePath
    {
        get => GetValue<string>(nameof(FilePath));
        set => SetValue(nameof(FilePath), value);
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
    /// Get the table (single table for JSON file)
    /// </summary>
    public TableArtifact? GetTable()
    {
        return Children.OfType<TableArtifact>().FirstOrDefault();
    }
}
