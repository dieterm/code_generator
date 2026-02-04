namespace CodeGenerator.Domain.CodeArchitecture
{
    public interface ICodeArchitecture
    {
        string Id { get; }
        string Name { get; }

        IEnumerable<ICodeArchitectureLayerFactory> Layers { get; }
    }
}