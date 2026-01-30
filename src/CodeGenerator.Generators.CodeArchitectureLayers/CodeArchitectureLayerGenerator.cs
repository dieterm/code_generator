using CodeGenerator.Core.Artifacts.CodeGeneration;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Workspaces.Generators;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Core.Workspaces.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Shared;
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
            
            
            foreach (var scope in commonScopes)
            {
                var folderNamePattern = new ParameterizedString(settings.FolderNamePattern);
                var parameters = new Dictionary<string, string>
                {
                    { CodeArchitectureLayerGeneratorSettings.FolderNamePattern_WorkspaceNamespaceParameter, args.Result.Workspace.RootNamespace },
                    { CodeArchitectureLayerGeneratorSettings.FolderNamePattern_LayerParameter, LayerName },
                    { CodeArchitectureLayerGeneratorSettings.FolderNamePattern_ScopeParameter, scope },
                    { CodeArchitectureLayerGeneratorSettings.FolderNamePattern_DomainNamespaceParameter, scope }
                };
                var folderName = folderNamePattern.GetOutput(parameters);
                
                var scopeFolderArtifact = new FolderArtifact(folderName);
                AddChildArtifactToParent(args.Result.RootArtifact, scopeFolderArtifact, args.Result);
                
                var layerArtifact = CreateLayerArtifact(scope);
                AddChildArtifactToParent(scopeFolderArtifact, layerArtifact, args.Result);
            }

            // Get domains from workspace
            // var domains = args.Result.Workspace.Domains.GetDomains().ToArray();
            var scopes = args.Result.Workspace.Scopes.ToArray();

            //foreach (var domain in domains)
            foreach (var scope in scopes)
            {
                var folderNamePattern = new ParameterizedString(settings.FolderNamePattern);
                var parameters = new Dictionary<string, string>
                {
                    { CodeArchitectureLayerGeneratorSettings.FolderNamePattern_WorkspaceNamespaceParameter, args.Result.Workspace.RootNamespace },
                    { CodeArchitectureLayerGeneratorSettings.FolderNamePattern_LayerParameter, LayerName },
                    { CodeArchitectureLayerGeneratorSettings.FolderNamePattern_ScopeParameter, scope.Name },
                    { CodeArchitectureLayerGeneratorSettings.FolderNamePattern_DomainNamespaceParameter, scope.Namespace }
                };
                var folderName = folderNamePattern.GetOutput(parameters);
                var scopeFolderArtifact = new FolderArtifact(folderName);
                AddChildArtifactToParent(args.Result.RootArtifact, scopeFolderArtifact, args.Result);
                var layerArtifact = CreateLayerArtifact(scope.Name);
                // attach a reference to the domain artifact which is being used here
                layerArtifact.AddDecorator(new ScopeArtifactRefDecorator(scope));
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
