using CodeGenerator.Core.Artifacts;

namespace CodeGenerator.Core.Generators.MessageBus
{
    public class CreatingArtifactEventArgs : GeneratorContextEventArgs
    {
        public CreatingArtifactEventArgs(GenerationResult result, IArtifact artifact) : base(result)
        {
            Artifact = artifact;
        }
        public IArtifact Artifact { get; }
    }
}
