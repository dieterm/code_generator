
namespace CodeGenerator.Shared.Memento
{
    public interface IMementoState
    {
        Dictionary<string, object?> Properties { get; set; }
        string TypeName { get; set; }
    }
}