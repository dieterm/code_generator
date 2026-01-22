using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.Artifacts.CodeGeneration;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Datasources.Mysql.Artifacts;
using CodeGenerator.Core.Workspaces.Settings;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Ribbon;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System.Dynamic;

namespace CodeGenerator.Application.Controllers.Template;

/// <summary>
/// Controller for managing templates and the template tree view
/// </summary>
public class TemplateController : CoreControllerBase
{
    private readonly TemplateEngineManager _templateEngineManager;
    private readonly WorkspaceTreeViewController _workspaceController;
    private TemplateTreeViewModel? _treeViewModel;
    private TemplateParametersViewModel? _parametersViewModel;

    public TemplateController(
        TemplateEngineManager templateEngineManager,
        WorkspaceTreeViewController workspaceController,
        IWindowManagerService windowManagerService,
        RibbonBuilder ribbonBuilder,
        ApplicationMessageBus messageBus,
        IMessageBoxService messageBoxService,
        IFileSystemDialogService fileSystemDialogService,
        ILogger<TemplateController> logger)
        : base(windowManagerService, ribbonBuilder, messageBus, messageBoxService, fileSystemDialogService, logger)
    {
        _templateEngineManager = templateEngineManager;
        _workspaceController = workspaceController;
    }

    //public override void Initialize()
    //{
    //    CreateRibbon();
    //}

    //private void CreateRibbon()
    //{
    //    _ribbonBuilder
    //        .AddTab("tabTemplates", "Templates")
    //            .AddToolStrip("toolstripTemplates", "Templates")
    //                .AddButton("btnShowTemplates", "Browse Templates")
    //                    .WithSize(RibbonButtonSize.Large)
    //                    .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
    //                    .WithImage("file_code")
    //                    .WithToolTip("Browse and execute templates")
    //                    .OnClick((e) => OnShowTemplatesRequested())
    //                .AddButton("btnRefreshTemplates", "Refresh")
    //                    .WithSize(RibbonButtonSize.Small)
    //                    .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
    //                    .WithImage("refresh_cw")
    //                    .WithToolTip("Refresh template list")
    //                    .OnClick((e) => OnRefreshTemplatesRequested())
    //            .EndToolStrip()
    //        .Build();
    //}

    //private void OnShowTemplatesRequested()
    //{
    //    ShowTemplateTreeView();
    //}

    //private void OnRefreshTemplatesRequested()
    //{
    //    if (_treeViewModel != null)
    //    {
    //        LoadTemplates(_treeViewModel);
    //    }
    //}

    ///// <summary>
    ///// Show or refresh the template tree view
    ///// </summary>
    //public void ShowTemplateTreeView()
    //{
    //    if (_treeViewModel == null)
    //    {
    //        // this code below will not work. all code in this class needs to be moved to TemplateTreeViewController
    //        _treeViewModel = new TemplateTreeViewModel((TemplateTreeViewController)(object)this);
    //        _treeViewModel.PropertyChanged += TreeViewModel_PropertyChanged;
    //        _treeViewModel.TemplateSelected += TreeViewModel_TemplateSelected;
    //    }

    //    LoadTemplates(_treeViewModel);
    //    _windowManagerService.ShowTemplateTreeView(_treeViewModel);
    //}

    //private void TreeViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    //{
    //    if (e.PropertyName == nameof(TemplateTreeViewModel.SelectedArtifact))
    //    {
    //        if (_treeViewModel?.SelectedArtifact is TemplateArtifact templateArtifact)
    //        {
    //            _treeViewModel.OnTemplateSelected(templateArtifact);
    //        }
    //    }
    //}

    //private void TreeViewModel_TemplateSelected(object? sender, TemplateArtifact template)
    //{
    //    ShowTemplateParameters(template);
    //}

    ///// <summary>
    ///// Show template parameters view for the selected template
    ///// </summary>
    //private void ShowTemplateParameters(TemplateArtifact template)
    //{
    //    if (_parametersViewModel == null)
    //    {
    //        _parametersViewModel = new TemplateParametersViewModel();
    //        _parametersViewModel.SetWorkspaceController(_workspaceController);
    //        _parametersViewModel.ExecuteRequested += ParametersViewModel_ExecuteRequested;
    //    }

    //    _parametersViewModel.TemplateArtifact = template;
    //    _windowManagerService.ShowTemplateParametersView(_parametersViewModel);
    //}

    //private async void ParametersViewModel_ExecuteRequested(object? sender, EventArgs e)
    //{
    //    if (_parametersViewModel?.TemplateArtifact == null)
    //        return;

    //    await ExecuteTemplateAsync(_parametersViewModel.TemplateArtifact, _parametersViewModel.GetParameterValues());
    //}

    ///// <summary>
    ///// Load templates from the configured template folder
    ///// </summary>
    //private void LoadTemplates(TemplateTreeViewModel viewModel)
    //{
    //    var templateFolder = WorkspaceSettings.Instance.DefaultTemplateFolder;

    //    if (string.IsNullOrEmpty(templateFolder) || !Directory.Exists(templateFolder))
    //    {
    //        _logger.LogWarning("Template folder not configured or does not exist: {TemplateFolder}", templateFolder);
    //        //viewModel.TemplateFolder = templateFolder ?? string.Empty;
    //        viewModel.RootArtifact = null;
    //        return;
    //    }

    //    //viewModel.TemplateFolder = templateFolder;

    //    // Create root artifact for the template folder
    //    var rootArtifact = new RootArtifact("Templates", templateFolder);

    //    // Recursively scan for templates
    //    ScanFolderForTemplates(templateFolder, rootArtifact);

    //    viewModel.RootArtifact = rootArtifact;
    //}

    ///// <summary>
    ///// Recursively scan a folder for template files
    ///// </summary>
    //private void ScanFolderForTemplates(string folderPath, Core.Artifacts.IArtifact parentArtifact)
    //{
    //    try
    //    {
    //        // Process subdirectories first
    //        foreach (var subDir in Directory.GetDirectories(folderPath))
    //        {
    //            var folderName = Path.GetFileName(subDir);
    //            var folderArtifact = new ExistingFolderArtifact(subDir, folderName);
    //            parentArtifact.AddChild(folderArtifact);

    //            // Recursively scan subdirectory
    //            ScanFolderForTemplates(subDir, folderArtifact);
    //        }

    //        // Process template files
    //        foreach (var filePath in Directory.GetFiles(folderPath))
    //        {
    //            // Skip definition files
    //            if (filePath.EndsWith(TemplateDefinition.DefinitionFileExtension, StringComparison.OrdinalIgnoreCase))
    //                continue;

    //            var extension = Path.GetExtension(filePath).TrimStart('.');
    //            var templateEngine = _templateEngineManager.GetTemplateEngineByFileExtension(extension);

    //            if (templateEngine != null)
    //            {
    //                var templateArtifact = new TemplateArtifact(filePath, templateEngine);
    //                parentArtifact.AddChild(templateArtifact);
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error scanning folder for templates: {FolderPath}", folderPath);
    //    }
    //}

    ///// <summary>
    ///// Execute a template with the given parameters
    ///// </summary>
    //public async Task ExecuteTemplateAsync(TemplateArtifact templateArtifact, Dictionary<string, object?> parameters, CancellationToken cancellationToken = default)
    //{
    //    if (templateArtifact.Template == null)
    //    {
    //        _messageBoxService.ShowError("Failed to load template.", "Template Error");
    //        return;
    //    }

    //    if (_parametersViewModel != null)
    //    {
    //        _parametersViewModel.IsExecuting = true;
    //    }

    //    try
    //    {
    //        // Get the appropriate template engine
    //        var engines = _templateEngineManager.GetTemplateEnginesForTemplate(templateArtifact.Template);
    //        var engine = engines.FirstOrDefault();

    //        if (engine == null)
    //        {
    //            _messageBoxService.ShowError("No template engine found for this template type.", "Template Error");
    //            return;
    //        }

    //        // Create template instance and set parameters
    //        var templateInstance = engine.CreateTemplateInstance(templateArtifact.Template);

    //        // Process parameters - load data for TableArtifact parameters
    //        var processedParameters = await ProcessTemplateParametersAsync(parameters, templateArtifact, cancellationToken);

    //        // Set parameters on the template instance
    //        foreach (var kvp in processedParameters)
    //        {
    //            templateInstance.SetParameter(kvp.Key, kvp.Value);
    //        }

    //        // Render the template
    //        var output = await engine.RenderAsync(templateInstance, cancellationToken);

    //        if (output.Succeeded)
    //        {
    //            // Show preview of the generated content
    //            if (output.Artifacts.Any())
    //            {
    //                var firstArtifact = output.Artifacts.First();
    //                if (firstArtifact is FileArtifact fileArtifact)
    //                {
    //                    _windowManagerService.ShowArtifactPreview(new ArtifactPreviewViewModel
    //                    {
    //                        TabLabel = $"Generated: {templateArtifact.DisplayName}",
    //                        TextContent = fileArtifact.GetTextContent(),
    //                        TextLanguageSchema = DetermineLanguageSchema(templateArtifact.FilePath)
    //                    });
    //                }
    //            }
    //            else if (!string.IsNullOrEmpty(output.TextContent))
    //            {
    //                _windowManagerService.ShowArtifactPreview(new ArtifactPreviewViewModel
    //                {
    //                    TabLabel = $"Generated: {templateArtifact.DisplayName}",
    //                    TextContent = output.TextContent,
    //                    TextLanguageSchema = DetermineLanguageSchema(templateArtifact.FilePath)
    //                });
    //            }

    //            _logger.LogInformation("Template executed successfully: {TemplateName}", templateArtifact.DisplayName);
    //        }
    //        else
    //        {
    //            var errorMessage = string.Join("\n", output.Errors);
    //            _messageBoxService.ShowError($"Template execution failed:\n{errorMessage}", "Template Error");
    //            _logger.LogError("Template execution failed: {Errors}", errorMessage);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        _messageBoxService.ShowError($"Error executing template: {ex.Message}", "Template Error");
    //        _logger.LogError(ex, "Error executing template: {TemplateName}", templateArtifact.DisplayName);
    //    }
    //    finally
    //    {
    //        if (_parametersViewModel != null)
    //        {
    //            _parametersViewModel.IsExecuting = false;
    //        }
    //    }
    //}

    ///// <summary>
    ///// Process template parameters, loading data for TableArtifact parameters
    ///// </summary>
    //private async Task<Dictionary<string, object?>> ProcessTemplateParametersAsync(
    //    Dictionary<string, object?> parameters,
    //    TemplateArtifact templateArtifact,
    //    CancellationToken cancellationToken)
    //{
    //    var result = new Dictionary<string, object?>();
    //    //{
    //    //    {"ConnectionString", parameters["ConnectionString"]}
    //    //};
    //    var templateParams = _parametersViewModel?.GetTemplateParameters() ?? templateArtifact.Parameters;

    //    foreach (var kvp in parameters)
    //    {
    //        var paramDef = templateParams.FirstOrDefault(p => p.Name == kvp.Key);

    //        // Check if this is a TableArtifactData parameter
    //        if (paramDef?.IsTableArtifactData == true && kvp.Value is TableArtifactItem tableItem)
    //        {
    //            // Load data from the database
    //            var data = await LoadTableDataAsync(
    //                tableItem, 
    //                paramDef.TableDataFilter, 
    //                paramDef.TableDataMaxRows,
    //                cancellationToken);
    //            result[kvp.Key] = data;
    //        }
    //        else
    //        {
    //            result[kvp.Key] = kvp.Value;
    //        }
    //    }

    //    return result;
    //}

    ///// <summary>
    ///// Load data from a table in the database
    ///// </summary>
    //private async Task<IEnumerable<dynamic>> LoadTableDataAsync(
    //    TableArtifactItem tableItem,
    //    string? filter,
    //    int? maxRows,
    //    CancellationToken cancellationToken)
    //{
    //    var results = new List<dynamic>();

    //    if (tableItem.DatasourceArtifact is not MysqlDatasourceArtifact mysqlDatasource)
    //    {
    //        _logger.LogWarning("TableArtifactData is only supported for MySQL datasources currently");
    //        return results;
    //    }

    //    var table = tableItem.TableArtifact;
    //    var columns = table.GetColumns().ToList();

    //    // Build SELECT query
    //    var columnNames = string.Join(", ", columns.Select(c => $"`{c.Name}`"));
    //    var tableName = !string.IsNullOrEmpty(table.Schema) 
    //        ? $"`{table.Schema}`.`{table.Name}`" 
    //        : $"`{table.Name}`";

    //    var query = $"SELECT {columnNames} FROM {tableName}";

    //    if (!string.IsNullOrWhiteSpace(filter))
    //    {
    //        query += $" WHERE {filter}";
    //    }

    //    if (maxRows.HasValue && maxRows.Value > 0)
    //    {
    //        query += $" LIMIT {maxRows.Value}";
    //    }

    //    _logger.LogDebug("Executing query: {Query}", query);

    //    try
    //    {
    //        await using var connection = new MySqlConnection(mysqlDatasource.ConnectionString);
    //        await connection.OpenAsync(cancellationToken);

    //        await using var command = new MySqlCommand(query, connection);
    //        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

    //        while (await reader.ReadAsync(cancellationToken))
    //        {
    //            var row = new ExpandoObject() as IDictionary<string, object?>;

    //            for (int i = 0; i < reader.FieldCount; i++)
    //            {
    //                var columnName = reader.GetName(i);
    //                var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
    //                row[columnName] = value;
    //            }

    //            results.Add(row);
    //        }

    //        _logger.LogInformation("Loaded {RowCount} rows from table {TableName}", results.Count, table.Name);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error loading data from table {TableName}", table.Name);
    //        throw;
    //    }

    //    return results;
    //}

    ///// <summary>
    ///// Determine the language schema for syntax highlighting based on template file path
    ///// </summary>
    //private ArtifactPreviewViewModel.KnownLanguages DetermineLanguageSchema(string templateFilePath)
    //{
    //    var fileName = Path.GetFileName(templateFilePath);

    //    // Check for patterns like "file.sql.scriban" -> SQL
    //    if (fileName.Contains(".sql.", StringComparison.OrdinalIgnoreCase))
    //        return ArtifactPreviewViewModel.KnownLanguages.SQL;
    //    if (fileName.Contains(".cs.", StringComparison.OrdinalIgnoreCase))
    //        return ArtifactPreviewViewModel.KnownLanguages.CSharp;
    //    if (fileName.Contains(".xml.", StringComparison.OrdinalIgnoreCase))
    //        return ArtifactPreviewViewModel.KnownLanguages.XML;
    //    if (fileName.Contains(".html.", StringComparison.OrdinalIgnoreCase))
    //        return ArtifactPreviewViewModel.KnownLanguages.HTML;
    //    if (fileName.Contains(".js.", StringComparison.OrdinalIgnoreCase))
    //        return ArtifactPreviewViewModel.KnownLanguages.JScript;
    //    if (fileName.Contains(".java.", StringComparison.OrdinalIgnoreCase))
    //        return ArtifactPreviewViewModel.KnownLanguages.Java;
    //    if (fileName.Contains(".c.", StringComparison.OrdinalIgnoreCase))
    //        return ArtifactPreviewViewModel.KnownLanguages.C;
    //    if (fileName.Contains(".ps1.", StringComparison.OrdinalIgnoreCase))
    //        return ArtifactPreviewViewModel.KnownLanguages.PowerShell;
    //    if (fileName.Contains(".vb.", StringComparison.OrdinalIgnoreCase))
    //        return ArtifactPreviewViewModel.KnownLanguages.VBNET;

    //    return ArtifactPreviewViewModel.KnownLanguages.Text;
    //}

    public override void Dispose()
    {
        if (_treeViewModel != null)
        {
            _treeViewModel.PropertyChanged -= TreeViewModel_PropertyChanged;
            _treeViewModel.TemplateSelected -= TreeViewModel_TemplateSelected;
        }

        if (_parametersViewModel != null)
        {
            _parametersViewModel.ExecuteRequested -= ParametersViewModel_ExecuteRequested;
        }
    }
}

