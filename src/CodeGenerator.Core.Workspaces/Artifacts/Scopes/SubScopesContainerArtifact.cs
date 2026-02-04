using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts.Scopes
{
    public class SubScopesContainerArtifact : WorkspaceArtifactBase, IEnumerable<ScopeArtifact>
    {
        public SubScopesContainerArtifact()
        {
            PublishArtifactConstructionEvent();
        }

        public SubScopesContainerArtifact(ArtifactState state)
            : base(state) 
        {
            PublishArtifactConstructionEvent();
        }

        public override string TreeNodeText => "SubScopes";

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("volleyball");

        public void AddScope(string scopeName)
        {
            var scope = AddChild(new ScopeArtifact(scopeName));

            var codeArchitecture = Workspace!.CodeArchitecture!;

            foreach (var layerFactory in codeArchitecture.Layers)
            {
                var layerArtifact = layerFactory.CreateLayer(scope.Name);
                scope.AddChild(layerArtifact);
            }
            scope.AddChild(new SubScopesContainerArtifact());
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
