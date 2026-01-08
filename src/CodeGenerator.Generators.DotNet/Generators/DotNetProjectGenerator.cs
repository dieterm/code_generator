using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Templates;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Generators.CodeArchitectureLayers.ApplicationLayer;
using CodeGenerator.Generators.CodeArchitectureLayers.DomainLayer;
using CodeGenerator.Generators.CodeArchitectureLayers.InfrastructureLayer;
using CodeGenerator.Generators.CodeArchitectureLayers.PresentationLayer;
using CodeGenerator.Shared.Models;
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
            SettingsDescription = ConfigureSettingsDescription();
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
            var settingsDescription = new GeneratorSettingsDescription(id, name, description);
            // ProjectType
            settingsDescription.ParameterDefinitions.Add(new ParameterDefinition
            {
                Name = "ProjectType",
                PossibleValues = DotNetProjectType.AllTypes.Select(t => (object)new { DisplayName = t, Id=t }).ToList(),
                Description = "The type of .NET project to generate (e.g., Class Library, Console App, Web API).",
                Type = ParameterDefinitionTypes.String,
                DefaultValue = Layer == CodeArchitectureLayerArtifact.PRESENTATION_LAYER ? DotNetProjectType.WinFormsLib : DotNetProjectType.ClassLib,
                Required = true
            });
            // TargetFramework
            settingsDescription.ParameterDefinitions.Add(new ParameterDefinition
            {
                Name = "TargetFramework",
                PossibleValues = TargetFrameworks.AllFrameworks.Select(t => (object)new { DisplayName = t, Id=t }).ToList(),
                Description = "The target framework for the .NET project (e.g., net8.0, net7.0).",
                Type = ParameterDefinitionTypes.String,
                DefaultValue = TargetFrameworks.Net8,
                Required = true
            });
            // DotNet Language
            settingsDescription.ParameterDefinitions.Add(new ParameterDefinition
            {
                Name = "Language",
                PossibleValues = DotNetLanguages.AllLanguages.Select(language => (object)new { DisplayName = $"{language.DotNetCommandLineArgument} (*.{language.ProjectFileExtension})", Id=language.DotNetCommandLineArgument }).ToList(),
                Description = "The programming language for the .NET project (e.g., C#, VB.NET).",
                Type = ParameterDefinitionTypes.String,
                DefaultValue = DotNetLanguages.CSharp.DotNetCommandLineArgument,
                Required = true
            });
            // Project Name Pattern
            settingsDescription.ParameterDefinitions.Add(new ParameterDefinition
            {
                Name = "ProjectNamePattern",
                Description = "Pattern for naming the .NET project files.",
                Type = ParameterDefinitionTypes.ParameterisedString,
                DefaultValue = "{{ Layer }}.{{ Scope }}.{{ Language }}",
                PossibleValues = new List<object>
                {
                    new ParameterizedStringParameter{ Description = "Use {{ Layer }} for the layer name.", Parameter="Layer", ExampleValue="Application" },
                    new ParameterizedStringParameter{ Description = "Use {{ Scope }} for the scope name.", Parameter="Scope", ExampleValue="Shared" },
                    new ParameterizedStringParameter{ Description = "Use {{ Language }} for the programming language.", Parameter="Language", ExampleValue="C#" },
                    new ParameterizedStringParameter{ Description = "Use {{ RootNamespace }} for the root namespace.", Parameter="RootNamespace", ExampleValue="MyCompany.MyProduct" }
                },
                Required = true
            });

            settingsDescription.Templates.Add(new TemplateRequirement
            {
                TemplateId = "DotNetProject",
                Description = "Template for generating .NET project files.",
                TemplateType = TemplateType.Scriban,
                Enabled = true,
                OutputFileNamePattern = "{{ ProjectName }}.csproj",
                TemplateFilePath = "Templates/DotNet/Project.scriban"
            });
            if (Layer == CodeArchitectureLayerArtifact.APPLICATION_LAYER)
            {
                settingsDescription.DependingGeneratorIds.Add(nameof(ApplicationLayerGenerator));
            }
            else if (Layer == CodeArchitectureLayerArtifact.DOMAIN_LAYER)
            {
                settingsDescription.DependingGeneratorIds.Add(nameof(DomainLayerGenerator));
            }
            else if (Layer == CodeArchitectureLayerArtifact.INFRASTRUCTURE_LAYER)
            {
                settingsDescription.DependingGeneratorIds.Add(nameof(InfrastructureLayerGenerator));
            }
            else if (Layer == CodeArchitectureLayerArtifact.PRESENTATION_LAYER)
            {
                settingsDescription.DependingGeneratorIds.Add(nameof(PresentationLayerGenerator));
            }
                
            return settingsDescription;
        }
    }
}
