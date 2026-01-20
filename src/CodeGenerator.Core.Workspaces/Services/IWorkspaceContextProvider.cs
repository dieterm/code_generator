using CodeGenerator.Core.Workspaces.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Services
{
    public interface IWorkspaceContextProvider
    {
        WorkspaceArtifact? CurrentWorkspace { get; }
    }
}
