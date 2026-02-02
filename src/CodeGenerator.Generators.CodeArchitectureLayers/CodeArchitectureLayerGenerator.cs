using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.CodeGeneration;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
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
    public class CodeArchitectureLayerGenerator : GeneratorBase
    {
        private Action<CreatingArtifactEventArgs>? _unsubscribeHandler;
        public override void SubscribeToEvents(GeneratorMessageBus messageBus)
        {
            _unsubscribeHandler = messageBus.Subscribe<CreatingArtifactEventArgs>(OnCreatingRootArtifact, (e) => e.Artifact is RootArtifact);
        }

        public override void UnsubscribeFromEvents(GeneratorMessageBus messageBus)
        {
            if (_unsubscribeHandler != null)
                messageBus.Unsubscribe<CreatingArtifactEventArgs>(_unsubscribeHandler);
        }

        private void OnCreatingRootArtifact(CreatingArtifactEventArgs args)
        {
            if (!Enabled)
                return;

            foreach (var scope in args.Result.Workspace.Scopes)
            {
                // recursively create folders for each scope & sub-scopes
                CreateScopeFolder(scope, args, args.Artifact); 
            }
        }

        private void CreateScopeFolder(ScopeArtifact scope, CreatingArtifactEventArgs args, IArtifact parentArtifact)
        {
            var folderName = scope.Name;
            var scopeFolderArtifact = new FolderArtifact(folderName);
            scopeFolderArtifact.AddDecorator(new ScopeArtifactRefDecorator(scope));
            AddChildArtifactToParent(parentArtifact, scopeFolderArtifact, args.Result);
            foreach(var subScope in scope.SubScopes)
            {
                CreateScopeFolder(subScope, args, scopeFolderArtifact);
            }
        }

        protected override GeneratorSettingsDescription ConfigureSettingsDescription()
        {
            var id = nameof(CodeArchitectureLayerGenerator);
            var name = "Code Architecture Layer Generator";
            var description = "Generates code architecture layers for each scope in the workspace.";
            return new GeneratorSettingsDescription(id, name, description);
        }
    }
}
