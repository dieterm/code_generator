using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Datasources.Csv.Services;
using Microsoft.Extensions.Logging;
using System.Dynamic;

namespace CodeGenerator.Core.Workspaces.Datasources.Csv.Artifacts;

public class CsvTableTemplateDatasourceProviderDecorator : TemplateDatasourceProviderDecorator
{
    public CsvTableTemplateDatasourceProviderDecorator(ArtifactDecoratorState state)
        : base(state)
    {
    }

    public CsvTableTemplateDatasourceProviderDecorator(string key)
        : base(key)
    {
    }

    public override void Attach(Artifact artifact)
    {
        if (artifact is not TableArtifact)
        {
            throw new InvalidOperationException("CsvTableTemplateDatasourceProviderDecorator can only be attached to TableArtifact.");
        }
        base.Attach(artifact);
    }

    public override string DisplayName
    {
        get
        {
            var tableArtifact = this.Artifact as TableArtifact;

            if (tableArtifact?.Parent is CsvDatasourceArtifact datasource)
            {
                return $"{datasource.Name} > {tableArtifact.Name}";
            }
            throw new InvalidOperationException("Failed to determine datasource name for CSV template datasource provider.");
        }
    }

    public override string FullPath
    {
        get
        {
            var tableArtifact = this.Artifact as TableArtifact;
            if (tableArtifact?.Parent is CsvDatasourceArtifact datasource)
            {
                return $"{datasource.Name}.{tableArtifact.Name}";
            }
            throw new InvalidOperationException("Failed to determine datasource full path for CSV template datasource provider.");
        }
    }

    public override async Task<IEnumerable<dynamic>> LoadDataAsync(ILogger Logger, string? filter, int? maxRows, CancellationToken cancellationToken)
    {
        var tableArtifact = this.Artifact as TableArtifact;
        var results = new List<dynamic>();

        if (tableArtifact?.Parent is not CsvDatasourceArtifact datasource)
        {
            Logger.LogWarning("TableArtifactData is only supported for CSV datasources");
            return results;
        }

        if (string.IsNullOrEmpty(datasource.FilePath) || !File.Exists(datasource.FilePath))
        {
            Logger.LogWarning("CSV file not found: {FilePath}", datasource.FilePath);
            return results;
        }

        Logger.LogDebug("Loading data from CSV: {FilePath}", datasource.FilePath);

        try
        {
            var schemaReader = new CsvSchemaReader();
            var data = await schemaReader.ReadCsvDataAsync(
                datasource.FilePath,
                datasource.FieldDelimiter,
                datasource.RowTerminator,
                datasource.FirstRowIsHeader,
                maxRows,
                cancellationToken);

            foreach (var row in data)
            {
                var expando = new ExpandoObject() as IDictionary<string, object?>;
                foreach (var kvp in row)
                {
                    expando[kvp.Key] = kvp.Value;
                }
                results.Add(expando);
            }

            Logger.LogInformation("Loaded {RowCount} rows from CSV {FilePath}", results.Count, datasource.FilePath);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading data from CSV {FilePath}", datasource.FilePath);
            throw;
        }

        return results;
    }
}
