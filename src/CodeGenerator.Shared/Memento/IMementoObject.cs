using System.ComponentModel;

namespace CodeGenerator.Shared.Memento
{
    public interface IMementoObject
    {
        bool IsStateChanged { get; }
        Dictionary<string, object?> Properties { get; }

        event PropertyChangedEventHandler? PropertyChanged;
        event PropertyChangingEventHandler? PropertyChanging;

        T? GetValue<T>(string name, T? defaultValue = default);
        bool SetValue<T>(string name, T? value);

        void RestoreState(IMementoState state);

        IMementoState CaptureState();
    }
}