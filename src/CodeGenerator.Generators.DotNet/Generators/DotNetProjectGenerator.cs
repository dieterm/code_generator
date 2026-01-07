using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Generators
{
    public abstract class DotNetProjectGenerator<TLayer> : GeneratorBase where TLayer : CodeArchitectureLayerArtifact
    {
        private Action<CreatedArtifactEventArgs>? _unsubscribe_handler;

        public string Layer { get; }
        public string Scope { get; }   
        protected DotNetProjectGenerator(string layer, string scope)
        {
            Layer = layer;
            Scope = scope;
        }
        public virtual bool LayerArtifactFilter(CreatedArtifactEventArgs e)
        {
            return e.Artifact is TLayer a && a.Scope == Scope;
        }

        public override void SubscribeToEvents(GeneratorMessageBus messageBus)
        {
            _unsubscribe_handler = messageBus.Subscribe<CreatedArtifactEventArgs>((e) => OnLayerScopeCreated(e), LayerArtifactFilter);
        }

        protected virtual DotNetProjectArtifact OnLayerScopeCreated(CreatedArtifactEventArgs args)
        {
            var appLayerArtifact = args.Artifact as TLayer;
            if (appLayerArtifact == null) throw new ArgumentException("Artifact is not an ApplicationLayerArtifact");

            var projectName = $"{appLayerArtifact.Layer}.{appLayerArtifact.Scope}";
            // get from settings later
            var language = DotNetLanguages.CSharp;
            var targetFramework = TargetFrameworks.Net8;
            var projectType = DotNetProjectType.ClassLib;
            var dotNetProjectArtifact = new DotNetProjectArtifact(projectName, language, projectType, targetFramework);
            
            AddChildArtifactToParent(appLayerArtifact, dotNetProjectArtifact, args.Result);
            //MessageBus.Publish(new CreatingArtifactEventArgs(args.Result, dotNetProjectArtifact));
            //appLayerArtifact.AddChild(dotNetProjectArtifact);
            //MessageBus.Publish(new CreatedArtifactEventArgs(args.Result, dotNetProjectArtifact));
            // eg. "<namespace>.<layer>.<domain>.csproj"
            var fileArtifact = new FileArtifact(dotNetProjectArtifact.ProjectFileName);
            AddChildArtifactToParent(dotNetProjectArtifact, fileArtifact, args.Result);

            return dotNetProjectArtifact;
        }

        public override void UnsubscribeFromEvents(GeneratorMessageBus messageBus)
        {
            if (_unsubscribe_handler != null)
                messageBus.Unsubscribe<CreatedArtifactEventArgs>(_unsubscribe_handler);
        }

        protected override GeneratorSettingsDescription ConfigureSettingsDescription()
        {
            var id = $"{Layer}Layer.{Scope}Scope.DotNetProject";
            var name = $".NET {Layer} Layer {Scope} Scope Project Generator";
            var description = $"Generates .NET projects for the {Layer} Layer within the {Scope} Scope.";
            var templateRequirements = new List<TemplateRequirement>
            {
                // Define any template requirements specific to this generator
            };
            return new GeneratorSettingsDescription(id, name, description, templateRequirements);
        }
    }
}
