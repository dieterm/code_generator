using Microsoft.Extensions.AI;

namespace CodeGenerator.Core.LLM.Providers
{
    /// <summary>
    /// Configuration for creating an LLM chat session
    /// </summary>
    public class LlmSessionConfig
    {
        /// <summary>Model identifier (e.g. "gpt-4.1", "llama3.2")</summary>
        public string? Model { get; set; }

        /// <summary>Whether to use streaming responses</summary>
        public bool Streaming { get; set; } = true;

        /// <summary>System prompt to prepend to the conversation</summary>
        public string? SystemMessage { get; set; }

        /// <summary>AI tools available for tool-calling</summary>
        public List<AIFunction> Tools { get; set; } = new();
    }
}
