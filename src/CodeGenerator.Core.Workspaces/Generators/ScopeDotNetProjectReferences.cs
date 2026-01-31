using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Generators
{
    public class ScopeDotNetProjectReferences
    {
        public ScopeArtifact ScopeArtifact { get; set; }
        /// <summary>
        /// Key: Layer-name (string)<br /> eg. "Application", "Domain", "Infrastructure", "Presentation"<br />
        /// Value: DotNetProjectArtifact
        /// </summary>
        public Dictionary<string, DotNetProjectArtifact> DotNetProjectArtifacts { get;  } = new Dictionary<string, DotNetProjectArtifact>();

        public List<ScopeDotNetProjectReferences> SubScopeReferences { get; } = new List<ScopeDotNetProjectReferences>();

        public ScopeDotNetProjectReferences(ScopeArtifact scopeArtifact)
        {
            ScopeArtifact = scopeArtifact;
        }

        public void Reset()
        {
            DotNetProjectArtifacts.Clear();
            foreach(var subscope in SubScopeReferences)
            {
                subscope.Reset();
            }
        }
    }
}
