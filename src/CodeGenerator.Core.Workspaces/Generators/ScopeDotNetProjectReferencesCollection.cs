using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Generators
{
    public class ScopeDotNetProjectReferencesCollection : KeyedCollection<string, ScopeDotNetProjectReferences>
    {
        protected override string GetKeyForItem(ScopeDotNetProjectReferences item)
        {
            return item.ScopeArtifact.Name;
        }
    }
}
