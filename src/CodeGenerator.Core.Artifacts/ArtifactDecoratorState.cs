using CodeGenerator.Shared.Memento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts
{
    public sealed class ArtifactDecoratorState : MementoState<IArtifactDecorator>
    {
        public string Key { get {  return GetValue<string>(nameof(Key)); } } 
    }
}
