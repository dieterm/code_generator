using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.ValueTypes;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Shared;
using Microsoft.Extensions.AI;
using System.ComponentModel;

namespace CodeGenerator.Core.Copilot.Tools
{
    /// <summary>
    /// Defines AI-callable tool functions for workspace manipulation.
    /// These functions are registered with the Copilot SDK so the LLM can call them
    /// to perform actions on the workspace (add domains, entities, properties, etc.)
    /// All tool functions are marshalled to the UI thread via the provided invoker,
    /// because workspace changes trigger TreeView UI updates.
    /// </summary>
    public class WorkspaceTools
    {
        private readonly IWorkspaceContextProvider _workspaceContextProvider;
        private readonly Func<Func<string>, string> _uiInvoker;

        /// <param name="workspaceContextProvider">Provides access to the current workspace</param>
        /// <param name="uiInvoker">
        /// Delegate that executes a function on the UI thread and returns its result.
        /// Required because workspace mutations trigger TreeView updates.
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
        /// Returns all AIFunction tool definitions for the Copilot session.
        /// </summary>
        public List<AIFunction> GetAllTools()
        {
            return
            [
                AIFunctionFactory.Create(ListScopes, nameof(ListScopes), "List all scopes in the current workspace"),
                AIFunctionFactory.Create(ListDomains, nameof(ListDomains), "List all domains within a scope"),
                AIFunctionFactory.Create(ListEntities, nameof(ListEntities), "List all entities within a domain in a scope"),
                AIFunctionFactory.Create(AddScope, nameof(AddScope), "Add a new scope to the workspace"),
                AIFunctionFactory.Create(AddSubScope, nameof(AddSubScope), "Add a new sub-scope to an existing scope"),
                AIFunctionFactory.Create(AddDomain, nameof(AddDomain), "Add a new domain to a scope"),
                AIFunctionFactory.Create(AddEntity, nameof(AddEntity), "Add a new entity to a domain"),
                AIFunctionFactory.Create(AddEntityWithProperties, nameof(AddEntityWithProperties), "Add a new entity with properties to a domain"),
                AIFunctionFactory.Create(AddPropertyToEntity, nameof(AddPropertyToEntity), "Add a property to an existing entity's default state"),
                AIFunctionFactory.Create(AddValueType, nameof(AddValueType), "Add a value type to a domain"),
                AIFunctionFactory.Create(GetWorkspaceInfo, nameof(GetWorkspaceInfo), "Get general information about the current workspace"),
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
                var scope = FindScope(scopeName);
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
                var domain = FindDomain(scopeName, domainName);
                var entities = domain.Entities.GetEntities().Select(e => e.Name).ToList();
                return entities.Count == 0
                    ? $"No entities found in domain '{domainName}'."
                    : $"Entities in '{domainName}': {string.Join(", ", entities)}";
            });
        }

        public string AddDomain(
            [Description("The name of the scope (e.g. 'Shared' or 'Application')")] string scopeName,
            [Description("The name for the new domain")] string domainName)
        {
            return _uiInvoker(() =>
            {
                var scope = FindScope(scopeName);
                var existing = scope.Domains.FirstOrDefault(d => d.Name.Equals(domainName, StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                    return $"Domain '{domainName}' already exists in scope '{scopeName}'.";

                scope.Domains.AddDomain(domainName);
                return $"Domain '{domainName}' added to scope '{scopeName}'.";
            });
        }

        public string AddScope(
            [Description("The name of the new scope")] string scopeName)
        {
            return _uiInvoker(() =>
            {
                var workspace = GetWorkspace();
                var existing = workspace.Scopes.FirstOrDefault(s => s.Name.Equals(scopeName, StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                    return $"Scope '{scopeName}' already exists.";

                if(workspace.CodeArchitecture==null)
                    return $"Workspace does not have a code architecture defined. Cannot add scope.";

                var scope = workspace.CodeArchitecture.ScopeFactory.CreateScopeArtifact(scopeName);
                workspace.Scopes.AddChild(scope);
                
                return $"Scope '{scopeName}' added.";
            });
        }

        public string AddSubScope(
            [Description("The name of the parent scope")] string parentScopeName,
            [Description("The name of the new sub-scope")] string newScopeName
            )
        {
            return _uiInvoker(() =>
            {
                var workspace = GetWorkspace();
                var parentScope = FindScope(parentScopeName);
                var existing = FindScope(newScopeName, true);
                if (existing != null)
                    return $"Scope '{newScopeName}' already exists.";

                parentScope.SubScopes.AddScope(newScopeName);

                return $"Sub-scope '{newScopeName}' added to scope '{parentScopeName}'.";
            });
        }

        public string AddEntity(
            [Description("The name of the scope")] string scopeName,
            [Description("The name of the domain")] string domainName,
            [Description("The name for the new entity")] string entityName)
        {
            return _uiInvoker(() =>
            {
                var domain = FindDomain(scopeName, domainName);
                var existing = domain.Entities.GetEntities()
                    .FirstOrDefault(e => e.Name.Equals(entityName, StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                    return $"Entity '{entityName}' already exists in domain '{domainName}'.";

                var entity = new EntityArtifact(entityName);
                domain.AddEntity(entity);
                return $"Entity '{entityName}' added to domain '{domainName}'.";
            });
        }

        public string AddEntityWithProperties(
            [Description("The name of the scope")] string scopeName,
            [Description("The name of the domain")] string domainName,
            [Description("The name for the new entity")] string entityName,
            [Description("Comma-separated list of property definitions in format 'Name:DataType:IsNullable' (e.g. 'Name:varchar:false,Population:int:false,Area:decimal:true'). Supported data types: varchar, int, bigint, decimal, float, bool, datetime, guid, text.")] string propertiesDefinition)
        {
            return _uiInvoker(() =>
            {
                var domain = FindDomain(scopeName, domainName);
                var existing = domain.Entities.GetEntities()
                    .FirstOrDefault(e => e.Name.Equals(entityName, StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                    return $"Entity '{entityName}' already exists in domain '{domainName}'.";

                var entity = new EntityArtifact(entityName);
                var state = entity.AddEntityState(entityName);
                entity.DefaultStateId = state.Id;

                var propertyDefs = propertiesDefinition.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var propDef in propertyDefs)
                {
                    var parts = propDef.Split(':', StringSplitOptions.TrimEntries);
                    var propName = parts[0];
                    var dataType = parts.Length > 1 ? parts[1] : "varchar";
                    var isNullable = parts.Length > 2 && bool.TryParse(parts[2], out var n) && n;

                    state.AddProperty(new PropertyArtifact(propName, dataType, isNullable));
                }

                domain.AddEntity(entity);
                return $"Entity '{entityName}' with {propertyDefs.Length} properties added to domain '{domainName}'.";
            });
        }

        public string AddPropertyToEntity(
            [Description("The name of the scope")] string scopeName,
            [Description("The name of the domain")] string domainName,
            [Description("The name of the entity")] string entityName,
            [Description("The property name")] string propertyName,
            [Description("The data type (e.g. varchar, int, decimal, bool, datetime, guid, text)")] string dataType,
            [Description("Whether the property is nullable")] bool isNullable)
        {
            return _uiInvoker(() =>
            {
                var domain = FindDomain(scopeName, domainName);
                var entity = domain.Entities.GetEntities()
                    .FirstOrDefault(e => e.Name.Equals(entityName, StringComparison.OrdinalIgnoreCase))
                    ?? throw new InvalidOperationException($"Entity '{entityName}' not found in domain '{domainName}'.");

                var state = entity.DefaultState ?? entity.AddEntityState(entityName);
                if (entity.DefaultStateId == null)
                    entity.DefaultStateId = state.Id;

                state.AddProperty(new PropertyArtifact(propertyName, dataType, isNullable));
                return $"Property '{propertyName}' ({dataType}, nullable={isNullable}) added to entity '{entityName}'.";
            });
        }

        public string AddValueType(
            [Description("The name of the scope")] string scopeName,
            [Description("The name of the domain")] string domainName,
            [Description("The name for the new value type")] string valueTypeName)
        {
            return _uiInvoker(() =>
            {
                var domain = FindDomain(scopeName, domainName);
                var existing = domain.ValueTypes.GetValueTypes()
                    .FirstOrDefault(v => v.Name.Equals(valueTypeName, StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                    return $"Value type '{valueTypeName}' already exists in domain '{domainName}'.";

                domain.AddValueType(new ValueTypeArtifact(valueTypeName));
                return $"Value type '{valueTypeName}' added to domain '{domainName}'.";
            });
        }

        #region Helpers

        private ScopeArtifact FindScope(string scopeName, bool returnNullIfNotFound = false)
        {
            var workspace = GetWorkspace();
            foreach(var scope in workspace.Scopes)
            {
                if (scope.Name.Equals(scopeName, StringComparison.OrdinalIgnoreCase))
                    return scope;
                var found = FindScope(scopeName, scope);
                if (found != null)
                    return found;
            }
            if (returnNullIfNotFound) return null;
            throw new InvalidOperationException($"Scope '{scopeName}' not found. Available scopes: {string.Join(", ", workspace.Scopes.Select(s => s.Name))}");
        }

        private ScopeArtifact FindScope(string scopeName, ScopeArtifact parentScope)
        {
            var workspace = GetWorkspace();
            foreach(var subScope in parentScope.SubScopes)
            {
                if (subScope.Name.Equals(scopeName, StringComparison.OrdinalIgnoreCase))
                    return subScope;
                var found = FindScope(scopeName, subScope);
                if (found != null)
                    return found;
            }
            return null;
        }

        private DomainArtifact FindDomain(string scopeName, string domainName)
        {
            var scope = FindScope(scopeName);
            return scope.Domains.FirstOrDefault(d => d.Name.Equals(domainName, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException($"Domain '{domainName}' not found in scope '{scopeName}'. Available domains: {string.Join(", ", scope.Domains.Select(d => d.Name))}");
        }

        #endregion
    }
}
