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
        public virtual T? CreateMementoObject(TState state, List<string> errors)
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
                    errors.Add($"Type '{state.TypeName}' could not be found.");
                    return default;
                }
                if (!typeof(IMementoObject).IsAssignableFrom(type))
                {
                    errors.Add($"Type '{state.TypeName}' does not implement IMementoObject.");
                    return default;
                }
                var instance = (T)Activator.CreateInstance(type, state, errors)!;

                return instance;
            }
            catch (Exception ex)
            {
                foreach(var prop in state.Properties)
                {
                    Debug.WriteLine($"  State Property: {prop.Key} = {prop.Value}");
                }
                Debug.WriteLine($"Failed to create memento object of type '{state.TypeName}': {ex}");
                //throw;
            }
            return default;
        }
    }
}
