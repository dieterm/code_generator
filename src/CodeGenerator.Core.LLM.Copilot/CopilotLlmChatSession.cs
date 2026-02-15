using CodeGenerator.Core.LLM.Providers;
using GitHub.Copilot.SDK;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.LLM.Copilot
{
    /// <summary>
    /// Wraps a CopilotSession as an ILlmChatSession
    /// </summary>
    internal class CopilotLlmChatSession : ILlmChatSession
    {
        private readonly CopilotSession _session;
        private readonly ILogger _logger;

        public string SessionId => _session.SessionId;

        public CopilotLlmChatSession(CopilotSession session, ILogger logger)
        {
            _session = session;
            _logger = logger;
        }

        public async Task SendAsync(string prompt, Action<LlmChatEvent> onEvent, CancellationToken cancellationToken = default)
        {
            var done = new TaskCompletionSource();

            var subscription = _session.On(ev =>
            {
                if (ev is AssistantMessageDeltaEvent deltaEvent)
                {
                    onEvent(new LlmMessageDeltaEvent(deltaEvent.Data.DeltaContent ?? ""));
                }
                else if (ev is AssistantMessageEvent msgEvent)
                {
                    onEvent(new LlmMessageCompleteEvent(msgEvent.Data.Content ?? ""));
                }
                else if (ev is SessionIdleEvent)
                {
                    onEvent(new LlmSessionIdleEvent());
                    done.TrySetResult();
                }
                else if (ev is PendingMessagesModifiedEvent pendingMessagesModifiedEvent)
                {
                    _logger.LogInformation("Pending messages modified: {Data} pending messages", pendingMessagesModifiedEvent.Data.ToString());
                }
                else
                {
                    _logger.LogWarning("Received unexpected event type: {EventType}", ev.GetType().Name);
                }
            });

            try
            {
                await _session.SendAsync(new MessageOptions { Prompt = prompt });
                await done.Task;
            }
            finally
            {
                subscription.Dispose();
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _session.DisposeAsync();
        }
    }
}
