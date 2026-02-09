using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Services;
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
            PublishArtifactConstructionEvent();
        }

        public ScopesContainerArtifact(ArtifactState state)
            : base(state) 
        {
            PublishArtifactConstructionEvent();
        }

        public override string TreeNodeText => "Scopes";

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("volleyball");

        public ScopeArtifact? FindScope(string scopeName, bool exceptionIfNotFound = true)
        {
            foreach (var scope in this)
            {
                if (scope.Name.Equals(scopeName, StringComparison.OrdinalIgnoreCase))
                    return scope;
                var found = scope.FindScopeRecursive(scopeName);
                if (found != null)
                    return found;
            }

            if (exceptionIfNotFound)
            {
                throw new InvalidOperationException(
                    $"Scope '{scopeName}' not found. Available scopes: {string.Join(", ", this.Select(s => s.Name))}");
            }

            return null;
        }

        //public ScopeArtifact? FindScopeOrDefault(string scopeName)
        //{
            
        //    foreach (var scope in this)
        //    {
        //        if (scope.Name.Equals(scopeName, StringComparison.OrdinalIgnoreCase))
        //            return scope;
        //        var found = scope.FindScopeRecursive(scopeName);
        //        if (found != null)
        //            return found;
        //    }

        //    return null;
        //}
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
