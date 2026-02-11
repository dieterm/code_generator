using CodeGenerator.Core.Artifacts.ViewModels;
using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.CodeElements.Services
{
    public interface ICodeElementsWindowManagerService
    {
        void ShowCodeElementsDetailsView(CodeElementArtifactDetailsViewModel detailsViewModel);


        void ShowCodeElementsEditor(CodeElementsEditorViewModel editorViewModel);


        void ShowCodeElementsTreeView(CodeElementsTreeViewModel treeViewModel);
    }
}
