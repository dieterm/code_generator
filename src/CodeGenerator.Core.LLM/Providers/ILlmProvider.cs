using CodeGenerator.Core.LLM.Settings;
using CodeGenerator.Core.Settings;
using CodeGenerator.Core.Settings.Models;
using Microsoft.Extensions.AI;

namespace CodeGenerator.Core.LLM.Providers
{
    /// <summary>
    /// Abstraction for an LLM provider (e.g. GitHub Copilot, Ollama, OpenAI, etc.)
    /// Each provider can connect, create sessions, and expose tools.
    /// </summary>
    public interface ILlmProvider : IAsyncDisposable, IDisposable
    {
        /// <summary>Unique identifier of the provider (e.g. "GitHubCopilot", "Ollama")</summary>
        string ProviderId { get; }

        /// <summary>Display name for UI (e.g. "GitHub Copilot", "Ollama (Local)")</summary>
        string DisplayName { get; }

        /// <summary>Whether the provider is currently connected and ready</summary>
        bool IsConnected { get; }

        /// <summary>Connect to the LLM backend</summary>
        Task ConnectAsync(CancellationToken cancellationToken = default);

        /// <summary>Create a chat session with the given tools and system prompt</summary>
        Task<ILlmChatSession> CreateSessionAsync(LlmSessionConfig config, CancellationToken cancellationToken = default);

        List<ISettingsItem> GetSettingsItems(LlmProviderSettings settings);
    }
}
