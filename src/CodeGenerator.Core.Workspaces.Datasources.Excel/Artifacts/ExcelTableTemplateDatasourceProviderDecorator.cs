using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Datasources.Excel.Services;
using Microsoft.Extensions.Logging;
using System.Dynamic;

namespace CodeGenerator.Core.Workspaces.Datasources.Excel.Artifacts;

public class ExcelTableTemplateDatasourceProviderDecorator : TemplateDatasourceProviderDecorator
{
    public ExcelTableTemplateDatasourceProviderDecorator(ArtifactDecoratorState state)
        : base(state)
    {
    }

    public ExcelTableTemplateDatasourceProviderDecorator(string key)
        : base(key)
    {
    }

    public override void Attach(Artifact artifact)
    {
        if (artifact is not TableArtifact)
        {
            throw new InvalidOperationException("ExcelTableTemplateDatasourceProviderDecorator can only be attached to TableArtifact.");
        }
        base.Attach(artifact);
    }

    public override string DisplayName
    {
        get
        {
            var tableArtifact = this.Artifact as TableArtifact;

            if (tableArtifact?.Parent is ExcelDatasourceArtifact datasource)
            {
                return $"{datasource.Name} > {tableArtifact.Name}";
            }
            throw new InvalidOperationException("Failed to determine datasource name for Excel template datasource provider.");
        }
    }

    public override string FullPath
    {
        get
        {
            var tableArtifact = this.Artifact as TableArtifact;
            if (tableArtifact?.Parent is ExcelDatasourceArtifact datasource)
            {
                return $"{datasource.Name}.{tableArtifact.Name}";
            }
            throw new InvalidOperationException("Failed to determine datasource full path for Excel template datasource provider.");
        }
    }

    public override async Task<IEnumerable<dynamic>> LoadDataAsync(ILogger Logger, string? filter, int? maxRows, CancellationToken cancellationToken)
    {
        var tableArtifact = this.Artifact as TableArtifact;
        var results = new List<dynamic>();

        if (tableArtifact?.Parent is not ExcelDatasourceArtifact datasource)
        {
            Logger.LogWarning("TableArtifactData is only supported for Excel datasources");
            return results;
        }

        if (string.IsNullOrEmpty(datasource.FilePath) || !File.Exists(datasource.FilePath))
        {
            Logger.LogWarning("Excel file not found: {FilePath}", datasource.FilePath);
            return results;
        }

        Logger.LogDebug("Loading data from sheet: {SheetName}", tableArtifact.Name);

        try
        {
            var schemaReader = new ExcelSchemaReader();
            var data = await schemaReader.ReadSheetDataAsync(
                datasource.FilePath,
                tableArtifact.Name,
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

            Logger.LogInformation("Loaded {RowCount} rows from sheet {SheetName}", results.Count, tableArtifact.Name);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading data from sheet {SheetName}", tableArtifact.Name);
            throw;
        }

        return results;
    }
}
