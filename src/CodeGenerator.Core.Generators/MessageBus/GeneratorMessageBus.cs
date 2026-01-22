using CodeGenerator.Core.MessageBus;
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

}
