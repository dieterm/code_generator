using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Services
{
    public static class WorkspaceTemplateHelpers
    {
        /// <summary>
        /// Get namespace of a specific layer within a scope.
        /// </summary>
        /// <param name="scopeName">The name of the scope.</param>
        /// <param name="layerName">The name of the layer.</param>
        /// <returns>The namespace of the specified layer within the scope, or null if not found.</returns>
        public static string? GetLayerNamespace(string scopeName, string layerName)
        {
            var scope = GetScopeByName(scopeName);
            var layer = scope?.GetLayers().FirstOrDefault(l => l.LayerName== layerName);
            return (layer as WorkspaceArtifactBase)?.Context?.Namespace;
        }
        /// <summary>
        /// Get scope-artifact by name, searching recursively in sub-scopes.
        /// </summary>
        public static ScopeArtifact? GetScopeByName(string scopeName, IEnumerable<ScopeArtifact>? scopes = null)
        {
            if (scopes == null)
            {
                var workspaceContext = ServiceProviderHolder.GetRequiredService<IWorkspaceContextProvider>();
                var workspace = workspaceContext.CurrentWorkspace!;
                scopes = workspace.Scopes;
            }
            var scope = scopes.SingleOrDefault(s => s.Name == scopeName);
            if(scope!=null)
                return scope;
            
            foreach(var parentScope in scopes)
            {
                var subScope = GetScopeByName(scopeName, parentScope.SubScopes);
                if(subScope!=null)
                    return subScope;
            }

            return null;
        }
        /// <summary>
        /// Get namespace of Shared scope Domain layer. Eg. "MyWorkspace.Shared.Domain"
        /// </summary>
        public static string? GetSharedDomainNamespace()
        {
            return GetLayerNamespace(CodeArchitectureScopes.SHARED_SCOPE, OnionCodeArchitecture.DOMAIN_LAYER);
        }

        /// <summary>
        /// Get namespace of Shared scope Application layer. Eg. "MyWorkspace.Shared.Application"
        /// </summary>
        public static string? GetSharedApplicationNamespace()
        {
            return GetLayerNamespace(CodeArchitectureScopes.SHARED_SCOPE, OnionCodeArchitecture.APPLICATION_LAYER);
        }

        /// <summary>
        /// Get namespace of Shared scope Infrastructure layer. Eg. "MyWorkspace.Shared.Infrastructure"
        /// </summary>
        public static string? GetSharedInfrastructureNamespace()
        {
            return GetLayerNamespace(CodeArchitectureScopes.SHARED_SCOPE, OnionCodeArchitecture.INFRASTRUCTURE_LAYER);
        }

        /// <summary>
        /// Get namespace of Shared scope Presentation layer. Eg. "MyWorkspace.Shared.Presentation"
        /// </summary>
        public static string? GetSharedPresentationNamespace()
        {
            return GetLayerNamespace(CodeArchitectureScopes.SHARED_SCOPE, OnionCodeArchitecture.PRESENTATION_LAYER);
        }

        /// <summary>
        /// Get namespace of Application scope Application layer. Eg. "MyWorkspace.Application.Application"
        /// </summary>
        public static string? GetApplicationApplicationNamespace()
        {
            return GetLayerNamespace(CodeArchitectureScopes.APPLICATION_SCOPE, OnionCodeArchitecture.APPLICATION_LAYER);
        }

        /// <summary>
        /// Get namespace of Application scope Presentation layer. Eg. "MyWorkspace.Application.Presentation"
        /// </summary>
        public static string? GetApplicationPresentationNamespace()
        {
            return GetLayerNamespace(CodeArchitectureScopes.APPLICATION_SCOPE, OnionCodeArchitecture.PRESENTATION_LAYER);
        }
        /// <summary>
        /// Get namespace of Application scope Infrastructure layer. Eg. "MyWorkspace.Application.Infrastructure"
        /// </summary>
        public static string? GetApplicationInfrastructureNamespace()
        {
            return GetLayerNamespace(CodeArchitectureScopes.APPLICATION_SCOPE, OnionCodeArchitecture.INFRASTRUCTURE_LAYER);
        }
        /// <summary>
        /// Get namespace of Application scope Domain layer. Eg. "MyWorkspace.Application.Domain"
        /// </summary>
        public static string? GetApplicationDomainNamespace()
        {
            return GetLayerNamespace(CodeArchitectureScopes.APPLICATION_SCOPE, OnionCodeArchitecture.DOMAIN_LAYER);
        }
    }
}
