using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
//using CodeGenerator.Core.Workspaces.Models;
using CodeGenerator.Core.Workspaces.Settings;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CodeGenerator.Core.Workspaces.Services
{
    /// <summary>
    /// Service for loading and saving workspace files
    /// </summary>
    public class WorkspaceFileService
    {
        private readonly ILogger<WorkspaceFileService> _logger;
        private readonly IDatasourceFactory _datasourceFactory;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new DictionaryStringObjectJsonConverter() }
        };

        public const string WorkspaceFileExtension = ".codegenerator";

        public WorkspaceFileService(ILogger<WorkspaceFileService> logger, IDatasourceFactory datasourceFactory)
        {
            _logger = logger;
            _datasourceFactory = datasourceFactory;
        }

        /// <summary>
        /// Load a workspace from a .codegenerator file
        /// </summary>
        public async Task<WorkspaceArtifact> LoadAsync(string filePath, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Workspace file not found: {filePath}");
            }

            _logger.LogInformation("Loading workspace from {FilePath}", filePath);

            var json = await File.ReadAllTextAsync(filePath, cancellationToken);
            
            var workspaceState = JsonSerializer.Deserialize<ArtifactState>(json, JsonOptions)
                ?? throw new InvalidOperationException("Failed to deserialize workspace state");
            var workspaceArtifact = (WorkspaceArtifact)ArtifactFactory.CreateArtifact(workspaceState);
            
            _logger.LogInformation("Loaded workspace '{Name}' with {Count} datasources", 
                workspaceArtifact.Name, workspaceArtifact.Datasources.GetDatasources().Count());
            
            return workspaceArtifact;

            //var workspaceFile = JsonSerializer.Deserialize<WorkspaceFile>(json, JsonOptions)
            //    ?? throw new InvalidOperationException("Failed to deserialize workspace file");

            //var workspace = new WorkspaceArtifact(workspaceFile.Name)
            //{
            //    WorkspaceFilePath = filePath,
            //    RootNamespace = workspaceFile.RootNamespace,
            //    DefaultOutputDirectory = workspaceFile.DefaultOutputDirectory,
            //    DefaultTargetFramework = workspaceFile.DefaultTargetFramework,
            //    DefaultLanguage = workspaceFile.DefaultLanguage
            //};

            //// Load datasources
            //foreach (var datasourceDef in workspaceFile.Datasources)
            //{
            //    try
            //    {
            //        var datasource = _datasourceFactory.Create(datasourceDef);
            //        if (datasource != null)
            //        {
            //            workspace.Datasources.AddDatasource(datasource);

            //            // Load tables
            //            foreach (var tableDef in datasourceDef.Tables)
            //            {
            //                var table = LoadTable(tableDef);
            //                datasource.AddChild(table);
            //            }

            //            // Load views
            //            foreach (var viewDef in datasourceDef.Views)
            //            {
            //                var view = LoadView(viewDef);
            //                datasource.AddChild(view);
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogWarning(ex, "Failed to load datasource {Name} ({Type})", datasourceDef.Name, datasourceDef.Type);
            //    }
            //}

            //_logger.LogInformation("Loaded workspace '{Name}' with {Count} datasources", 
            //    workspace.Name, workspace.Datasources.GetDatasources().Count());

            //return workspace;
        }

        /// <summary>
        /// Load a table from its definition
        /// </summary>
        //private TableArtifact LoadTable(TableDefinition tableDef)
        //{
        //    var table = new TableArtifact(tableDef.Name, tableDef.Schema);

        //    // Load columns
        //    foreach (var columnDef in tableDef.Columns)
        //    {
        //        var column = LoadColumn(columnDef);
        //        table.AddChild(column);
        //    }

        //    // Load indexes
        //    foreach (var indexDef in tableDef.Indexes)
        //    {
        //        var index = LoadIndex(indexDef);
        //        table.AddChild(index);
        //    }

        //    // Load metadata (decorators, etc.)
        //    if (tableDef.Metadata != null)
        //    {
        //        LoadMetadata(table, tableDef.Metadata);
        //    }

        //    return table;
        //}

        /// <summary>
        /// Load a view from its definition
        /// </summary>
        //private ViewArtifact LoadView(ViewDefinition viewDef)
        //{
        //    var view = new ViewArtifact(viewDef.Name, viewDef.Schema)
        //    {
        //        Definition = viewDef.Definition ?? string.Empty
        //    };

        //    // Load columns
        //    foreach (var columnDef in viewDef.Columns)
        //    {
        //        var column = LoadColumn(columnDef);
        //        view.AddChild(column);
        //    }

        //    // Load metadata (decorators, etc.)
        //    if (viewDef.Metadata != null)
        //    {
        //        LoadMetadata(view, viewDef.Metadata);
        //    }

        //    return view;
        //}

        /// <summary>
        /// Load a column from its definition
        /// </summary>
        //private ColumnArtifact LoadColumn(ColumnDefinition columnDef)
        //{
        //    var column = new ColumnArtifact(columnDef.Name, columnDef.DataType, columnDef.IsNullable)
        //    {
        //        IsPrimaryKey = columnDef.IsPrimaryKey,
        //        IsAutoIncrement = columnDef.IsAutoIncrement,
        //        MaxLength = columnDef.MaxLength,
        //        Precision = columnDef.Precision,
        //        Scale = columnDef.Scale,
        //        DefaultValue = columnDef.DefaultValue,
        //        ForeignKeyTable = columnDef.ForeignKeyTable,
        //        ForeignKeyColumn = columnDef.ForeignKeyColumn
        //    };

        //    // Load metadata (decorators, etc.)
        //    if (columnDef.Metadata != null)
        //    {
        //        LoadMetadata(column, columnDef.Metadata);
        //    }

        //    return column;
        //}

        /// <summary>
        /// Load an index from its definition
        /// </summary>
        //private IndexArtifact LoadIndex(IndexDefinition indexDef)
        //{
        //    var index = new IndexArtifact(indexDef.Name, indexDef.IsUnique)
        //    {
        //        IsClustered = indexDef.IsClustered
        //    };

        //    // Load column names
        //    foreach (var columnName in indexDef.ColumnNames)
        //    {
        //        index.AddColumn(columnName);
        //    }

        //    // Load metadata (decorators, etc.)
        //    if (indexDef.Metadata != null)
        //    {
        //        LoadMetadata(index, indexDef.Metadata);
        //    }

        //    return index;
        //}

        /// <summary>
        /// Load metadata (decorators, custom attributes, etc.) onto an artifact
        /// </summary>
        //private void LoadMetadata(Core.Artifacts.Artifact artifact, Dictionary<string, object> metadata)
        //{
        //TODO: Implement decorator loading based on metadata
        //     This will be used to restore decorators like ExistingMysqlTableDecorator
        //     For now, we just skip this - decorators can be added in future enhancement
        //    foreach (var key in metadata.Keys)
        //    {
        //    Example: if key is "decorators", we could restore decorators here
        //        if (key == "decorators" && metadata[key] is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Array)
        //        {
        //            foreach (var decoratorElement in jsonElement.EnumerateArray())
        //            {
        //                Logic to recreate and add decorators based on saved state
        //                 This is a placeholder and would need actual implementation
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Save a workspace to a .codegenerator file
        /// </summary>
        public async Task SaveAsync(WorkspaceArtifact workspace, string? filePath = null, CancellationToken cancellationToken = default)
        {
            var targetPath = filePath ?? workspace.WorkspaceFilePath;
            
            if (string.IsNullOrEmpty(targetPath))
            {
                throw new InvalidOperationException("Workspace file path is not set");
            }

            _logger.LogInformation("Saving workspace to {FilePath}", targetPath);

            //var workspaceFile = new WorkspaceFile
            //{
            //    Name = workspace.Name,
            //    RootNamespace = workspace.RootNamespace,
            //    DefaultOutputDirectory = workspace.DefaultOutputDirectory,
            //    DefaultTargetFramework = workspace.DefaultTargetFramework,
            //    DefaultLanguage = workspace.DefaultLanguage
            //};

            //// Save datasources
            //foreach (var datasource in workspace.Datasources.GetDatasources())
            //{
            //    var definition = _datasourceFactory.CreateDefinition(datasource);
            //    if (definition != null)
            //    {
            //        // Save tables
            //        var tables = datasource.Children.OfType<TableArtifact>();
            //        foreach (var table in tables)
            //        {
            //            var tableDef = SaveTable(table);
            //            definition.Tables.Add(tableDef);
            //        }

            //        // Save views
            //        var views = datasource.Children.OfType<ViewArtifact>();
            //        foreach (var view in views)
            //        {
            //            var viewDef = SaveView(view);
            //            definition.Views.Add(viewDef);
            //        }

            //        workspaceFile.Datasources.Add(definition);
            //    }
            //}
            var workspaceState = (ArtifactState)workspace.CaptureState();
            var json = JsonSerializer.Serialize(workspaceState, JsonOptions);
            await File.WriteAllTextAsync(targetPath, json, cancellationToken);

            workspace.WorkspaceFilePath = targetPath;

            _logger.LogInformation("Saved workspace '{Name}'", workspace.Name);
        }

        /// <summary>
        /// Save a table to its definition
        /// </summary>
        //private TableDefinition SaveTable(TableArtifact table)
        //{
        //    var tableDef = new TableDefinition
        //    {
        //        Name = table.Name,
        //        Schema = table.Schema
        //    };

        //    // Save columns
        //    foreach (var column in table.Children.OfType<ColumnArtifact>())
        //    {
        //        var columnDef = SaveColumn(column);
        //        tableDef.Columns.Add(columnDef);
        //    }

        //    // Save indexes
        //    foreach (var index in table.Children.OfType<IndexArtifact>())
        //    {
        //        var indexDef = SaveIndex(index);
        //        tableDef.Indexes.Add(indexDef);
        //    }

        //    // Save metadata (decorators, etc.)
        //    tableDef.Metadata = SaveMetadata(table);

        //    return tableDef;
        //}

        /// <summary>
        /// Save a view to its definition
        /// </summary>
        //private ViewDefinition SaveView(ViewArtifact view)
        //{
        //    var viewDef = new ViewDefinition
        //    {
        //        Name = view.Name,
        //        Schema = view.Schema,
        //        Definition = view.Definition
        //    };

        //    // Save columns
        //    foreach (var column in view.Children.OfType<ColumnArtifact>())
        //    {
        //        var columnDef = SaveColumn(column);
        //        viewDef.Columns.Add(columnDef);
        //    }

        //    // Save metadata (decorators, etc.)
        //    viewDef.Metadata = SaveMetadata(view);

        //    return viewDef;
        //}

        /// <summary>
        /// Save a column to its definition
        /// </summary>
        //private ColumnDefinition SaveColumn(ColumnArtifact column)
        //{
        //    var columnDef = new ColumnDefinition
        //    {
        //        Name = column.Name,
        //        DataType = column.DataType,
        //        IsNullable = column.IsNullable,
        //        IsPrimaryKey = column.IsPrimaryKey,
        //        IsAutoIncrement = column.IsAutoIncrement,
        //        MaxLength = column.MaxLength,
        //        Precision = column.Precision,
        //        Scale = column.Scale,
        //        DefaultValue = column.DefaultValue,
        //        ForeignKeyTable = column.ForeignKeyTable,
        //        ForeignKeyColumn = column.ForeignKeyColumn
        //    };

        //    // Save metadata (decorators, etc.)
        //    columnDef.Metadata = SaveMetadata(column);

        //    return columnDef;
        //}

        /// <summary>
        /// Save an index to its definition
        /// </summary>
        //private IndexDefinition SaveIndex(IndexArtifact index)
        //{
        //    var indexDef = new IndexDefinition
        //    {
        //        Name = index.Name,
        //        IsUnique = index.IsUnique,
        //        IsClustered = index.IsClustered,
        //        ColumnNames = new List<string>(index.ColumnNames)
        //    };

        //    // Save metadata (decorators, etc.)
        //    indexDef.Metadata = SaveMetadata(index);

        //    return indexDef;
        //}

        /// <summary>
        /// Save metadata (decorators, custom attributes, etc.) from an artifact
        /// </summary>
        //private Dictionary<string, object>? SaveMetadata(Core.Artifacts.Artifact artifact)
        //{
        //    // TODO: Implement decorator saving as metadata
        //    // This will be used to persist decorators like ExistingMysqlTableDecorator
        //    // For now, we just return null - decorators can be added in future enhancement
            
        //    if (!artifact.Decorators.Any())
        //    {
        //        return null;
        //    }

        //    var metadata = new Dictionary<string, object>();
            
        //    // Save decorator keys for future reference
        //    metadata["decorators"] = artifact.Decorators.Select(d => d.CaptureState()).ToList();

        //    return metadata;
        //}

        /// <summary>
        /// Create a new workspace in a directory
        /// </summary>
        public async Task<WorkspaceArtifact> CreateNewAsync(string directory, string name = "Workspace", CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var filePath = Path.Combine(directory, WorkspaceFileExtension);
            // get default settings for new workspace
            var workspaceSettings = WorkspaceSettings.Instance;
            var workspace = new WorkspaceArtifact(name)
            {
                WorkspaceFilePath = filePath,
                RootNamespace = workspaceSettings.RootNamespace,
                DefaultOutputDirectory = workspaceSettings.DefaultOutputDirectory,
                DefaultTargetFramework = workspaceSettings.DefaultTargetFramework,
                DefaultLanguage = workspaceSettings.DefaultLanguage
            };
            var datasourcesContainer= new DatasourcesContainerArtifact();
            workspace.AddChild(datasourcesContainer);

            await SaveAsync(workspace, filePath, cancellationToken);

            _logger.LogInformation("Created new workspace '{Name}' at {Directory}", name, directory);

            return workspace;
        }

        /// <summary>
        /// Check if a directory contains a workspace
        /// </summary>
        public bool IsWorkspaceDirectory(string directory)
        {
            var workspaceFile = GetWorkspaceFilePath(directory);
            return File.Exists(workspaceFile);
        }

        /// <summary>
        /// Get the workspace file path for a directory
        /// </summary>
        public string GetWorkspaceFilePath(string directory)
        {
            return Path.Combine(directory, WorkspaceFileExtension);
        }
    }
}
