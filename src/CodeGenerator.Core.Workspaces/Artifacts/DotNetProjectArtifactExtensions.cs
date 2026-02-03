using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Domain.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DotNet
{
    public static class DotNetProjectArtifactExtensions
    {
            public static string GetLayerNamespace(this DotNetProjectArtifact projectArtifact, params string[] subnamespaces)
            {
                if (projectArtifact == null) throw new ArgumentNullException(nameof(projectArtifact));

                var projectFolderArtifact = projectArtifact.Parent as FolderArtifact;
                var layerArtifact = projectFolderArtifact?.GetDecoratorOfType<LayerArtifactRefDecorator>()?.LayerArtifact;
                var layerNamespace = (layerArtifact as WorkspaceArtifactBase).Context?.Namespace;

                return string.Join(".", new[] { layerNamespace }.Concat(subnamespaces));
        }
    }
}
