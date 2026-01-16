using CodeGenerator.Core.Artifacts;

namespace CodeGenerator.Application.Controllers.Base
{
    /// <summary>
    /// Event args for artifact child added/removed events
    /// </summary>
    public class ArtifactChildChangedEventArgs : EventArgs
    {
        public IArtifact Parent { get; }
        public IArtifact Child { get; }

        public ArtifactChildChangedEventArgs(IArtifact parent, IArtifact child)
        {
            Parent = parent;
            Child = child;
        }
    }
}
