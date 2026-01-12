using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts
{
    public class ParentChangedEventArgs : EventArgs
    {
        public IArtifact? OldParent { get; }
        public IArtifact? NewParent { get; }
        public ParentChangedEventArgs(IArtifact? oldParent, IArtifact? newParent)
        {
            OldParent = oldParent;
            NewParent = newParent;
        }
    }
}
