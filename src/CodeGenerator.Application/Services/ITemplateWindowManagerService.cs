using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Application.ViewModels.Workspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Services
{
    public interface ITemplateWindowManagerService
    {
        void ShowTemplateTreeView(TemplateTreeViewModel treeViewModel);
        void ShowTemplateDetailsView(WorkspaceArtifactDetailsViewModel artifactDetailsViewModel);
        void ShowScribanTemplateEditView(ScribanTemplateEditViewModel viewModel);
    }
}
