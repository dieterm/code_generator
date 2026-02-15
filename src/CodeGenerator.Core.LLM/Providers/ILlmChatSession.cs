namespace CodeGenerator.Core.LLM.Providers
{
    /// <summary>
    /// Represents an active chat session with an LLM provider.
    /// Supports sending messages and receiving streamed or complete responses.
    /// </summary>
    public interface ILlmChatSession : IAsyncDisposable
    {
        /// <summary>Unique session identifier</summary>
        string SessionId { get; }

        /// <summary>
        /// Send a user message and receive the response via the event callback.
        /// The callback is invoked for each event (delta, complete, idle).
        /// The method returns when the LLM has finished processing.
        /// </summary>
        Task SendAsync(string prompt, Action<LlmChatEvent> onEvent, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Events emitted by an LLM chat session
    /// </summary>
    public abstract class LlmChatEvent
    {
    }

    /// <summary>Streamed content delta (partial response)</summary>
    public class LlmMessageDeltaEvent : LlmChatEvent
    {
        public string DeltaContent { get; }

        public LlmMessageDeltaEvent(string deltaContent)
        {
            DeltaContent = deltaContent;
        }
    }

    /// <summary>Complete message (non-streaming or final)</summary>
    public class LlmMessageCompleteEvent : LlmChatEvent
    {
        public string Content { get; }

        public LlmMessageCompleteEvent(string content)
        {
            Content = content;
        }
    }

    /// <summary>A tool call was initiated by the LLM</summary>
    public class LlmToolCallEvent : LlmChatEvent
    {
        public string ToolName { get; }
        public string Arguments { get; }

        public LlmToolCallEvent(string toolName, string arguments)
        {
            ToolName = toolName;
            Arguments = arguments;
        }
    }

    /// <summary>A tool call completed with a result</summary>
    public class LlmToolResultEvent : LlmChatEvent
    {
        public string ToolName { get; }
        public string Result { get; }

        public LlmToolResultEvent(string toolName, string result)
        {
            ToolName = toolName;
            Result = result;
        }
    }

    /// <summary>Session has finished processing and is idle</summary>
    public class LlmSessionIdleEvent : LlmChatEvent
    {
    }
}
