using CodeGenerator.Core.Copilot.ViewModels;
using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Copilot.Services
{
    public interface ICopilotWindowManagerService
    {
        void ShowCopilotChatView(CopilotChatViewModel viewModel);
    }
}
