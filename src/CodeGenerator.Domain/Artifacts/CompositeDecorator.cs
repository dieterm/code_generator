using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.Artifacts
{
    public class CompositeDecorator : IArtifactDecorator
    {
        private readonly List<IArtifactDecorator> _childDecorators = new List<IArtifactDecorator>();

        protected void AddChildDecorator(IArtifactDecorator decorator)
        {
            _childDecorators.Add(decorator);
        }

        public IEnumerable<IArtifactDecorator> ChildDecorators => _childDecorators;

        public Artifact? Artifact => _artifact;

        public int Priority { get; set; }
        public string RegisteredBy { get; set; }

        private Artifact? _artifact;
        public void Attach(Artifact artifact)
        {
            _artifact = artifact;
            foreach (var childDecorator in _childDecorators)
            {
                childDecorator.Attach(artifact);
            }
        }

        public void Detach()
        {
            foreach(var childDecorator in _childDecorators)
            {
                childDecorator.Detach();
            }
            _artifact = null;
        }
    }
}
