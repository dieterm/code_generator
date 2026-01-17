using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.Artifacts.CodeGeneration;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Datasources.Mysql.Artifacts;
using CodeGenerator.Core.Workspaces.Settings;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ExtensionMethods;
using CodeGenerator.Shared.Ribbon;
using CodeGenerator.Shared.ViewModels;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Controllers.Template
{
    public class TemplateTreeViewController : ArtifactTreeViewController<TemplateTreeViewModel>, IDisposable
    {
        private readonly RibbonBuilder _ribbonBuilder;
        
        private readonly WorkspaceTreeViewController _workspaceController = ServiceProviderHolder.GetRequiredService<WorkspaceTreeViewController>();
        private readonly TemplateEngineManager _templateEngineManager;
        private ArtifactDetailsViewModel _artifactDetailsViewModel;

        public TemplateTreeViewController(TemplateEngineManager templateEngineManager, WorkspaceTreeViewController workspaceController, RibbonBuilder ribbonBuilder, IWindowManagerService windowManagerService, IMessageBoxService messageBoxService, ILogger<TemplateTreeViewController> logger) : base(windowManagerService, messageBoxService, logger)
        {
            _ribbonBuilder = ribbonBuilder ?? throw new ArgumentNullException(nameof(ribbonBuilder));
            _workspaceController = workspaceController ?? throw new ArgumentNullException(nameof(workspaceController));
            _templateEngineManager = templateEngineManager ?? throw new ArgumentNullException(nameof(templateEngineManager));
        }

        protected override IEnumerable<IArtifactController> LoadArtifactControllers()
        {
            // TemplateArtifactController
            return new IArtifactController[]
            {
                ServiceProviderHolder.GetRequiredService<TemplateArtifactController>(),
                ServiceProviderHolder.GetRequiredService<ExistingFolderArtifactController>(),
            };
        }

        public void Initialize()
        {
            CreateRibbon();
        }

        private void CreateRibbon()
        {
            _ribbonBuilder
                .AddTab("tabTemplates", "Templates")
                    .AddToolStrip("toolstripTemplates", "Templates")
                        .AddButton("btnShowTemplates", "Browse Templates")
                            .WithSize(RibbonButtonSize.Large)
                            .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                            .WithImage("file_code")
                            .WithToolTip("Browse and execute templates")
                            .OnClick((e) => OnShowTemplatesRequested())
                        .AddButton("btnRefreshTemplates", "Refresh")
                            .WithSize(RibbonButtonSize.Small)
                            .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                            .WithImage("refresh_cw")
                            .WithToolTip("Refresh template list")
                            .OnClick((e) => OnRefreshTemplatesRequested())
                    .EndToolStrip()
                .Build();
        }

        private void OnShowTemplatesRequested()
        {
            ShowTemplateTreeView();
        }

        private void OnRefreshTemplatesRequested()
        {
            if (TreeViewModel != null)
            {
                LoadTemplates(TreeViewModel);
            }
        }

        /// <summary>
        /// Show or refresh the template tree view
        /// </summary>
        public void ShowTemplateTreeView()
        {
            if (TreeViewModel == null)
            {
                // this code below will not work. all code in this class needs to be moved to TemplateTreeViewController
                TreeViewModel = new TemplateTreeViewModel((TemplateTreeViewController)(object)this);
                TreeViewModel.PropertyChanged += TreeViewModel_PropertyChanged;
                //TreeViewModel.TemplateSelected += TreeViewModel_TemplateSelected;
            }

            LoadTemplates(TreeViewModel);
            WindowManagerService.ShowTemplateTreeView(TreeViewModel);
        }

        private void TreeViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TemplateTreeViewModel.SelectedArtifact))
            {
                if (TreeViewModel?.SelectedArtifact is TemplateArtifact templateArtifact)
                {
                    TreeViewModel.OnTemplateSelected(templateArtifact);
                }
            }
        }

       

        /// <summary>
        /// Load templates from the configured template folder
        /// </summary>
        private void LoadTemplates(TemplateTreeViewModel viewModel)
        {
            var templateFolder = WorkspaceSettings.Instance.DefaultTemplateFolder;

            if (string.IsNullOrEmpty(templateFolder) || !Directory.Exists(templateFolder))
            {
                Logger.LogWarning("Template folder not configured or does not exist: {TemplateFolder}", templateFolder);
                //viewModel.TemplateFolder = templateFolder ?? string.Empty;
                viewModel.RootArtifact = null;
                return;
            }

            //viewModel.TemplateFolder = templateFolder;

            // Create root artifact for the template folder
            var rootArtifact = new RootArtifact("Templates", templateFolder);

            // Recursively scan for templates
            ScanFolderForTemplates(templateFolder, rootArtifact);

            viewModel.RootArtifact = rootArtifact;
        }

        /// <summary>
        /// Recursively scan a folder for template files
        /// </summary>
        private void ScanFolderForTemplates(string folderPath, Core.Artifacts.IArtifact parentArtifact)
        {
            try
            {
                // Process subdirectories first
                foreach (var subDir in Directory.GetDirectories(folderPath))
                {
                    var folderName = Path.GetFileName(subDir);
                    var folderArtifact = new ExistingFolderArtifact(subDir, folderName);
                    parentArtifact.AddChild(folderArtifact);

                    // Recursively scan subdirectory
                    ScanFolderForTemplates(subDir, folderArtifact);
                }

                // Process template files
                foreach (var filePath in Directory.GetFiles(folderPath))
                {
                    // Skip definition files
                    if (filePath.EndsWith(TemplateDefinition.DefinitionFileExtension, StringComparison.OrdinalIgnoreCase))
                        continue;

                    var extension = Path.GetExtension(filePath).TrimStart('.');
                    var templateEngine = _templateEngineManager.GetTemplateEngineByFileExtension(extension);

                    if (templateEngine != null)
                    {
                        var templateArtifact = new TemplateArtifact(filePath, templateEngine);
                        parentArtifact.AddChild(templateArtifact);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error scanning folder for templates: {FolderPath}", folderPath);
            }
        }

        private TemplateParametersViewModel? _parametersViewModel { get { return ArtifactControllers.OfType<TemplateArtifactController>().FirstOrDefault()?.EditViewModel; } }

        /// <summary>
        /// Execute a template with the given parameters
        /// </summary>
        public async Task ExecuteTemplateAsync(TemplateArtifact templateArtifact, Dictionary<string, object?> parameters, CancellationToken cancellationToken = default)
        {
            if (templateArtifact.Template == null)
            {
                MessageBoxService.ShowError("Failed to load template.", "Template Error");
                return;
            }

            if (_parametersViewModel != null)
            {
                _parametersViewModel.IsExecuting = true;
            }

            try
            {
                // Get the appropriate template engine
                var engines = _templateEngineManager.GetTemplateEnginesForTemplate(templateArtifact.Template);
                var engine = engines.FirstOrDefault();

                if (engine == null)
                {
                    MessageBoxService.ShowError("No template engine found for this template type.", "Template Error");
                    return;
                }

                // Create template instance and set parameters
                var templateInstance = engine.CreateTemplateInstance(templateArtifact.Template);

                // Process parameters - load data for TableArtifact parameters
                var processedParameters = await ProcessTemplateParametersAsync(parameters, templateArtifact, cancellationToken);

                // Set parameters on the template instance
                foreach (var kvp in processedParameters)
                {
                    templateInstance.SetParameter(kvp.Key, kvp.Value);
                }

                // Render the template
                var output = await engine.RenderAsync(templateInstance, cancellationToken);

                if (output.Succeeded)
                {
                    // Show preview of the generated content
                    if (output.Artifacts.Any())
                    {
                        var firstArtifact = output.Artifacts.First();
                        if (firstArtifact is FileArtifact fileArtifact)
                        {
                            if(fileArtifact.HasDecorator<TextContentDecorator>())
                            {
                                // If the file artifact has text content decorator, show that
                                WindowManagerService.ShowArtifactPreview(new ArtifactPreviewViewModel
                                {
                                    FileName = fileArtifact.FileName,
                                    TabLabel = $"Generated: {templateArtifact.DisplayName}",
                                    TextContent = fileArtifact.GetTextContent(),
                                    TextLanguageSchema = DetermineLanguageSchema(templateArtifact.FilePath)
                                });
                                return;
                            }
                            else if (fileArtifact.HasDecorator<ImageContentDecorator>())
                            {
                                var image = fileArtifact.GetDecoratorOfType<ImageContentDecorator>()?.CreatePreview() as Image;
                                if (image != null) {
                                    WindowManagerService.ShowArtifactPreview(new ArtifactPreviewViewModel
                                    {
                                        FileName = fileArtifact.FileName,
                                        TabLabel = $"Generated: {templateArtifact.DisplayName}",
                                        //TextContent = fileArtifact.GetTextContent(),
                                        ImageContent = image,
                                        TextLanguageSchema = DetermineLanguageSchema(templateArtifact.FilePath)
                                    });
                                }
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(output.TextContent))
                    {
                        WindowManagerService.ShowArtifactPreview(new ArtifactPreviewViewModel
                        {
                            FileName = templateArtifact.FileName?.GetFileNameWithoutExtension()??"output.txt",
                            TabLabel = $"Generated: {templateArtifact.DisplayName}",
                            TextContent = output.TextContent,
                            TextLanguageSchema = DetermineLanguageSchema(templateArtifact.FilePath)
                        });
                    }

                    Logger.LogInformation("Template executed successfully: {TemplateName}", templateArtifact.DisplayName);
                }
                else
                {
                    var errorMessage = string.Join("\n", output.Errors);
                    MessageBoxService.ShowError($"Template execution failed:\n{errorMessage}", "Template Error");
                    Logger.LogError("Template execution failed: {Errors}", errorMessage);
                }
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowError($"Error executing template: {ex.Message}", "Template Error");
                Logger.LogError(ex, "Error executing template: {TemplateName}", templateArtifact.DisplayName);
            }
            finally
            {
                if (_parametersViewModel != null)
                {
                    _parametersViewModel.IsExecuting = false;
                }
            }
        }

        /// <summary>
        /// Process template parameters, loading data for TableArtifact parameters
        /// </summary>
        private async Task<Dictionary<string, object?>> ProcessTemplateParametersAsync(
            Dictionary<string, object?> parameters,
            TemplateArtifact templateArtifact,
            CancellationToken cancellationToken)
        {
            var result = new Dictionary<string, object?>();
            var templateParams = _parametersViewModel?.GetTemplateParameters() ?? templateArtifact.Parameters;

            foreach (var kvp in parameters)
            {
                var paramDef = templateParams.FirstOrDefault(p => p.Name == kvp.Key);

                // Check if this is a TableArtifactData parameter
                if (paramDef?.IsTemplateDatasourceArtifactData == true && kvp.Value is TemplateDatasourceArtifactItem tableItem)
                {
                    // Load data from the database, jsonfile, ...
                    var data = await LoadTemplateDatasourceDataAsync(
                        tableItem,
                        paramDef.TableDataFilter,
                        paramDef.TableDataMaxRows,
                        cancellationToken);
                    result[kvp.Key] = data;
                }
                else
                {
                    result[kvp.Key] = kvp.Value;
                }
            }

            return result;
        }

        /// <summary>
        /// Load data from a table in the database
        /// </summary>
        private async Task<IEnumerable<dynamic>> LoadTemplateDatasourceDataAsync(
            TemplateDatasourceArtifactItem tableItem,
            string? filter,
            int? maxRows,
            CancellationToken cancellationToken)
        {
            var datasourceDecorator = tableItem.DatasourceTargetArtifact.GetDecoratorLikeType<TemplateDatasourceProviderDecorator>();
            return await datasourceDecorator.LoadDataAsync(Logger, filter, maxRows, cancellationToken) ?? Enumerable.Empty<dynamic>();
        }

        /// <summary>
        /// Determine the language schema for syntax highlighting based on template file path
        /// </summary>
        private ArtifactPreviewViewModel.KnownLanguages DetermineLanguageSchema(string templateFilePath)
        {
            var fileName = Path.GetFileName(templateFilePath);

            // Check for patterns like "file.sql.scriban" -> SQL
            if (fileName.Contains(".sql.", StringComparison.OrdinalIgnoreCase))
                return ArtifactPreviewViewModel.KnownLanguages.SQL;
            if (fileName.Contains(".cs.", StringComparison.OrdinalIgnoreCase))
                return ArtifactPreviewViewModel.KnownLanguages.CSharp;
            if (fileName.Contains(".xml.", StringComparison.OrdinalIgnoreCase))
                return ArtifactPreviewViewModel.KnownLanguages.XML;
            if (fileName.Contains(".html.", StringComparison.OrdinalIgnoreCase))
                return ArtifactPreviewViewModel.KnownLanguages.HTML;
            if (fileName.Contains(".js.", StringComparison.OrdinalIgnoreCase))
                return ArtifactPreviewViewModel.KnownLanguages.JScript;
            if (fileName.Contains(".java.", StringComparison.OrdinalIgnoreCase))
                return ArtifactPreviewViewModel.KnownLanguages.Java;
            if (fileName.Contains(".c.", StringComparison.OrdinalIgnoreCase))
                return ArtifactPreviewViewModel.KnownLanguages.C;
            if (fileName.Contains(".ps1.", StringComparison.OrdinalIgnoreCase))
                return ArtifactPreviewViewModel.KnownLanguages.PowerShell;
            if (fileName.Contains(".vb.", StringComparison.OrdinalIgnoreCase))
                return ArtifactPreviewViewModel.KnownLanguages.VBNET;

            return ArtifactPreviewViewModel.KnownLanguages.Text;
        }

        public void Dispose()
        {
            if (TreeViewModel != null)
            {
                TreeViewModel.PropertyChanged -= TreeViewModel_PropertyChanged;
            }
        }

        public override void ShowArtifactDetailsView(ViewModelBase? detailsModel)
        {

            if (_artifactDetailsViewModel == null)
            {
                _artifactDetailsViewModel = new ArtifactDetailsViewModel();
            }
            _artifactDetailsViewModel.DetailsViewModel = detailsModel;
            WindowManagerService.ShowTemplateDetailsView(_artifactDetailsViewModel);
        }
    }
}
