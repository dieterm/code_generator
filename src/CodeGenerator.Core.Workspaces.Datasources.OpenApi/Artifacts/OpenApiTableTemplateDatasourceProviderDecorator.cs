using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using Microsoft.Extensions.Logging;
using System.Dynamic;

namespace CodeGenerator.Core.Workspaces.Datasources.OpenApi.Artifacts;

/// <summary>
/// Template datasource provider decorator for OpenAPI schemas
/// </summary>
public class OpenApiTableTemplateDatasourceProviderDecorator : TemplateDatasourceProviderDecorator
{
    public OpenApiTableTemplateDatasourceProviderDecorator(ArtifactDecoratorState state)
        : base(state)
    {
    }

    public OpenApiTableTemplateDatasourceProviderDecorator(string key)
        : base(key)
    {
    }

    public override void Attach(Artifact artifact)
    {
        if (artifact is not TableArtifact)
        {
            throw new InvalidOperationException("OpenApiTableTemplateDatasourceProviderDecorator can only be attached to TableArtifact.");
        }
        base.Attach(artifact);
    }

    public override string DisplayName
    {
        get
        {
            var tableArtifact = this.Artifact as TableArtifact;

            if (tableArtifact?.Parent is OpenApiDatasourceArtifact datasource)
            {
                return $"{datasource.Name} > {tableArtifact.Name}";
            }
            throw new InvalidOperationException("Failed to determine datasource name for OpenAPI template datasource provider.");
        }
    }

    public override string FullPath
    {
        get
        {
            var tableArtifact = this.Artifact as TableArtifact;
            if (tableArtifact?.Parent is OpenApiDatasourceArtifact datasource)
            {
                return $"{datasource.Name}.{tableArtifact.Name}";
            }
            throw new InvalidOperationException("Failed to determine datasource full path for OpenAPI template datasource provider.");
        }
    }

    public override async Task<IEnumerable<dynamic>> LoadDataAsync(ILogger Logger, string? filter, int? maxRows, CancellationToken cancellationToken)
    {
        var tableArtifact = this.Artifact as TableArtifact;
        var results = new List<dynamic>();

        if (tableArtifact?.Parent is not OpenApiDatasourceArtifact datasource)
        {
            Logger.LogWarning("TableArtifactData is only supported for OpenAPI datasources");
            return results;
        }

        // For OpenAPI schemas, we return the schema metadata as a single row
        var expando = new ExpandoObject() as IDictionary<string, object?>;
        expando["SchemaName"] = tableArtifact.Name;
        expando["SourceFile"] = datasource.FilePath;

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

        Logger.LogInformation("Loaded schema metadata for {SchemaName} from OpenAPI spec {FilePath}",
            tableArtifact.Name, datasource.FilePath);

        return await Task.FromResult<IEnumerable<dynamic>>(results);
    }
}
