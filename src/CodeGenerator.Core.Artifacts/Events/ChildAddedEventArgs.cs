using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.Events
{
    public class ChildAddedEventArgs : EventArgs
    {
        public IArtifact ChildArtifact { get; }
        public ChildAddedEventArgs(IArtifact childArtifact)
        {
            ChildArtifact = childArtifact;
        }
    }
}
