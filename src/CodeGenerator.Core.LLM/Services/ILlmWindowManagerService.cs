using CodeGenerator.Core.LLM.ViewModels;

namespace CodeGenerator.Core.LLM.Services
{
    public interface ILlmWindowManagerService
    {
        void ShowLlmChatView(LlmChatViewModel viewModel);
    }
}
