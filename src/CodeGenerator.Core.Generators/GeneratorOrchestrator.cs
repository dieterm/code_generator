using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Services;
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
    private readonly SchemaParser _schemaparser;

    public GeneratorOrchestrator(
        IEnumerable<IMessageBusAwareGenerator> messageBusAwareGenerators,
        GeneratorMessageBus messageBus,
        SchemaParser schemaparser,
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
        string schemaFilePath, 
        bool previewOnly,
        IProgress<GenerationProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if(string.IsNullOrWhiteSpace(schemaFilePath))
            throw new ArgumentNullException(nameof(schemaFilePath));

        // report starting
        var startTime = DateTime.UtcNow;
        // create root artifact
        var result = new GenerationResult
        {
            RootArtifact = new RootArtifact(),
            GeneratedAt = DateTime.UtcNow
        };

        try
        {
            // load schema
            progress?.Report(new GenerationProgress
            {
                CurrentStep = "Loading Schema",
                Message = "Loading and parsing schema",
                PercentComplete = 0,
                Phase = GenerationPhase.Initializing,
                IsIndeterminate = true
            });
            
            var schema = await _schemaparser.LoadSchemaAsync(schemaFilePath, cancellationToken);

            progress.Report(new GenerationProgress
            {
                CurrentStep = "Creating Root Artifact",
                Message = "Initializing root artifact creation",
                PercentComplete = 0,
                Phase = GenerationPhase.Initializing,
                IsIndeterminate = true
            });

            // publish event to allow generators to contribute
            MessageBus.Publish(new CreatingRootArtifactEventArgs(result));
            progress.Report(new GenerationProgress
            {
                CurrentStep = "Finalizing Root Artifact",
                Message = "Finalizing root artifact creation",
                PercentComplete = previewOnly ? 80: 20,
                Phase = GenerationPhase.Finalizing,
                IsIndeterminate = true
            });
            // finalize root artifact if needed
            MessageBus.Publish(new CreatedRootArtifactEventArgs(result));
            
            if (!previewOnly)
            {
                progress.Report(new GenerationProgress
                {
                    CurrentStep = "Generating Root Artifact",
                    Message = "Generating root artifact",
                    PercentComplete = 0,
                    Phase = GenerationPhase.RunningGenerators,
                    IsIndeterminate = true
                });
                await result.RootArtifact.GenerateAsync();
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
