using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.CodeGeneration;
using CodeGenerator.Core.DomainSchema.Services;
using CodeGenerator.Core.Generators.MessageBus;
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
    private readonly DomainSchemaParser _schemaparser;

    public GeneratorOrchestrator(
        IEnumerable<IMessageBusAwareGenerator> messageBusAwareGenerators,
        GeneratorMessageBus messageBus,
        DomainSchemaParser schemaparser,
        ILogger<GeneratorOrchestrator> logger)
    {
        _messageBus = messageBus;
        _logger = logger;
        _messageBusAwareGenerators = messageBusAwareGenerators;
        _schemaparser = schemaparser;
        InitializeGenerators();
    }

    /// <summary>
    /// The message bus for event communication
    /// </summary>
    public GeneratorMessageBus MessageBus => _messageBus;

    private void InitializeGenerators()
    {
        foreach (var generator in _messageBusAwareGenerators)
        {
            generator.Initialize(_messageBus);
            generator.SubscribeToEvents(_messageBus);
            _logger.LogDebug("Initialized message bus aware generator: {GeneratorName}", generator.SettingsDescription.Name);
        }
    }

    /// <summary>
    /// Run all enabled generators
    /// </summary>
    public async Task<GenerationResult> GenerateAsync(
        DomainSchema.Schema.DomainSchema? domainSchema, 
        bool previewOnly,
        IProgress<GenerationProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        // report starting
        var startTime = DateTime.UtcNow;
        // create root artifact
        var result = new GenerationResult(new RootArtifact(), domainSchema);
        
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

        var endTime = DateTime.UtcNow;
        result.Duration = endTime - startTime;

        progress.Report(new GenerationProgress
        {
            CurrentStep = "Completed",
            Message = "Root artifact generation completed",
            PercentComplete = 100,
            Phase = GenerationPhase.Completed,
            IsIndeterminate = false
        });

        return result;
    }
}
