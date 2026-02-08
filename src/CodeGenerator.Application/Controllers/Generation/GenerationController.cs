using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.ArtifactPreview;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Application.ViewModels.Generation;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Presentation.WinForms.ViewModels;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ExtensionMethods;
using CodeGenerator.Shared.Ribbon;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Generation
{
    public class GenerationController : CoreControllerBase, IProgress<GenerationProgress>
    {
        private readonly GenerationResultTreeViewModel _treeViewModel;
        private readonly GenerationRibbonViewModel _generationRibbonViewModel;
        private readonly GenerationTreeViewController _generationTreeViewController;
        private readonly GenerationDetailsViewModel _detailsViewModel;
        private readonly GeneratorOrchestrator _generatorOrchestrator;

        public GenerationController(
            GeneratorOrchestrator generatorOrchestrator,
            GenerationTreeViewController generationTreeViewController,
            GenerationDetailsViewModel detailsViewModel,
            GenerationResultTreeViewModel treeViewModel,
            GenerationRibbonViewModel generationRibbonViewModel,
            IWindowManagerService windowManagerService,
            RibbonBuilder ribbonBuilder,
            IMessageBoxService messageService,
            ApplicationMessageBus messageBus,
            IFileSystemDialogService fileSystemDialogService,
            ILogger<GenerationController> logger)
            : base(windowManagerService, ribbonBuilder, messageBus, messageService, fileSystemDialogService, logger)
        {
            _generatorOrchestrator = generatorOrchestrator ?? throw new ArgumentNullException(nameof(generatorOrchestrator));
            _generationTreeViewController = generationTreeViewController ?? throw new ArgumentNullException(nameof(generationTreeViewController));
            _detailsViewModel = detailsViewModel ?? throw new ArgumentNullException(nameof(detailsViewModel));
            _treeViewModel = treeViewModel ?? throw new ArgumentNullException(nameof(treeViewModel));
            _generationRibbonViewModel = generationRibbonViewModel ?? throw new ArgumentNullException(nameof(generationRibbonViewModel));
        }

        public override void Initialize()
        {
            _generationTreeViewController.Initialize();

            // Wire up the sub-viewmodels
            _treeViewModel.TreeViewModel = _generationTreeViewController.TreeViewModel;
            _treeViewModel.DetailsViewModel = _detailsViewModel;

            // Subscribe to tree selection events for backward compatibility
            _generationTreeViewController.ArtifactSelected += OnArtifactSelected;

            _generationRibbonViewModel.StartGenerationRequested += OnGenerateRequested;
            _generationRibbonViewModel.StartPreviewRequested += OnGeneratePreviewRequested;
            _generationRibbonViewModel.CancelGenerationRequested += OnCancelGenerationRequested;

            _generatorOrchestrator.Initialize();
        }

        public void ShowGenerationResultView()
        {
            _windowManagerService.ShowGenerationTreeView(_treeViewModel);
        }

        private void OnArtifactSelected(object? sender, IArtifact? artifact)
        {
            if (artifact == null) return;

            _treeViewModel.SelectedArtifact = artifact;
            _detailsViewModel.SelectedArtifact = artifact;
        }

        public async Task GenerateAsync(IProgress<GenerationProgress> progress, CancellationToken cancellationToken)
        {
            try
            {
                _generationRibbonViewModel.IsGenerating = true;

                var workspaceContextProvider = ServiceProviderHolder.GetRequiredService<IWorkspaceContextProvider>();
                if (workspaceContextProvider.CurrentWorkspace == null)
                    return;

                _logger.LogInformation("Starting code generation process...");
                var generationResult = await _generatorOrchestrator.GenerateAsync(workspaceContextProvider.CurrentWorkspace, false, progress, cancellationToken);
                _treeViewModel.GenerationResult = generationResult;
                _generationTreeViewController.LoadGenerationResult(generationResult);
                ShowGenerationResultView();
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
                _generationTreeViewController.LoadGenerationResult(generationResult);
                ShowGenerationResultView();
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
            _generationTreeViewController.ArtifactSelected -= OnArtifactSelected;
            _generationRibbonViewModel.StartGenerationRequested -= OnGenerateRequested;
            _generationRibbonViewModel.StartPreviewRequested -= OnGeneratePreviewRequested;
            _generationRibbonViewModel.CancelGenerationRequested -= OnCancelGenerationRequested;
        }

        public void Report(GenerationProgress value)
        {
            _messageBus.ReportApplicationStatus(value.CurrentStep);
            _messageBus.ReportTaskProgress(value.Message, value.IsIndeterminate ? null : value.PercentComplete);
        }
    }
}