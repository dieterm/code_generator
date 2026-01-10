using CodeGenerator.Core.Artifacts.CodeGeneration;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Workspaces.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace CodeGenerator.Generators.CodeArchitectureLayers
{
    public abstract class CodeArchitectureLayerGenerator : GeneratorBase
    {
        //private GeneratorMessageBus? _messageBus;
        private Action<CreatingArtifactEventArgs>? _unsubscribeHandler;

        protected abstract CodeArchitectureLayerArtifact CreateLayerArtifact(string scope);
        protected abstract string LayerName { get; }

        private void OnCreatingRootArtifact(CreatingArtifactEventArgs args)
        {
            var settings = new CodeArchitectureLayerGeneratorSettings(base.GetSettings());
           
            var commonScopes = new List<string>
            {
                CodeArchitectureLayerArtifact.SHARED_SCOPE,
                CodeArchitectureLayerArtifact.APPLICATION_SCOPE
            };
            // TODO: Get domain-specific scopes from workspace context
            var domainName = args.Result?.DomainSchema?.DomainDrivenDesignMetadata?.BoundedContext;
            var domainScopes = domainName!=null ? new string[] { domainName } : new string[] { "ProjectA", "ProjectB" };
            var domainNamespace = args.Result?.DomainSchema?.CodeGenMetadata?.Namespace;

            var allScopes = commonScopes.Concat(domainScopes).ToArray();
            foreach (var scope in allScopes)
            {
                var folderNamePattern = new ParameterizedString(settings.FolderNamePattern);
                var parameters = new Dictionary<string, string>
                {
                    { CodeArchitectureLayerGeneratorSettings.FolderNamePattern_WorkspaceNamespaceParameter, domainNamespace ?? WorkspaceSettings.Instance.RootNamespace },
                    { CodeArchitectureLayerGeneratorSettings.FolderNamePattern_LayerParameter, LayerName },
                    { CodeArchitectureLayerGeneratorSettings.FolderNamePattern_ScopeParameter, scope }
                };
                var folderName = folderNamePattern.GetOutput(parameters);
                
                var scopeFolderArtifact = new FolderArtifact(folderName);
                AddChildArtifactToParent(args.Result.RootArtifact, scopeFolderArtifact, args.Result);
                
                var layerArtifact = CreateLayerArtifact(scope);
                AddChildArtifactToParent(scopeFolderArtifact, layerArtifact, args.Result);
            }
        }

        public override void SubscribeToEvents(GeneratorMessageBus messageBus)
        {
            _unsubscribeHandler = messageBus.Subscribe<CreatingArtifactEventArgs>(OnCreatingRootArtifact, (e) => e.Artifact is RootArtifact);
        }

        public override void UnsubscribeFromEvents(GeneratorMessageBus messageBus)
        {
            if(_unsubscribeHandler!=null)
                messageBus.Unsubscribe<CreatingArtifactEventArgs>(_unsubscribeHandler);
        }
    }
}
