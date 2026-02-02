using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Domain.CodeArchitecture;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    public class WorkspaceArtifactContext
    {
        public string? Namespace { get; init; }
        public string? ClassName { get; init; }

        public string? OutputPath { get; init; }

        public ScopeArtifact? Scope { get; init; }

        public CodeArchitectureLayerArtifact? Layer { get; init; }

        public ReadOnlyDictionary<string, string> NamespaceParameters { get; init; } = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
    }
}
