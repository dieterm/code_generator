using CodeGenerator.Core.Artifacts.CodeGeneration;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace CodeGenerator.Generators.CodeArchitectureLayers
{
    public abstract class CodeArchitectureLayerGenerator : IMessageBusAwareGenerator
    {
        private GeneratorMessageBus? _messageBus;
        private Action<CreatingArtifactEventArgs>? _unsubscribeHandler;

        protected abstract CodeArchitectureLayerArtifact CreateLayerArtifact(string scope);
        protected abstract string LayerName { get; }

        public abstract GeneratorSettingsDescription SettingsDescription { get; }

        public void Initialize(GeneratorMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        private void OnCreatingRootArtifact(CreatingArtifactEventArgs args)
        {
            var commonScopes = new List<string>
            {
                CodeArchitectureLayerArtifact.SHARED_SCOPE,
                CodeArchitectureLayerArtifact.APPLICATION_SCOPE
            };
            // TODO: Get domain-specific scopes from generator context
            var domainName = args.Result?.DomainSchema?.DomainDrivenDesignMetadata?.BoundedContext;
            var domainScopes = domainName!=null ? new string[] { domainName } : new string[] { "ProjectA", "ProjectB" };
            var ns = args.Result?.DomainSchema?.CodeGenMetadata?.Namespace;
            var allScopes = commonScopes.Concat(domainScopes).ToArray();
            foreach (var scope in allScopes)
            {
                var folderName = $"{ns}.{scope}.{LayerName}";
                var scopeFolderArtifact = new FolderArtifact(folderName);
                _messageBus?.Publish(new CreatingArtifactEventArgs(args.Result, scopeFolderArtifact));
                args.Result.RootArtifact.AddChild(scopeFolderArtifact);
                _messageBus?.Publish(new CreatedArtifactEventArgs(args.Result, scopeFolderArtifact));
                var layerArtifact = CreateLayerArtifact(scope);
                _messageBus?.Publish(new CreatingArtifactEventArgs(args.Result, layerArtifact));
                scopeFolderArtifact.AddChild(layerArtifact);
                _messageBus?.Publish(new CreatedArtifactEventArgs(args.Result, layerArtifact));
            }
        }

        public void SubscribeToEvents(GeneratorMessageBus messageBus)
        {
            _unsubscribeHandler = messageBus.Subscribe<CreatingArtifactEventArgs>(OnCreatingRootArtifact, (e) => e.Artifact is RootArtifact);
        }

        public void UnsubscribeFromEvents(GeneratorMessageBus messageBus)
        {
            if(_unsubscribeHandler!=null)
                messageBus.Unsubscribe<CreatingArtifactEventArgs>(_unsubscribeHandler);
        }
    }
}
