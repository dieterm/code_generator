using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts.Scopes
{
    public class ScopesContainerArtifact : WorkspaceArtifactBase, IEnumerable<ScopeArtifact>
    {
        public ScopesContainerArtifact()
        {
            AddChild(new ScopeArtifact(ScopeArtifact.DEFAULT_SCOPE_SHARED));
            AddChild(new ScopeArtifact(ScopeArtifact.DEFAULT_SCOPE_APPLICATION));

            PublishArtifactConstructionEvent();
        }

        public ScopesContainerArtifact(ArtifactState state)
            : base(state) 
        {
            EnsureChildArtifactExists<ScopeArtifact>(() => new ScopeArtifact(ScopeArtifact.DEFAULT_SCOPE_SHARED), (s) => s.Name == ScopeArtifact.DEFAULT_SCOPE_SHARED);
            EnsureChildArtifactExists<ScopeArtifact>(() => new ScopeArtifact(ScopeArtifact.DEFAULT_SCOPE_APPLICATION), (s) => s.Name == ScopeArtifact.DEFAULT_SCOPE_APPLICATION);
            PublishArtifactConstructionEvent();
        }

        public override string TreeNodeText => "Scopes";

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("volleyball");

        public void AddScope(string scopeName)
        {
            AddChild(new ScopeArtifact(scopeName));
        }

        #region Implement IEnumerable<ScopeArtifact>
        public IEnumerator<ScopeArtifact> GetEnumerator()
        {
            return Children.OfType<ScopeArtifact>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
