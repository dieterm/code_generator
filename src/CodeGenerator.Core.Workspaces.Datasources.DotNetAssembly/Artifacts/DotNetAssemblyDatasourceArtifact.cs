using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;

namespace CodeGenerator.Core.Workspaces.Datasources.DotNetAssembly.Artifacts;

/// <summary>
/// Represents a .NET Assembly datasource (DLL or EXE)
/// </summary>
public class DotNetAssemblyDatasourceArtifact : DatasourceArtifact
{
    public const string TYPE_ID = "DotNetAssembly";
    public const string DotNetAssemblyTemplateDatasourceProviderDecorator_ID = "DotNetAssemblyTemplateDatasourceProviderDecorator";

    public DotNetAssemblyDatasourceArtifact(string name, string? description = null)
        : base(name, description)
    {
        FilePath = string.Empty;
    }

    public DotNetAssemblyDatasourceArtifact(ArtifactState state)
        : base(state)
    {
    }

    public TemplateDatasourceProviderDecorator CreateTableArtifactTemplateDatasourceProviderDecorator()
    {
        return new DotNetAssemblyTableTemplateDatasourceProviderDecorator(DotNetAssemblyTemplateDatasourceProviderDecorator_ID);
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

    protected override string IconKey => "file-cog";

    /// <summary>
    /// Path to the assembly file (.dll or .exe)
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
    /// Get all types (tables) from the assembly
    /// </summary>
    public IEnumerable<TableArtifact> GetTypes()
    {
        return Children.OfType<TableArtifact>();
    }
}
