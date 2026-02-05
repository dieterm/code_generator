using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Datasources.Directory.Models;
using CodeGenerator.Core.Workspaces.Datasources.Directory.Services;
using Microsoft.Extensions.Logging;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;
using System.Dynamic;

namespace CodeGenerator.Core.Workspaces.Datasources.Directory.Artifacts;

/// <summary>
/// Decorator that provides template data access for Directory datasources
/// </summary>
public class DirectoryTemplateDatasourceProviderDecorator : TemplateDatasourceProviderDecorator
{
    public DirectoryTemplateDatasourceProviderDecorator(ArtifactDecoratorState state)
        : base(state)
    {
    }

    public DirectoryTemplateDatasourceProviderDecorator(string key)
        : base(key)
    {
    }

    public override void Attach(Artifact artifact)
    {
        if (artifact is not TableArtifact)
        {
            throw new InvalidOperationException("DirectoryTemplateDatasourceProviderDecorator can only be attached to TableArtifact.");
        }
        base.Attach(artifact);
    }

    public override string DisplayName
    {
        get
        {
            var tableArtifact = this.Artifact as TableArtifact;

            if (tableArtifact?.Parent is DirectoryDatasourceArtifact datasource)
            {
                return $"{datasource.Name} > {tableArtifact.Name}";
            }
            throw new InvalidOperationException("Failed to determine datasource name for Directory template datasource provider.");
        }
    }

    public override string FullPath
    {
        get
        {
            var tableArtifact = this.Artifact as TableArtifact;
            if (tableArtifact?.Parent is DirectoryDatasourceArtifact datasource)
            {
                return $"{datasource.Name}.{tableArtifact.Name}";
            }
            throw new InvalidOperationException("Failed to determine datasource full path for Directory template datasource provider.");
        }
    }

    public override async Task<IEnumerable<dynamic>> LoadDataAsync(ILogger logger, string? filter, int? maxRows, CancellationToken cancellationToken)
    {
        var tableArtifact = this.Artifact as TableArtifact;
        var results = new List<dynamic>();

        if (tableArtifact?.Parent is not DirectoryDatasourceArtifact datasource)
        {
            logger.LogWarning("DirectoryTemplateDatasourceProviderDecorator is only supported for Directory datasources");
            return results;
        }

        if (string.IsNullOrEmpty(datasource.DirectoryPath) || !System.IO.Directory.Exists(datasource.DirectoryPath))
        {
            logger.LogWarning("Directory not found: {DirectoryPath}", datasource.DirectoryPath);
            return results;
        }

        logger.LogDebug("Loading data from Directory: {DirectoryPath}", datasource.DirectoryPath);

        try
        {
            var schemaReader = new DirectorySchemaReader();
            var directoryModel = await schemaReader.LoadDirectoryModelAsync(
                datasource.DirectoryPath,
                datasource.SearchPattern,
                datasource.IncludeSubdirectories,
                cancellationToken);

            if (directoryModel != null)
            {
                // Return the root directory as a single item (the root directory model)
                var scriptObject = ConvertDirectoryModelToScriptObject(directoryModel);
                results.Add(scriptObject);
            }

            logger.LogInformation("Loaded directory data from {DirectoryPath} with {FileCount} files", 
                datasource.DirectoryPath, directoryModel?.Files.Count ?? 0);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading data from Directory {DirectoryPath}", datasource.DirectoryPath);
            throw;
        }

        return results;
    }

    /// <summary>
    /// Convert a DirectoryModel to a ScriptObject for Scriban template access
    /// </summary>
    private ScriptObject ConvertDirectoryModelToScriptObject(DirectoryModel model)
    {
        var scriptObject = new ScriptObject();

        scriptObject["name"] = model.Name;
        scriptObject["full_path"] = model.FullPath;
        scriptObject["relative_path"] = model.RelativePath;
        scriptObject["parent_directory"] = model.ParentDirectory;
        scriptObject["creation_date"] = model.CreationDate;
        scriptObject["modified_date"] = model.ModifiedDate;
        scriptObject["is_hidden"] = model.IsHidden;
        scriptObject["is_system"] = model.IsSystem;
        scriptObject["attributes"] = model.Attributes;
        scriptObject["size"] = model.Size;
        scriptObject["file_count"] = model.FileCount;
        scriptObject["directory_count"] = model.DirectoryCount;

        // Convert files list - use LazyFileScriptObject for lazy content loading
        scriptObject["files"] = model.Files.Select(f => new LazyFileScriptObject(f)).ToList();

        // Convert directories list recursively
        scriptObject["directories"] = model.Directories.Select(ConvertDirectoryModelToScriptObject).ToList();

        return scriptObject;
    }
}

/// <summary>
/// ScriptObject that provides lazy loading of file content for Scriban templates.
/// Content is only read from disk when the content property is accessed.
/// </summary>
public class LazyFileScriptObject : ScriptObject
{
    private readonly FileModel _model;
    private string? _content;
    private bool _contentLoaded;

    public LazyFileScriptObject(FileModel model)
    {
        _model = model;

        // Set all properties except Content
        this["title"] = model.Title;
        this["extension"] = model.Extension;
        this["name"] = model.Name;
        this["full_path"] = model.FullPath;
        this["directory"] = model.Directory;
        this["relative_path"] = model.RelativePath;
        this["size"] = model.Size;
        this["creation_date"] = model.CreationDate;
        this["modified_date"] = model.ModifiedDate;
        this["last_access_date"] = model.LastAccessDate;
        this["is_read_only"] = model.IsReadOnly;
        this["is_hidden"] = model.IsHidden;
        this["is_system"] = model.IsSystem;
        this["attributes"] = model.Attributes;
    }

    public override bool TryGetValue(TemplateContext context, SourceSpan span, string member, out object? value)
    {
        // Intercept "content" to provide lazy loading
        if (member == "content")
        {
            value = GetContent();
            return true;
        }

        return base.TryGetValue(context, span, member, out value);
    }

    private string GetContent()
    {
        if (!_contentLoaded)
        {
            _contentLoaded = true;
            try
            {
                if (File.Exists(_model.FullPath))
                {
                    _content = File.ReadAllText(_model.FullPath);
                }
            }
            catch
            {
                _content = string.Empty;
            }
        }
        return _content ?? string.Empty;
    }
}
