using CodeGenerator.Core.Artifacts.ViewModels;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.Controllers;
using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Controllers.CodeElements
{
    public interface ICodeElementsController : IControllerBase
    {
        void ShowCodeElements(CodeFileElement codeFileElement);
        void ShowCodeElements();
    }
}
