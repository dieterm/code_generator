using CodeGenerator.Core.Artifacts.CodeGeneration;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Shared;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.Generators;

/// <summary>
/// Default implementation of the generator message bus
/// </summary>
public class GeneratorMessageBus : MessageBus<GeneratorContextEventArgs>
{
    public GeneratorMessageBus(ILogger<GeneratorMessageBus>? logger = null)
         : base(logger)
    {
        
    }

    public string RequestPlaceholderContent(string placeholderName)
    {
        var result = ServiceProviderHolder.GetRequiredService<GeneratorOrchestrator>().CurrentGenerationResult;
        if(result==null) throw new InvalidOperationException("No current generation result available to request placeholder content.");
        var eventArgs = new RequestingPlaceholderContentEventArgs(placeholderName, result);
        Publish(eventArgs);
        return string.Join(Environment.NewLine, eventArgs.Content.Values);
    }

    public void RootArtifactCreated()
    {
        var result = ServiceProviderHolder.GetRequiredService<GeneratorOrchestrator>().CurrentGenerationResult;
        if(result==null) throw new InvalidOperationException("No current generation result available to publish RootArtifactCreated event.");
        var eventArgs = new RootArtifactCreatedEventArgs(result.RootArtifact, result);
        Publish(eventArgs);
    }
}
