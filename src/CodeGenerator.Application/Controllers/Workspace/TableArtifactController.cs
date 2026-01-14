using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Settings;
using CodeGenerator.Domain.Databases.RelationalDatabases;
using CodeGenerator.TemplateEngines.Scriban;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Controller for TableArtifact
    /// </summary>
    public class TableArtifactController : ArtifactControllerBase<TableArtifact>
    {
        private TableEditViewModel? _editViewModel;
        private readonly TemplateEngineManager _templateEngineManager;
        private readonly IWindowManagerService _windowManagerService;

        public TableArtifactController(
            WorkspaceController workspaceController,
            TemplateEngineManager templateEngineManager,
            IWindowManagerService windowManagerService,
            ILogger<TableArtifactController> logger)
            : base(workspaceController, logger)
        {
            _templateEngineManager = templateEngineManager;
            _windowManagerService = windowManagerService;
        }

        protected override IEnumerable<WorkspaceCommand> GetCommands(TableArtifact artifact)
        {
            var commands = new List<WorkspaceCommand>();
            
            // Generate Script command
            var generateScriptCommand = new WorkspaceCommand
            {
                Id = "generate_script",
                Text = "Generate script",
                IconKey = "script",
                SubCommands = new List<WorkspaceCommand>()
            };

            foreach (var db in RelationalDatabases.All)
            {
                var dbCommand = new WorkspaceCommand
                {
                    Id = $"generate_script_{db.Id}",
                    Text = db.Name,
                    IconKey = "database",
                    SubCommands = new List<WorkspaceCommand>
                    {
                        CreateScriptCommand(db, artifact, "Create Table", "create_table"),
                        CreateScriptCommand(db, artifact, "Drop Table", "drop_table"),
                        WorkspaceCommand.Separator,
                        CreateScriptCommand(db, artifact, "Select row", "select_row"),
                        CreateScriptCommand(db, artifact, "Insert row", "insert_row"),
                        CreateScriptCommand(db, artifact, "Update row", "update_row"),
                        CreateScriptCommand(db, artifact, "Delete row", "delete_row")
                    }
                };
                if(artifact.CanAlterTable())
                {
                    dbCommand.SubCommands.Insert(1,CreateScriptCommand(db, artifact, "Alter Table", "alter_table"));
                }
                generateScriptCommand.SubCommands.Add(dbCommand);
            }
            commands.Add(generateScriptCommand);

            commands.Add(WorkspaceCommand.Separator);

            // Add Column command
            commands.Add(new WorkspaceCommand
            {
                Id = "add_column",
                Text = "Add Column",
                IconKey = "columns",
                Execute = async (a) =>
                {
                    var column = new ColumnArtifact("NewColumn", "varchar(255)", true);
                    artifact.AddChild(column);
                    WorkspaceController.OnArtifactAdded(artifact, column);
                    WorkspaceController.RequestBeginRename(column);
                    await Task.CompletedTask;
                }
            });

            // Add Index command
            commands.Add(new WorkspaceCommand
            {
                Id = "add_index",
                Text = "Add Index",
                IconKey = "list",
                Execute = async (a) =>
                {
                    var index = new IndexArtifact("IX_NewIndex", false);
                    artifact.AddChild(index);
                    WorkspaceController.OnArtifactAdded(artifact, index);
                    WorkspaceController.RequestBeginRename(index);
                    await Task.CompletedTask;
                }
            });

            commands.Add(WorkspaceCommand.Separator);

            // Rename command
            commands.Add(new WorkspaceCommand
            {
                Id = "rename_table",
                Text = "Rename",
                IconKey = "edit",
                Execute = async (a) =>
                {
                    WorkspaceController.RequestBeginRename(artifact);
                    await Task.CompletedTask;
                }
            });

            commands.Add(WorkspaceCommand.Separator);

            // Convert to View command
            commands.Add(new WorkspaceCommand
            {
                Id = "convert_to_view",
                Text = "Convert to View",
                IconKey = "eye",
                Execute = async (a) =>
                {
                    await ConvertToViewAsync(artifact);
                }
            });

            commands.Add(WorkspaceCommand.Separator);

            // Delete command
            commands.Add(new WorkspaceCommand
            {
                Id = "delete_table",
                Text = "Delete",
                IconKey = "trash",
                Execute = async (a) =>
                {
                    var parent = artifact.Parent;
                    if (parent != null)
                    {
                        parent.RemoveChild(artifact);
                        WorkspaceController.OnArtifactRemoved(parent, artifact);
                    }
                    await Task.CompletedTask;
                }
            });

            return commands;
        }

        protected override Task OnSelectedInternalAsync(TableArtifact artifact, CancellationToken cancellationToken)
        {
            EnsureEditViewModel(artifact);
            WorkspaceController.ShowWorkspaceDetailsView(_editViewModel!);
            return Task.CompletedTask;
        }

        private void EnsureEditViewModel(TableArtifact artifact)
        {
            if (_editViewModel == null)
            {
                _editViewModel = new TableEditViewModel();
                _editViewModel.ValueChanged += OnEditViewModelValueChanged;
            }

            _editViewModel.Table = artifact;
        }

        private void OnEditViewModelValueChanged(object? sender, ArtifactPropertyChangedEventArgs e)
        {
            WorkspaceController.OnArtifactPropertyChanged(e.Artifact, e.PropertyName, e.NewValue);
        }

        private async Task ConvertToViewAsync(TableArtifact table)
        {
            var parent = table.Parent;
            if (parent == null) return;

            // Create a new view with the same properties
            var view = new ViewArtifact(table.Name, table.Schema);

            // Copy columns
            foreach (var column in table.GetColumns().ToList())
            {
                table.RemoveChild(column);
                view.AddChild(column);
            }

            // Remove the table
            parent.RemoveChild(table);
            WorkspaceController.OnArtifactRemoved(parent, table);

            // Add the view
            parent.AddChild(view);
            WorkspaceController.OnArtifactAdded(parent, view);

            await Task.CompletedTask;
        }

        private WorkspaceCommand CreateScriptCommand(RelationalDatabase db, TableArtifact artifact, string text, string action)
        {
            return new WorkspaceCommand
            {
                Id = $"generate_script_{db.Id}_{action}",
                Text = text,
                IconKey = "script",
                Execute = async (a) =>
                {
                    await GenerateScriptAsync(db, artifact, action);
                }
            };
        }

        private async Task GenerateScriptAsync(RelationalDatabase db, TableArtifact artifact, string action)
        {
            try
            {
                // Capitalize first letter of id for folder name (e.g. mysql -> Mysql)
                var folderName = char.ToUpper(db.Id[0]) + db.Id.Substring(1);
                var templateFolder = WorkspaceSettings.Instance.DefaultTemplateFolder;
                if(!Directory.Exists(templateFolder))
                {
                    throw new DirectoryNotFoundException($"Template folder not found: {templateFolder}");
                }
                var templatePath = Path.Combine(templateFolder,
                    "SQL",
                    folderName,
                    $"table_{action}.sql.scriban");
                var tabLabel = $"{artifact.Name} - {action} [{db.Id}]";
                //if (!File.Exists(templatePath))
                //{
                //    var errorContent = $"-- Template not found: {templatePath}";
                //    await AddGeneratedFileAsync(artifact, $"{artifact.Name}_{action}.sql", errorContent);
                //    return;
                //}
                var scribanTemplate = new ScribanFileTemplate($"table_{action}.sql.scriban", templatePath);
                scribanTemplate.CreateTemplateFileIfMissing = true;
                var templateInstance = new ScribanTemplateInstance(scribanTemplate);

                // Create a context model that includes the table and the target database
                templateInstance.Parameters["Table"] = artifact;
                templateInstance.Parameters["Database"] = db;
                templateInstance.Parameters["Columns"] = artifact.GetColumns().ToList();
                templateInstance.Parameters["Indexes"] = artifact.GetIndexes().ToList();
                templateInstance.Parameters["PrimaryKeyColumns"] = artifact.GetPrimaryKeyColumns().ToList();
                templateInstance.Parameters["NewColumns"] = artifact.GetNewColumns().ToList();
                templateInstance.Parameters["ExistingColumns"] = artifact.GetExistingColumns().ToList();
                templateInstance.Parameters["DeletedColumns"] = artifact.RemovedExistingColumns.ToList();
                templateInstance.Parameters["DeletedIndexes"] = artifact.RemovedExistingIndexes.ToList();
                templateInstance.Parameters["ModifiedExistingColumns"] = artifact.GetModifiedExistingColumns().ToList();
                templateInstance.Parameters["CanAlterTable"] = artifact.CanAlterTable();
                var existingTableDecorator = artifact.GetDecorator<ExistingTableDecorator>();
                templateInstance.Parameters["IsTableRenamed"] = existingTableDecorator!=null && ! string.Equals(artifact.Name, existingTableDecorator?.OriginalTableName);
                templateInstance.Parameters["TableOriginalName"] = existingTableDecorator?.OriginalTableName ?? artifact.Name;
                templateInstance.Functions.Add("GetTypeDef", new Func<ColumnArtifact, string>((c) =>
                {
                    return db.GetMapping(c.DataType)?.GenerateTypeDef(c.MaxLength, c.Precision, c.Scale) ?? c.DataType;
                }));
                templateInstance.Functions.Add("GetIdentifier", new Func<string, string>((identifier) =>
                {
                    return db.EscapeIdentifier(identifier);
                }));

                if (false)
                {
                    var columns = artifact.GetColumns()
                    .Select(c => (name: c.Name, type: db.GetMapping(c.DataType)?.GenerateTypeDef(c.MaxLength, c.Precision, c.Scale) ?? c.DataType, isNullable: c.IsNullable, isPrimaryKey: c.IsPrimaryKey))
                    .ToList();
                    var createScript = db.GenerateCreateTableStatement(artifact.Name, artifact.Schema, columns);
                    _windowManagerService.ShowArtifactPreview(new ArtifactPreviewViewModel()
                    {
                        TabLabel = $"{artifact.Name} - {action} [{db.Id}]",
                        TextContent = createScript,
                        TextLanguageSchema = ArtifactPreviewViewModel.KnownLanguages.SQL
                    });
                    return;
                }
                
                var content = await _templateEngineManager.RenderTemplateAsync(templateInstance);
                if(content.Succeeded == false)
                {
                    var errorContent = $"-- Errors generating script:\n-- {string.Join("\n-- ", content.Errors)}";
                    _windowManagerService.ShowArtifactPreview(new ArtifactPreviewViewModel()
                    {
                        TabLabel = tabLabel,
                        TextContent = errorContent,
                        TextLanguageSchema = ArtifactPreviewViewModel.KnownLanguages.Text
                    });
                    return;
                }
                
                IArtifact? lastGenArtifact = content.Artifacts?.FirstOrDefault();
                    
                if(lastGenArtifact is FileArtifact fileArtifact)
                {
                    _windowManagerService.ShowArtifactPreview(new ArtifactPreviewViewModel() { 
                        TabLabel = tabLabel,
                        TextContent = fileArtifact.GetTextContent(), 
                        TextLanguageSchema = ArtifactPreviewViewModel.KnownLanguages.SQL
                    });
                }
                
            }
            catch (Exception ex)
            {
                var errorContent = $"-- Error generating script: {ex.Message}\n/*\n{ex.StackTrace}\n*/";
                await AddGeneratedFileAsync(artifact, $"{artifact.Name}_{action}_Error.sql", errorContent);
            }
        }

        private async Task AddGeneratedFileAsync(TableArtifact artifact, string fileName, string content)
        {
            var fileArtifact = new FileArtifact(fileName);
            fileArtifact.SetTextContent(content);
            
            // Add to parent (Datasource) to keep it organized
            if (artifact.Parent != null)
            {
                artifact.Parent.AddChild(fileArtifact);
                WorkspaceController.OnArtifactAdded(artifact.Parent, fileArtifact);
                await WorkspaceController.SelectArtifactAsync(fileArtifact);
            }
        }
    }
}
