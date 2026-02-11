using CodeGenerator.Core.Artifacts;

namespace CodeGenerator.Application.Controllers.Base
{
    /// <summary>
    /// Event args for artifact renamed event
    /// </summary>
    public class ArtifactRenamedEventArgs : EventArgs
    {
        public IArtifact Artifact { get; }
        public string OldName { get; }
        public string NewName { get; }

        public ArtifactRenamedEventArgs(IArtifact artifact, string oldName, string newName)
        {
            Artifact = artifact;
            OldName = oldName;
            NewName = newName;
        }
    }
}
