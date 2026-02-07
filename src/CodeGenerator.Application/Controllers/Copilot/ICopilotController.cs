using CodeGenerator.Shared.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Controllers.Copilot
{
    public interface ICopilotController : IControllerBase
    {
        void ShowCopilot();
    }
}
