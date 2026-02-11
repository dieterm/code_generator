using CodeGenerator.Core.Artifacts;

namespace CodeGenerator.Application.Controllers.Base
{
    /// <summary>
    /// Event args for artifact property changed event
    /// </summary>
    public class ArtifactPropertyChangedEventArgs : EventArgs
    {
        public IArtifact Artifact { get; }
        public string PropertyName { get; }
        public object? NewValue { get; }

        public ArtifactPropertyChangedEventArgs(IArtifact artifact, string propertyName, object? newValue)
        {
            Artifact = artifact;
            PropertyName = propertyName;
            NewValue = newValue;
        }
    }
}
