using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.CodeGeneration;
using CodeGenerator.Core.DomainSchema.Services;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Settings.Generators;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Settings;
using CodeGenerator.Shared;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.Generators;

/// <summary>
/// Orchestrates code generation across all configured generators
/// </summary>
public class GeneratorOrchestrator
{
    private readonly IEnumerable<IMessageBusAwareGenerator> _messageBusAwareGenerators;
    private readonly GeneratorMessageBus _messageBus;
    private readonly ILogger<GeneratorOrchestrator> _logger;

    public GeneratorOrchestrator(
        IEnumerable<IMessageBusAwareGenerator> messageBusAwareGenerators,
        GeneratorMessageBus messageBus,
        ILogger<GeneratorOrchestrator> logger)
    {
        _messageBus = messageBus;
        _logger = logger;
        _messageBusAwareGenerators = messageBusAwareGenerators;
        _messageBus.BeforeEventPublished += _messageBus_BeforeEventPublished;
        _messageBus.AfterEventPublished += _messageBus_AfterEventPublished;
    }

    private void _messageBus_AfterEventPublished(object? sender, GeneratorContextEventArgs e)
    {
        if(e is CreatedArtifactEventArgs createdArtifactEventArgs)
        {
            _logger.LogDebug("AfterEventPublished: Artifact created: '{ArtifactId}' of type {ArtifactType} - {ArtifactContent}", 
                createdArtifactEventArgs.Artifact.Id, 
                createdArtifactEventArgs.Artifact.GetType().Name,
                createdArtifactEventArgs.Artifact.ToString());
        } 
        else if(e is CreatingArtifactEventArgs creatingArtifactEventArgs)
        {
            _logger.LogDebug("AfterEventPublished: Creating artifact: '{ArtifactId}' of type {ArtifactType} - {ArtifactContent}", 
                creatingArtifactEventArgs.Artifact.Id, 
                creatingArtifactEventArgs.Artifact.GetType().Name,
                creatingArtifactEventArgs.Artifact.ToString());
        } 
        else
        {
            _logger.LogDebug("AfterEventPublished: Published event of type {EventType}", e.GetType().Name);
        }
    }

    private void _messageBus_BeforeEventPublished(object? sender, GeneratorContextEventArgs e)
    {
        if (e is CreatedArtifactEventArgs createdArtifactEventArgs)
        {
            _logger.LogDebug("BeforeEventPublished:Artifact created: '{ArtifactId}' of type {ArtifactType} - {ArtifactContent}",
                createdArtifactEventArgs.Artifact.Id,
                createdArtifactEventArgs.Artifact.GetType().Name,
                createdArtifactEventArgs.Artifact.ToString());
        }
        else if (e is CreatingArtifactEventArgs creatingArtifactEventArgs)
        {
            _logger.LogDebug("BeforeEventPublished: Creating artifact: '{ArtifactId}' of type {ArtifactType} - {ArtifactContent}",
                creatingArtifactEventArgs.Artifact.Id,
                creatingArtifactEventArgs.Artifact.GetType().Name,
                creatingArtifactEventArgs.Artifact.ToString());
        }
        else
        {
            _logger.LogDebug("BeforeEventPublished: Publishing event of type {EventType}", e.GetType().Name);
        }
    }
    /// <summary>
    /// The message bus for event communication
    /// </summary>
    public GeneratorMessageBus MessageBus => _messageBus;

    /// <summary>
    /// Initialize all message bus aware generators
    /// </summary>
    public void Initialize()
    {
        // first initialize generators
        foreach (var generator in _messageBusAwareGenerators)
        {
            generator.Initialize(_messageBus);
            
            _logger.LogDebug("Initialized message bus aware generator: {GeneratorName}", generator.SettingsDescription.Name);
        }

        // then subscribe to events
        foreach (var generator in _messageBusAwareGenerators)
        {
            generator.SubscribeToEvents(_messageBus);

            _logger.LogDebug("Initialized message bus aware generator: {GeneratorName}", generator.SettingsDescription.Name);
        }
    }

    private void UnsubscribeGenerators()
    {
        foreach (var generator in _messageBusAwareGenerators)
        {
            generator.UnsubscribeFromEvents(_messageBus);
        }
    }
    public GenerationResult? CurrentGenerationResult { get; private set; }
    /// <summary>
    /// Run all enabled generators
    /// </summary>
    public async Task<GenerationResult> GenerateAsync(
        WorkspaceArtifact workspaceArtifact, 
        bool previewOnly,
        IProgress<GenerationProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        // report starting
        var startTime = DateTime.UtcNow;
        
        // create root artifact
        var rootArtifact = new RootArtifact("Output", workspaceArtifact.OutputDirectory);
        var result = new GenerationResult(rootArtifact, workspaceArtifact, previewOnly);

        // first check if output directory is set
        if (string.IsNullOrWhiteSpace(workspaceArtifact.OutputDirectory))
        {
            result.Errors.Add($"Workspace OutputDirectory not set.");
            return result;
        }

        CurrentGenerationResult = result;
        try
        {
            progress?.Report(new GenerationProgress
            {
                CurrentStep = "Creating Root Artifact",
                Message = "Initializing root artifact creation",
                PercentComplete = 0,
                Phase = GenerationPhase.Initializing,
                IsIndeterminate = true
            });

            // publish event to allow generators to contribute
            MessageBus.Publish(new CreatingArtifactEventArgs(result, result.RootArtifact));
            progress?.Report(new GenerationProgress
            {
                CurrentStep = "Finalizing Root Artifact",
                Message = "Finalizing root artifact creation",
                PercentComplete = previewOnly ? 80: 20,
                Phase = GenerationPhase.Finalizing,
                IsIndeterminate = true
            });
            // finalize root artifact if needed
            MessageBus.Publish(new CreatedArtifactEventArgs(result, result.RootArtifact));
            
            if (!previewOnly)
            {
                progress?.Report(new GenerationProgress
                {
                    CurrentStep = "Generating Root Artifact",
                    Message = "Generating root artifact",
                    PercentComplete = 0,
                    Phase = GenerationPhase.RunningGenerators,
                    IsIndeterminate = true
                });
                var progressHandler = new ArtifactGenerationProgressHandler(progress);
                await result.RootArtifact.GenerateAsync(progressHandler, cancellationToken);
            }

            result.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while publishing root artifact creation event");
            result.Errors.Add($"Error during root artifact creation: {ex.Message}");
            result.Success = false;
        }
        finally
        {
            MessageBus.Publish(new GenerationCompletedEventArgs(result));

            var endTime = DateTime.UtcNow;
            result.Duration = endTime - startTime;
            CurrentGenerationResult = null;

            progress?.Report(new GenerationProgress
            {
                CurrentStep = "Completed",
                Message = "Root artifact generation completed",
                PercentComplete = 100,
                Phase = GenerationPhase.Completed,
                IsIndeterminate = false
            });
        }

        return result;
    }
}
