using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Settings;
using CodeGenerator.Domain.Databases.RelationalDatabases;
using CodeGenerator.Shared;
using CodeGenerator.TemplateEngines.Scriban;
using Microsoft.Extensions.Logging;
using System;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Controller for TableArtifact
    /// </summary>
    public class TableArtifactController : ArtifactControllerBase<WorkspaceTreeViewController, TableArtifact>
    {
        private TableEditViewModel? _editViewModel;
        private readonly TemplateEngineManager _templateEngineManager;
        private readonly IWindowManagerService _windowManagerService;
        private static List<CreateScriptCommandInfo>? _createScriptCommands;

        static TableArtifactController()
        {
            _createScriptCommands = GetCreateScriptCommands().ToList();
        }

        public TableArtifactController(
            WorkspaceTreeViewController workspaceController,
            TemplateEngineManager templateEngineManager,
            IWindowManagerService windowManagerService,
            ILogger<TableArtifactController> logger)
            : base(workspaceController, logger)
        {
            _templateEngineManager = templateEngineManager;
            _windowManagerService = windowManagerService;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(TableArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            var createObjectCommand = new ArtifactTreeNodeCommand
            {
                Id = "create_object",
                Text = "Create object",
                IconKey = "script",
                SubCommands = new List<ArtifactTreeNodeCommand>()
            };

            var newEntityCommand = new ArtifactTreeNodeCommand
            {
                Id = "new_entity",
                Text = "New Entity",
                IconKey = "script",
                SubCommands = new List<ArtifactTreeNodeCommand>()
            };
            createObjectCommand.SubCommands.Add(newEntityCommand);

            foreach (var domain in TreeViewController.CurrentWorkspace!.Domains.GetDomains())
            {
                var domainCommand = new ArtifactTreeNodeCommand
                {
                    Id = $"new_entity_{domain.Id}",
                    Text = domain.Name,
                    IconKey = "domain",
                    Execute = async (a) =>
                    {
                        TreeViewController.CreateEntityFromTableInDomain(artifact, domain);
                        await Task.CompletedTask;
                    }
                };
                newEntityCommand.SubCommands.Add(domainCommand);
            }

            commands.Add(createObjectCommand);

            // Generate Script command
            var generateScriptCommand = new ArtifactTreeNodeCommand
            {
                Id = "generate_script",
                Text = "Generate script",
                IconKey = "script",
                SubCommands = new List<ArtifactTreeNodeCommand>()
            };

            foreach (var db in RelationalDatabases.All)
            {
                var dbCommand = new ArtifactTreeNodeCommand
                {
                    Id = $"generate_script_{db.Id}",
                    Text = db.Name,
                    IconKey = "database",
                    SubCommands = new List<ArtifactTreeNodeCommand>()
                };

                foreach (var commandInfo in _createScriptCommands!)
                {
                    if (commandInfo.Database.Id == db.Id)
                    {
                        if (commandInfo.CanExecute(artifact))
                        {
                            dbCommand.SubCommands.Add(CreateScriptCommand(commandInfo, artifact));
                        }
                    }
                }

                generateScriptCommand.SubCommands.Add(dbCommand);
            }
            commands.Add(generateScriptCommand);

            commands.Add(ArtifactTreeNodeCommand.Separator);

            // Add Column command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "add_column",
                Text = "Add Column",
                IconKey = "columns",
                Execute = async (a) =>
                {
                    var column = new ColumnArtifact("NewColumn", "varchar(255)", true);
                    artifact.AddChild(column);
                    TreeViewController.OnArtifactAdded(artifact, column);
                    TreeViewController.RequestBeginRename(column);
                    await Task.CompletedTask;
                }
            });

            // Add Index command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "add_index",
                Text = "Add Index",
                IconKey = "list",
                Execute = async (a) =>
                {
                    var index = new IndexArtifact("IX_NewIndex", false);
                    artifact.AddChild(index);
                    TreeViewController.OnArtifactAdded(artifact, index);
                    TreeViewController.RequestBeginRename(index);
                    await Task.CompletedTask;
                }
            });

            // Add Foreign Key command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "add_foreignkey",
                Text = "Add Foreign Key",
                IconKey = "link",
                Execute = async (a) =>
                {
                    var foreignKey = new ForeignKeyArtifact($"FK_{artifact.Name}_New");
                    artifact.AddChild(foreignKey);
                    TreeViewController.OnArtifactAdded(artifact, foreignKey);
                    TreeViewController.RequestBeginRename(foreignKey);
                    await Task.CompletedTask;
                }
            });

            commands.Add(ArtifactTreeNodeCommand.Separator);

            // Rename command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "rename_table",
                Text = "Rename",
                IconKey = "edit",
                Execute = async (a) =>
                {
                    TreeViewController.RequestBeginRename(artifact);
                    await Task.CompletedTask;
                }
            });

            commands.Add(ArtifactTreeNodeCommand.Separator);

            // Convert to View command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "convert_to_view",
                Text = "Convert to View",
                IconKey = "eye",
                Execute = async (a) =>
                {
                    await ConvertToViewAsync(artifact);
                }
            });

            // Note: Delete command is now added automatically by base class via GetClipboardCommands()

            return commands;
        }

        #region Clipboard Operations

        public override bool CanDelete(TableArtifact artifact)
        {
            return artifact.Parent != null;
        }

        public override void Delete(TableArtifact artifact)
        {
            if (!CanDelete(artifact)) return;

            DeleteTableAsync(artifact).GetAwaiter().GetResult();
        }

        private async Task DeleteTableAsync(TableArtifact artifact)
        {
            var parent = artifact.Parent;
            if (parent == null) return;

            // Find the datasource to check for foreign key references
            var datasource = artifact.FindAncesterOfType<DatasourceArtifact>();
            if (datasource != null)
            {
                // Find all foreign keys that reference this table
                var referencingForeignKeys = FindForeignKeysReferencingTable(datasource, artifact.Id);

                if (referencingForeignKeys.Any())
                {
                    // Build warning message
                    var referencingTables = referencingForeignKeys
                        .Select(fk => fk.Parent as TableArtifact)
                        .Where(t => t != null)
                        .Select(t => t!.Name)
                        .Distinct()
                        .ToList();

                    var message = $"The table '{artifact.Name}' is referenced by foreign keys in the following tables:\n\n" +
                                  string.Join("\n", referencingTables.Select(t => $"• {t}")) +
                                  "\n\nDo you want to delete this table and remove all referencing foreign key columns?";

                    var messageBoxService = ServiceProviderHolder.GetRequiredService<IMessageBoxService>();
                    if (!messageBoxService.AskYesNo(message, "Delete Table"))
                    {
                        return; // User cancelled
                    }

                    // Remove column mappings from referencing foreign keys
                    foreach (var foreignKey in referencingForeignKeys)
                    {
                        foreignKey.RemoveColumnMappingsForTable(artifact.Id);

                        // If foreign key has no more column mappings, remove it entirely
                        if (!foreignKey.ColumnMappings.Any())
                        {
                            var fkParent = foreignKey.Parent;
                            if (fkParent != null)
                            {
                                fkParent.RemoveChild(foreignKey);
                                TreeViewController.OnArtifactRemoved(fkParent, foreignKey);
                            }
                        }
                    }
                }
            }

            // Delete the table
            parent.RemoveChild(artifact);
            TreeViewController.OnArtifactRemoved(parent, artifact);

            await Task.CompletedTask;
        }

        #endregion

        private IEnumerable<ForeignKeyArtifact> FindForeignKeysReferencingTable(DatasourceArtifact datasource, string tableId)
        {
            return datasource.GetAllDescendants()
                .OfType<ForeignKeyArtifact>()
                .Where(fk => fk.ReferencesTable(tableId));
        }

        protected override Task OnSelectedInternalAsync(TableArtifact artifact, CancellationToken cancellationToken)
        {
            EnsureEditViewModel(artifact);
            TreeViewController.ShowArtifactDetailsView(_editViewModel!);
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
            TreeViewController.OnArtifactPropertyChanged(e.Artifact, e.PropertyName, e.NewValue);
        }

        private async Task ConvertToViewAsync(TableArtifact table)
        {
            var parent = table.Parent;
            if (parent == null) return;

            var view = new ViewArtifact(table.Name, table.Schema);

            foreach (var column in table.GetColumns().ToList())
            {
                table.RemoveChild(column);
                view.AddChild(column);
            }

            parent.RemoveChild(table);
            TreeViewController.OnArtifactRemoved(parent, table);

            parent.AddChild(view);
            TreeViewController.OnArtifactAdded(parent, view);

            await Task.CompletedTask;
        }
        private ArtifactTreeNodeCommand CreateScriptCommand(CreateScriptCommandInfo commandInfo, TableArtifact artifact)
        {
            return new ArtifactTreeNodeCommand
            {
                Id = $"generate_script_{commandInfo.Database.Id}_{commandInfo.Action}",
                Text = commandInfo.Text,
                IconKey = "script",
                Execute = async (a) =>
                {
                    await commandInfo.ExecuteAsync(artifact, this);
                }
            };
        }

        private async Task GenerateScriptAsync(RelationalDatabase db, TableArtifact artifact, string action)
        {
            try
            {
                var commandInfo = _createScriptCommands!.Single(cmd => cmd.Action == action && cmd.Database == db);
                var requiredTemplate = _requiredTemplates!.Single(rt => rt.TemplateFolderPath[2] == SUBFOLDER_GENERATE_SCRIPT && rt.TemplateFolderPath[3] == db.Id && rt.TemplateDefinition.TemplateName==commandInfo.GetTemplateName());
                var templateManager = ServiceProviderHolder.GetRequiredService<TemplateManager>();
                var scribanTemplate = templateManager.GetTemplateById(requiredTemplate.TemplateId) as ScribanFileTemplate;
                if(scribanTemplate == null)
                {
                    var templateFolderPath = templateManager.ResolveTemplateIdToFolderPath(requiredTemplate.TemplateId);
                    var templatePath = Path.Combine(templateFolderPath, requiredTemplate.TemplateDefinition.TemplateName+ ".scriban");
                    scribanTemplate = new ScribanFileTemplate(requiredTemplate.TemplateDefinition.TemplateName, templatePath);
                    Logger.LogWarning($"Template not loaded in manager, loading from path: {templatePath}");
                 }

                var tabLabel = $"{artifact.Name} - {action} [{db.Id}]";

                scribanTemplate.CreateTemplateFileIfMissing = true;
                var templateInstance = new ScribanTemplateInstance(scribanTemplate);

                templateInstance.Parameters["Table"] = artifact;
                templateInstance.Parameters["Database"] = db;
                templateInstance.Parameters["Columns"] = artifact.GetColumns().ToList();
                templateInstance.Parameters["Indexes"] = artifact.GetIndexes().ToList();
                templateInstance.Parameters["ForeignKeys"] = artifact.GetForeignKeys().ToList();
                templateInstance.Parameters["PrimaryKeyColumns"] = artifact.GetPrimaryKeyColumns().ToList();
                templateInstance.Parameters["NewColumns"] = artifact.GetNewColumns().ToList();
                templateInstance.Parameters["ExistingColumns"] = artifact.GetExistingColumns().ToList();
                templateInstance.Parameters["DeletedColumns"] = artifact.RemovedExistingColumns.ToList();
                templateInstance.Parameters["DeletedIndexes"] = artifact.RemovedExistingIndexes.ToList();
                templateInstance.Parameters["ModifiedExistingColumns"] = artifact.GetModifiedExistingColumns().ToList();
                templateInstance.Parameters["CanAlterTable"] = artifact.CanAlterTable();
                var existingTableDecorator = artifact.GetDecoratorOfType<ExistingTableDecorator>();
                templateInstance.Parameters["IsTableRenamed"] = existingTableDecorator != null && !string.Equals(artifact.Name, existingTableDecorator?.OriginalTableName);
                templateInstance.Parameters["TableOriginalName"] = existingTableDecorator?.OriginalTableName ?? artifact.Name;
                templateInstance.Functions.Add("GetTypeDef", new Func<ColumnArtifact, string>((c) =>
                {
                    return db.GetMapping(c.DataType)?.GenerateTypeDef(c.MaxLength, c.Precision, c.Scale, c.GetAllowedValues()) ?? c.DataType;
                }));
                templateInstance.Functions.Add("GetDefaultValue", new Func<ColumnArtifact, string?>((c) =>
                {
                    return db.FormatDefaultValue(c.DefaultValue, c.DataType);
                }));
                templateInstance.Functions.Add("GetIdentifier", new Func<string, string>((identifier) =>
                {
                    return db.EscapeIdentifier(identifier);
                }));

                var content = await _templateEngineManager.RenderTemplateAsync(templateInstance);
                if (content.Succeeded == false)
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

                if (lastGenArtifact is FileArtifact fileArtifact)
                {
                    _windowManagerService.ShowArtifactPreview(new ArtifactPreviewViewModel()
                    {
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

            if (artifact.Parent != null)
            {
                artifact.Parent.AddChild(fileArtifact);
                TreeViewController.OnArtifactAdded(artifact.Parent, fileArtifact);
                await TreeViewController.SelectArtifactAsync(fileArtifact);
            }
        }
        private List<TemplateDefinitionAndLocation>? _requiredTemplates = null;
        public override List<TemplateDefinitionAndLocation> RegisterRequiredTemplates()
        {
            var requiredTemplates = base.RegisterRequiredTemplates();

            foreach (var commandInfo in _createScriptCommands!)
            {
                var dbFolderName = commandInfo.Database.Id;
                var subFolders = new string[] { SUBFOLDER_GENERATE_SCRIPT, dbFolderName };

                var templateDefinition = TemplateDefinitionAndLocation.ForWorkspaceArtifact(
                    artifactTypeName: typeof(TableArtifact).Name,
                    templateName: commandInfo.GetTemplateName(),
                    displayName: $"{commandInfo.Database.Name} - {commandInfo.Text} Script",
                    description: $"Generates a {commandInfo.Text} script for the table in {commandInfo.Database.Name}.", subFolders);

                requiredTemplates.Add(templateDefinition);
            }
            
            _requiredTemplates = requiredTemplates;
            
            return requiredTemplates;
        }
        public const string SUBFOLDER_GENERATE_SCRIPT = "generate_script";
        public static IEnumerable<CreateScriptCommandInfo> GetCreateScriptCommands()
        {
            foreach (var db in RelationalDatabases.All)
            {
                yield return new CreateScriptCommandInfo(db, CreateScriptCommandInfo.CREATE_TABLE, $"Create Table");
                yield return new CreateScriptCommandInfo(db, CreateScriptCommandInfo.DROP_TABLE, $"Drop Table");
                yield return new CreateScriptCommandInfo(db, CreateScriptCommandInfo.SELECT_ROW, $"Select Row");
                yield return new CreateScriptCommandInfo(db, CreateScriptCommandInfo.INSERT_ROW, $"Insert Row");
                yield return new CreateScriptCommandInfo(db, CreateScriptCommandInfo.UPDATE_ROW, $"Update Row");
                yield return new CreateScriptCommandInfo(db, CreateScriptCommandInfo.DELETE_ROW, $"Delete Row");
                yield return new CreateScriptCommandInfo(db, CreateScriptCommandInfo.ALTER_TABLE, $"Alter Table");
            }
        }

        public class CreateScriptCommandInfo
        {
            public const string CREATE_TABLE = "create_table";
            public const string ALTER_TABLE = "alter_table";
            public const string DROP_TABLE = "drop_table";
            public const string SELECT_ROW = "select_row";
            public const string INSERT_ROW = "insert_row";
            public const string UPDATE_ROW = "update_row";
            public const string DELETE_ROW = "delete_row";

            public string GetTemplateName()
            {
                return $"{Action}_sql";
            }
            public RelationalDatabase Database { get; set; }
            public string Action { get; set; }
            public string Text { get; set; }
            public CreateScriptCommandInfo(RelationalDatabase database, string action, string text)
            {
                Database = database;
                Action = action;
                Text = text;
            }
            public bool CanExecute(TableArtifact artifact)
            {
                if (Action == ALTER_TABLE)
                {
                    return artifact.CanAlterTable();
                }
                return true;
            }
            public async Task ExecuteAsync(TableArtifact artifact, TableArtifactController controller)
            {
                await controller.GenerateScriptAsync(Database, artifact, Action);
            }
        }
    }
}
