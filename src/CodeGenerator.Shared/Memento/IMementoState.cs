
namespace CodeGenerator.Shared.Memento
{
    public interface IMementoState : ICloneable
    {
        Dictionary<string, object?> Properties { get; set; }
        string TypeName { get; set; }
    }
}