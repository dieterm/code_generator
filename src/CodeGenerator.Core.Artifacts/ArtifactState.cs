using CodeGenerator.Shared.Memento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts
{
    /// <summary>
    /// This is a JSON-serializable memento class that holds the state of an artifact at a specific point in time.
    /// </summary>
    public sealed class ArtifactState : ArtifactNoChildrenState
    {
        public ArtifactState() : base()
        {
        }

        //[JsonIgnore]
        //public string Id { get { return GetValue<string>(nameof(Id)); } }
        
        //[JsonPropertyName("decorators")]
        //public List<ArtifactDecoratorState> Decorators { get; set; } = new List<ArtifactDecoratorState>();
        
        [JsonPropertyName("children")]
        public List<ArtifactState> Children { get; set; } = new List<ArtifactState>();

        public override object Clone()
        {
            return new ArtifactState()
            {
                TypeName = this.TypeName,
                Properties = new Dictionary<string, object?>(this.Properties),
                Decorators = this.Decorators.Select(d => (ArtifactDecoratorState)d.Clone()).ToList(),
                Children = this.Children.Select(c => (ArtifactState)c.Clone()).ToList()
            };
        }
    }
}
