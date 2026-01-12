using CodeGenerator.Shared.Memento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts
{
    /// <summary>
    /// This is a JSON-serializable memento class that holds the state of an artifact at a specific point in time.
    /// </summary>
    public sealed class ArtifactState : MementoState<IArtifact>
    {
        public string Id { get { return GetValue<string>(nameof(Id)); } }

        public List<ArtifactDecoratorState> Decorators { get; } = new List<ArtifactDecoratorState>();
        public List<string> ChildrenIds { get; } = new List<string>();
    }
}
