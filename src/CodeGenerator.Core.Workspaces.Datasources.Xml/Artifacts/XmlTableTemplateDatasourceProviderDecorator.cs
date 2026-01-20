using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Datasources.Xml.Services;
using Microsoft.Extensions.Logging;
using System.Dynamic;

namespace CodeGenerator.Core.Workspaces.Datasources.Xml.Artifacts;

public class XmlTableTemplateDatasourceProviderDecorator : TemplateDatasourceProviderDecorator
{
    public XmlTableTemplateDatasourceProviderDecorator(ArtifactDecoratorState state)
        : base(state)
    {
    }

    public XmlTableTemplateDatasourceProviderDecorator(string key)
        : base(key)
    {
    }

    public override void Attach(Artifact artifact)
    {
        if (artifact is not TableArtifact)
        {
            throw new InvalidOperationException("XmlTableTemplateDatasourceProviderDecorator can only be attached to TableArtifact.");
        }
        base.Attach(artifact);
    }

    public override string DisplayName
    {
        get
        {
            var tableArtifact = this.Artifact as TableArtifact;

            if (tableArtifact?.Parent is XmlDatasourceArtifact datasource)
            {
                return $"{datasource.Name} > {tableArtifact.Name}";
            }
            throw new InvalidOperationException("Failed to determine datasource name for XML template datasource provider.");
        }
    }

    public override string FullPath
    {
        get
        {
            var tableArtifact = this.Artifact as TableArtifact;
            if (tableArtifact?.Parent is XmlDatasourceArtifact datasource)
            {
                return $"{datasource.Name}.{tableArtifact.Name}";
            }
            throw new InvalidOperationException("Failed to determine datasource full path for XML template datasource provider.");
        }
    }

    public override async Task<IEnumerable<dynamic>> LoadDataAsync(ILogger Logger, string? filter, int? maxRows, CancellationToken cancellationToken)
    {
        var tableArtifact = this.Artifact as TableArtifact;
        var results = new List<dynamic>();

        if (tableArtifact?.Parent is not XmlDatasourceArtifact datasource)
        {
            Logger.LogWarning("TableArtifactData is only supported for XML datasources");
            return results;
        }

        if (string.IsNullOrEmpty(datasource.FilePath) || !File.Exists(datasource.FilePath))
        {
            Logger.LogWarning("XML file not found: {FilePath}", datasource.FilePath);
            return results;
        }

        Logger.LogDebug("Loading data from XML: {FilePath}", datasource.FilePath);

        try
        {
            var schemaReader = new XmlSchemaReader();
            var data = await schemaReader.ReadXmlDataAsync(
                datasource.FilePath,
                datasource.RootElementPath,
                maxRows,
                cancellationToken);

            foreach (var row in data)
            {
                var expando = new ExpandoObject() as IDictionary<string, object?>;
                foreach (var kvp in row)
                {
                    // Convert nested dictionaries and lists to ExpandoObject for template access
                    expando[kvp.Key] = ConvertToExpandoIfNeeded(kvp.Value);
                }
                results.Add(expando);
            }

            Logger.LogInformation("Loaded {RowCount} items from XML {FilePath}", results.Count, datasource.FilePath);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading data from XML {FilePath}", datasource.FilePath);
            throw;
        }

        return results;
    }

    /// <summary>
    /// Convert nested dictionaries and lists to ExpandoObjects for template access
    /// </summary>
    private object? ConvertToExpandoIfNeeded(object? value)
    {
        if (value is Dictionary<string, object?> dict)
        {
            var expando = new ExpandoObject() as IDictionary<string, object?>;
            foreach (var kvp in dict)
            {
                expando[kvp.Key] = ConvertToExpandoIfNeeded(kvp.Value);
            }
            return expando;
        }
        else if (value is List<object?> list)
        {
            return list.Select(ConvertToExpandoIfNeeded).ToList();
        }
        
        return value;
    }
}
