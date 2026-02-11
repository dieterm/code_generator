using CodeGenerator.Application.ViewModels.Workspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Services
{
    public interface IWorkspaceWindowManagerService
    {
        void ShowWorkspaceTreeView(WorkspaceTreeViewModel treeViewModel);
        void ShowWorkspaceDetailsView(WorkspaceArtifactDetailsViewModel viewModel);
    }
}
