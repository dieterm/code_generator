using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Application.ViewModels.Generation;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.DomainSchema.Schema;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Presentation.WinForms.ViewModels;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ExtensionMethods;
using CodeGenerator.Shared.Ribbon;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Controllers
{
    public class GenerationController : CoreControllerBase, IProgress<GenerationProgress>
    {
        private readonly GenerationResultTreeViewModel _treeViewModel;
        private readonly GenerationRibbonViewModel _generationRibbonViewModel;
        private readonly ArtifactPreviewViewModel _artifactPreviewViewModel;

        public GenerationController(ArtifactPreviewViewModel artifactPreviewViewModel, GenerationResultTreeViewModel treeViewModel, GenerationRibbonViewModel generationRibbonViewModel, IWindowManagerService windowManagerService, RibbonBuilder ribbonBuilder, IMessageBoxService messageService, ApplicationMessageBus messageBus, IFileSystemDialogService fileSystemDialogService, ILogger<GenerationController> logger)
            : base(windowManagerService, ribbonBuilder, messageBus, messageService, fileSystemDialogService,logger)
        {
            _treeViewModel = treeViewModel ?? throw new ArgumentNullException(nameof(treeViewModel));
            _artifactPreviewViewModel = artifactPreviewViewModel ?? throw new ArgumentNullException(nameof(artifactPreviewViewModel));
            _generationRibbonViewModel = generationRibbonViewModel ?? throw new ArgumentNullException(nameof(generationRibbonViewModel));
        }

        public override void Initialize()
        {
            _treeViewModel.ArtifactSelected += OnArtifactSelected;

            _generationRibbonViewModel.StartGenerationRequested += OnGenerateRequested;
            _generationRibbonViewModel.StartPreviewRequested += OnGeneratePreviewRequested;
            _generationRibbonViewModel.CancelGenerationRequested += OnCancelGenerationRequested;
        }

        private void OnArtifactSelected(object? sender, GenerationResultTreeViewModel.ArtifactSelectedEventArgs e)
        {
            var selectedArtifact = e.SelectedArtifact;
            _treeViewModel.SelectedArtifact = selectedArtifact;

            // Artifact Preview
            if (selectedArtifact != null)
            {
                if(selectedArtifact.CanPreview)
                {
                    object? previewOutput = selectedArtifact.CreatePreview();

                    if (previewOutput is string) { 
                        _artifactPreviewViewModel.TextContent = previewOutput as string;
                        var fileExtension = selectedArtifact.GetDecoratorOfType<FileArtifactDecorator>()?.FileName.GetFileExtension();
                        switch (fileExtension)
                        {
                            case ".cs":
                                _artifactPreviewViewModel.TextLanguageSchema = ArtifactPreviewViewModel.KnownLanguages.CSharp;
                                break;
                            case ".js":
                                _artifactPreviewViewModel.TextLanguageSchema = ArtifactPreviewViewModel.KnownLanguages.JScript;
                                break;
                            case ".ts":
                                _artifactPreviewViewModel.TextLanguageSchema = ArtifactPreviewViewModel.KnownLanguages.CSharp;
                                break;
                            case ".html":
                            case ".htm":
                                _artifactPreviewViewModel.TextLanguageSchema = ArtifactPreviewViewModel.KnownLanguages.HTML;
                                break;
                            case ".xml":
                            case ".csproj":
                            case ".sln":
                            case ".targets":
                                _artifactPreviewViewModel.TextLanguageSchema = ArtifactPreviewViewModel.KnownLanguages.XML;
                                break;
                            case ".json":
                                _artifactPreviewViewModel.TextLanguageSchema = ArtifactPreviewViewModel.KnownLanguages.Undefined;
                                break;
                            case ".sql":
                                _artifactPreviewViewModel.TextLanguageSchema = ArtifactPreviewViewModel.KnownLanguages.SQL;
                                break;
                            case ".java":
                                _artifactPreviewViewModel.TextLanguageSchema = ArtifactPreviewViewModel.KnownLanguages.Java;
                                break;
                            case ".py":
                                _artifactPreviewViewModel.TextLanguageSchema = ArtifactPreviewViewModel.KnownLanguages.Undefined;
                                break;
                            default:
                                _artifactPreviewViewModel.TextLanguageSchema = ArtifactPreviewViewModel.KnownLanguages.Text;
                                break;
                        }
                    }
                    else
                        throw new NotImplementedException("Cannot preview Artifact");

                    _windowManagerService.ShowArtifactPreview(_artifactPreviewViewModel);
                }
            }
        }

        public async Task GenerateAsync(IProgress<GenerationProgress> progress, CancellationToken cancellationToken)
        {
            
            try
            {
                _generationRibbonViewModel.IsGenerating = true;

                var generatorOrchestrator = ServiceProviderHolder.GetRequiredService<GeneratorOrchestrator>();
                var workspaceContextProvider = ServiceProviderHolder.GetRequiredService<IWorkspaceContextProvider>();
                _logger.LogInformation("Starting code generation process...");
                var generationResult = await generatorOrchestrator.GenerateAsync(workspaceContextProvider.CurrentWorkspace, false, progress, cancellationToken);
                _treeViewModel.GenerationResult = generationResult;
                _windowManagerService.ShowGenerationTreeView(_treeViewModel);
                _logger.LogInformation("Code generation process completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the code generation process.");
                throw;
            }
            finally
            {
                _generationRibbonViewModel.IsGenerating = false;
            }
        }

        public async Task GeneratePreviewAsync(IProgress<GenerationProgress> progress, CancellationToken cancellationToken)
        {
            try
            {
                _generationRibbonViewModel.IsGenerating = true;
                var generatorOrchestrator = ServiceProviderHolder.GetRequiredService<GeneratorOrchestrator>();
                var workspaceContextProvider = ServiceProviderHolder.GetRequiredService<IWorkspaceContextProvider>();
                _logger.LogInformation("Starting code generation preview process...");
                var generationResult = await generatorOrchestrator.GenerateAsync(workspaceContextProvider.CurrentWorkspace, true, progress, cancellationToken);
                _treeViewModel.GenerationResult = generationResult;
                _windowManagerService.ShowGenerationTreeView(_treeViewModel);
                _logger.LogInformation("Code generation preview process completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the code generation preview process.");
                throw;
            }
            finally
            {
                _generationRibbonViewModel.IsGenerating = false;
            }
        }
        public void CreateRibbon()
        {
            var generationTab = _ribbonBuilder.AddTab("tabGeneration", "Generation");

            generationTab
                .AddToolStrip("toolstripGeneration", "Generation")
                    .AddButton("btnGenerate", "Generate")
                        .WithSize(RibbonButtonSize.Large)
                        .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                        .WithImage("scroll_text")
                        .WithCommand(_generationRibbonViewModel.StartGenerationCommand)
                    .AddButton("btnGeneratePreview", "Preview")
                        .WithSize(RibbonButtonSize.Large)
                        .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                        .WithImage("eye")
                        .WithCommand(_generationRibbonViewModel.StartPreviewCommand)
                    .AddButton("btnCancelGeneration", "Cancel")
                        .WithSize(RibbonButtonSize.Large)
                        .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                        .WithImage("x")
                        .HideWhenDisabled()
                        .WithCommand(_generationRibbonViewModel.CancelGenerationCommand)
                 .Build();

            generationTab.Build();
        }

        private CancellationTokenSource? _generationCancellationTokenSource;

        /// <summary>
        /// Handle code generation logic. <br/>
        /// Triggered when user clicks "Generate" button
        /// </summary>
        private async void OnGenerateRequested(object? sender, EventArgs e)
        {
            if (_generationCancellationTokenSource != null)
            {
                _messageBoxService.ShowWarning("Generation is already in progress.", "Generation In Progress");
                return;
            }
            _generationCancellationTokenSource = new CancellationTokenSource();
            await GenerateAsync(this, _generationCancellationTokenSource.Token);
            _generationCancellationTokenSource = null;
        }

        private async void OnGeneratePreviewRequested(object? sender, EventArgs e)
        {
            if (_generationCancellationTokenSource != null)
            {
                _messageBoxService.ShowWarning("Generation is already in progress.", "Generation In Progress");
                return;
            }
            _generationCancellationTokenSource = new CancellationTokenSource();
            await GeneratePreviewAsync(this, _generationCancellationTokenSource.Token);
            _generationCancellationTokenSource = null;
        }

        private void OnCancelGenerationRequested(object? sender, EventArgs e)
        {
            if (_generationCancellationTokenSource != null)
            {
                _generationCancellationTokenSource.Cancel();
                _generationCancellationTokenSource = null;
                _generationRibbonViewModel.IsGenerating = false;
                _messageBus.ReportApplicationStatus("Generation cancelled");
            }
        }

        public override void Dispose()
        {
            // Clean up resources if needed
            _generationRibbonViewModel.StartGenerationRequested -= OnGenerateRequested;
        }

        public void Report(GenerationProgress value)
        {
            _messageBus.ReportApplicationStatus(value.CurrentStep);
            _messageBus.ReportTaskProgress(value.Message, value.IsIndeterminate ? null : value.PercentComplete);
        }
    }
}