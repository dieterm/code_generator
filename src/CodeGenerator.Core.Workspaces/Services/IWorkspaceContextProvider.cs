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
        event EventHandler? WorkspaceHasUnsavedChangesChanged;

        bool HasUnsavedChanges { get; }
        /// <summary>
        /// Event raised when the workspace changes
        /// </summary>
        event EventHandler<WorkspaceArtifact?>? WorkspaceChanged;

        /// <summary>
        /// The currently active workspace
        /// </summary>
        WorkspaceArtifact? CurrentWorkspace { get; }
    }
}
