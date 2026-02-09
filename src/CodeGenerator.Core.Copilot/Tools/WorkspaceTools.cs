using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.AI;
using System.ComponentModel;

namespace CodeGenerator.Core.Copilot.Tools
{
    /// <summary>
    /// Defines AI-callable tool functions for workspace queries (read-only).
    /// Mutation operations are handled via IOperation implementations
    /// and registered through CopilotOperationBridge.
    /// </summary>
    public class WorkspaceTools
    {
        private readonly IWorkspaceContextProvider _workspaceContextProvider;
        private readonly Func<Func<string>, string> _uiInvoker;

        /// <param name="workspaceContextProvider">Provides access to the current workspace</param>
        /// <param name="uiInvoker">
        /// Delegate that executes a function on the UI thread and returns its result.
        /// Required because workspace reads may need UI-thread access.
        /// </param>
        public WorkspaceTools(IWorkspaceContextProvider workspaceContextProvider, Func<Func<string>, string> uiInvoker)
        {
            _workspaceContextProvider = workspaceContextProvider;
            _uiInvoker = uiInvoker;
        }

        private WorkspaceArtifact GetWorkspace()
        {
            return _workspaceContextProvider.CurrentWorkspace
                ?? throw new InvalidOperationException("No workspace is currently open.");
        }

        /// <summary>
        /// Returns all AIFunction tool definitions for read-only workspace queries.
        /// </summary>
        public List<AIFunction> GetAllTools()
        {
            return
            [
                AIFunctionFactory.Create(GetWorkspaceInfo, nameof(GetWorkspaceInfo), "Get general information about the current workspace"),
                AIFunctionFactory.Create(ListScopes, nameof(ListScopes), "List all scopes in the current workspace"),
                AIFunctionFactory.Create(ListDomains, nameof(ListDomains), "List all domains within a scope"),
                AIFunctionFactory.Create(ListEntities, nameof(ListEntities), "List all entities within a domain in a scope"),
            ];
        }

        public string GetWorkspaceInfo()
        {
            return _uiInvoker(() =>
            {
                var workspace = GetWorkspace();
                var scopes = workspace.Scopes.ToList();
                var totalDomains = scopes.Sum(s => s.Domains?.Count() ?? 0);

                return $"Workspace: {workspace.Name}, RootNamespace: {workspace.RootNamespace}, " +
                       $"Scopes: {scopes.Count}, Total Domains: {totalDomains}, " +
                       $"Framework: {workspace.DefaultTargetFramework}, Language: {workspace.DefaultLanguage}";
            });
        }

        public string ListScopes()
        {
            return _uiInvoker(() =>
            {
                var workspace = GetWorkspace();
                var scopes = workspace.Scopes.Select(s => s.Name).ToList();
                return scopes.Count == 0
                    ? "No scopes found in the workspace."
                    : $"Scopes: {string.Join(", ", scopes)}";
            });
        }

        public string ListDomains([Description("The name of the scope")] string scopeName)
        {
            return _uiInvoker(() =>
            {
                var scope = GetWorkspace().FindScope(scopeName);
                var domains = scope.Domains.Select(d => d.Name).ToList();
                return domains.Count == 0
                    ? $"No domains found in scope '{scopeName}'."
                    : $"Domains in '{scopeName}': {string.Join(", ", domains)}";
            });
        }

        public string ListEntities(
            [Description("The name of the scope")] string scopeName,
            [Description("The name of the domain")] string domainName)
        {
            return _uiInvoker(() =>
            {
                var domain = GetWorkspace().FindDomain(scopeName, domainName);
                var entities = domain.Entities.GetEntities().Select(e => e.Name).ToList();
                return entities.Count == 0
                    ? $"No entities found in domain '{domainName}'."
                    : $"Entities in '{domainName}': {string.Join(", ", entities)}";
            });
        }

    }
}
