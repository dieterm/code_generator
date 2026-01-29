using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Shared.Memento
{
    public abstract class MementoObjectFactory<T, TState> where T : IMementoObject where TState : IMementoState
    {
        protected MementoObjectFactory()
        {
        }
        public virtual T CreateMementoObject(TState state)
        {
            try
            {
                var type = Type.GetType(state.TypeName);
                if (state.Properties.ContainsKey("Name"))
                {
                    var name = state.Properties["Name"];
                    Debug.WriteLine($"Creating memento object of type '{state.TypeName}' with name '{name}'");
                } 
                else
                {
                    var id = state.Properties.ContainsKey("Id") ? state.Properties["Id"] : "(no id)";
                    Debug.WriteLine($"Creating memento object of type '{state.TypeName}' with id '{id}'");
                }

                if (type == null)
                {
                    throw new InvalidOperationException($"Type '{state.TypeName}' could not be found.");
                }
                if (!typeof(IMementoObject).IsAssignableFrom(type))
                {
                    throw new InvalidOperationException($"Type '{state.TypeName}' does not implement IMementoObject.");
                }
                var instance = (T)Activator.CreateInstance(type, state)!;

                return instance;
            }
            catch (Exception ex)
            {
                foreach(var prop in state.Properties)
                {
                    Debug.WriteLine($"  State Property: {prop.Key} = {prop.Value}");
                }
                throw;
            }
        }
    }
}
