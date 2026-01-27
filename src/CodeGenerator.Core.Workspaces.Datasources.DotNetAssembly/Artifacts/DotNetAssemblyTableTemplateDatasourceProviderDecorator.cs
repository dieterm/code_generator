using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using Microsoft.Extensions.Logging;
using System.Dynamic;

namespace CodeGenerator.Core.Workspaces.Datasources.DotNetAssembly.Artifacts;

/// <summary>
/// Template datasource provider decorator for .NET Assembly types
/// </summary>
public class DotNetAssemblyTableTemplateDatasourceProviderDecorator : TemplateDatasourceProviderDecorator
{
    public DotNetAssemblyTableTemplateDatasourceProviderDecorator(ArtifactDecoratorState state)
        : base(state)
    {
    }

    public DotNetAssemblyTableTemplateDatasourceProviderDecorator(string key)
        : base(key)
    {
    }

    public override void Attach(Artifact artifact)
    {
        if (artifact is not TableArtifact)
        {
            throw new InvalidOperationException("DotNetAssemblyTableTemplateDatasourceProviderDecorator can only be attached to TableArtifact.");
        }
        base.Attach(artifact);
    }

    public override string DisplayName
    {
        get
        {
            var tableArtifact = this.Artifact as TableArtifact;

            if (tableArtifact?.Parent is DotNetAssemblyDatasourceArtifact datasource)
            {
                return $"{datasource.Name} > {tableArtifact.Name}";
            }
            throw new InvalidOperationException("Failed to determine datasource name for .NET Assembly template datasource provider.");
        }
    }

    public override string FullPath
    {
        get
        {
            var tableArtifact = this.Artifact as TableArtifact;
            if (tableArtifact?.Parent is DotNetAssemblyDatasourceArtifact datasource)
            {
                return $"{datasource.Name}.{tableArtifact.Name}";
            }
            throw new InvalidOperationException("Failed to determine datasource full path for .NET Assembly template datasource provider.");
        }
    }

    public override async Task<IEnumerable<dynamic>> LoadDataAsync(ILogger Logger, string? filter, int? maxRows, CancellationToken cancellationToken)
    {
        var tableArtifact = this.Artifact as TableArtifact;
        var results = new List<dynamic>();

        if (tableArtifact?.Parent is not DotNetAssemblyDatasourceArtifact datasource)
        {
            Logger.LogWarning("TableArtifactData is only supported for .NET Assembly datasources");
            return results;
        }

        // For assembly types, we return the type metadata as a single row
        var expando = new ExpandoObject() as IDictionary<string, object?>;
        expando["TypeName"] = tableArtifact.Name;
        expando["Namespace"] = tableArtifact.Schema;
        expando["FullName"] = string.IsNullOrEmpty(tableArtifact.Schema) 
            ? tableArtifact.Name 
            : $"{tableArtifact.Schema}.{tableArtifact.Name}";
        expando["AssemblyPath"] = datasource.FilePath;

        // Add properties as a collection
        var properties = tableArtifact.GetColumns()
            .Select(c => new Dictionary<string, object?>
            {
                ["Name"] = c.Name,
                ["DataType"] = c.DataType,
                ["IsNullable"] = c.IsNullable
            })
            .ToList();
        expando["Properties"] = properties;

        results.Add(expando);

        Logger.LogInformation("Loaded type metadata for {TypeName} from assembly {FilePath}", 
            tableArtifact.Name, datasource.FilePath);

        return await Task.FromResult<IEnumerable<dynamic>>(results);
    }
}
