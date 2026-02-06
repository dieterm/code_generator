using CodeGenerator.Core.Artifacts;

namespace CodeGenerator.Domain.CodeArchitecture
{
    public interface ICodeArchitecture
    {
        string Id { get; }
        string Name { get; }
        IScopeArtifactFactory ScopeFactory { get; }
        IEnumerable<ICodeArchitectureLayerFactory> Layers { get; }
    }
}