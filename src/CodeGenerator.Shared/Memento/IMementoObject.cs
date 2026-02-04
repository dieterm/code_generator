using CodeGenerator.Shared.ObjectBase;
using System.ComponentModel;

namespace CodeGenerator.Shared.Memento
{
    public interface IMementoObject : IObjectBase
    {
        void ResetIsStateChangedFlag();
        bool IsStateChanged { get; }

        void RestoreState(IMementoState state);

        IMementoState CaptureState();
    }
}