using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Shared.Memento
{
    public abstract class MementoObjectBase<TState> : ObjectBase.ObjectBase, INotifyPropertyChanged, INotifyPropertyChanging, IMementoObject where TState : IMementoState, new()
    {
        public bool IsStateChanged { get; private set; } = false;

        protected MementoObjectBase(IMementoState state)
            : this()
        {
            RestoreState(state);
        }

        protected MementoObjectBase()
            : base()
        {

        }

        protected override void RaisePropertyChangedEvent(string propertyName)
        {
            IsStateChanged = true;
            base.RaisePropertyChangedEvent(propertyName);
        }

        public virtual void RestoreState(IMementoState state)
        {
            _properties.Clear();
            foreach (var kvp in state.Properties)
            {
                _properties[kvp.Key] = kvp.Value;
            }
            ResetIsStateChangedFlag();
        }

        public virtual IMementoState CaptureState()
        {
            TState state = new TState();
            state.TypeName = this.GetType().AssemblyQualifiedName!;
            foreach (var kvp in _properties)
            {
                state.Properties[kvp.Key] = kvp.Value;
            }
            
            return state;
        }

        public void ResetIsStateChangedFlag()
        {
            IsStateChanged = false;
        }
    }
}
