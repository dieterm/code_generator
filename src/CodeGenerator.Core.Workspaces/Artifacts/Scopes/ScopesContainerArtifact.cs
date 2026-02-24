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

        public ScopesContainerArtifact(ArtifactState state, List<string> errors)
            : base(state, errors) 
        {
            PublishArtifactConstructionEvent();
        }

        public override string TreeNodeText => "Scopes";

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("volleyball");

        public ScopeArtifact? FindScopeByFullName(string fullName, bool exceptionIfNotFound = true)
        {
            var parts = fullName.Split(new[] { ScopeArtifact.SCOPE_FULL_NAME_SEPERATOR }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                if (exceptionIfNotFound)
                {
                    throw new InvalidOperationException($"Invalid scope full name '{fullName}'.");
                }
                return null;
            }
            ScopeArtifact? currentScope = null;
            foreach (var part in parts)
            {
                if (currentScope == null)
                {
                    currentScope = this.FirstOrDefault(s => s.Name.Equals(part, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    currentScope = currentScope.SubScopes.FirstOrDefault(s => s.Name.Equals(part, StringComparison.OrdinalIgnoreCase));
                }
                if (currentScope == null)
                {
                    if (exceptionIfNotFound)
                    {
                        throw new InvalidOperationException($"Scope with full name '{fullName}' not found. Failed at part '{part}'.");
                    }
                    return null;
                }
            }
            return currentScope;
        }

        public ScopeArtifact? FindScope(string scopeName, bool exceptionIfNotFound = true)
        {
            if (scopeName.Contains(ScopeArtifact.SCOPE_FULL_NAME_SEPERATOR))
            {
                // Handle full name search
                return FindScopeByFullName(scopeName, exceptionIfNotFound);
            }

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
