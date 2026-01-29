using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Settings.Generators;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Generators.CodeArchitectureLayers.ApplicationLayer;
using CodeGenerator.Generators.CodeArchitectureLayers.DomainLayer;
using CodeGenerator.Generators.CodeArchitectureLayers.InfrastructureLayer;
using CodeGenerator.Generators.CodeArchitectureLayers.PresentationLayer;
using CodeGenerator.Generators.DotNet.Generators;
using CodeGenerator.Shared.Models;
using CodeGenerator.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace CodeGenerator.Generators.DotNet
{
    public class DotNetProjectGeneratorSettings<TLayer> where TLayer : CodeArchitectureLayerArtifact
    {
        public const string ProjectNamePattern_LayerParameter = "Layer";
        public const string ProjectNamePattern_ScopeParameter = "Scope";
        public const string ProjectNamePattern_LanguageParameter = WorkspaceArtifact.ProjectNamePattern_LanguageParameter;
        public const string ProjectNamePattern_WorkspaceNamespaceParameter = WorkspaceArtifact.ProjectNamePattern_WorkspaceNamespaceParameter;
        // "{WorkspaceNamespace}.{Layer}.{Scope}"
        public const string ProjectNamePattern_DefaultValue = $"{{{ProjectNamePattern_WorkspaceNamespaceParameter}}}.{{{ProjectNamePattern_LayerParameter}}}.{{{ProjectNamePattern_ScopeParameter}}}";
        private readonly GeneratorSettings _settings;
        private readonly string _layer;

        public DotNetProjectGeneratorSettings(GeneratorSettings settings, string layer)
        {
            _settings = settings;
            _layer = layer;
        }

        public string? TargetFramework
        {
            get => _settings.GetParameter<string>(nameof(TargetFramework)) ?? TargetFrameworks.Net8;
            set => _settings.SetParameter<string>(nameof(TargetFramework), value);
        }

        public string? ProjectType
        {
            get => _settings.GetParameter<string>(nameof(ProjectType)) ?? GetProjectTypeDefaultValue(_layer);
            set => _settings.SetParameter<string>(nameof(ProjectType), value);
        }

        public string? Language
        {
            get => _settings.GetParameter<string>(nameof(Language)) ?? DotNetLanguages.CSharp.DotNetCommandLineArgument;
            set => _settings.SetParameter<string>(nameof(Language), value);
        }

        public string? ProjectNamePattern
        {
            get => _settings.GetParameter<string>(nameof(ProjectNamePattern)) ?? ProjectNamePattern_DefaultValue;
            set => _settings.SetParameter<string>(nameof(ProjectNamePattern), value);
        }
        public static string GetProjectTypeDefaultValue(string layer)
        {
            return layer == CodeArchitectureLayerArtifact.PRESENTATION_LAYER ? DotNetProjectType.WinFormsLib : DotNetProjectType.ClassLib;
        }
        public static GeneratorSettingsDescription CreateDescription(DotNetProjectGenerator<TLayer> generator)
        {
            var id = $"{generator.Layer}Layer.{generator.Scope}Scope.DotNetProject";
            var name = $".NET {generator.Layer} Layer {generator.Scope} Scope Project Generator";
            var description = $"Generates .NET projects for the {generator.Layer} Layer within the {generator.Scope} Scope.";
            var settingsDescription = new GeneratorSettingsDescription(id, name, description);
            // ProjectType
            settingsDescription.ParameterDefinitions.Add(new ParameterDefinition
            {
                Name = nameof(ProjectType),
                PossibleValues = DotNetProjectType.AllTypes.Select(t => (object)new ComboboxItem { DisplayName = t, Value = t }).ToList(),
                Description = "The type of .NET project to generate (e.g., Class Library, Console App, Web API).",
                Type = ParameterDefinitionTypes.String,
                DefaultValue = GetProjectTypeDefaultValue(generator.Layer),
                Required = true
            });
            // TargetFramework
            settingsDescription.ParameterDefinitions.Add(new ParameterDefinition
            {
                Name = nameof(TargetFramework),
                PossibleValues = TargetFrameworks.AllFrameworks.Select(t => (object)new ComboboxItem { DisplayName = t, Value = t }).ToList(),
                Description = "The target framework for the .NET project (e.g., net8.0, net7.0).",
                Type = ParameterDefinitionTypes.String,
                DefaultValue = TargetFrameworks.Net8,
                Required = true
            });
            // DotNet Language
            settingsDescription.ParameterDefinitions.Add(new ParameterDefinition
            {
                Name = nameof(Language),
                PossibleValues = DotNetLanguages.AllLanguages.Select(language => (object)new ComboboxItem { DisplayName = $"{language.DotNetCommandLineArgument} (*.{language.ProjectFileExtension})", Value = language.DotNetCommandLineArgument }).ToList(),
                Description = "The programming language for the .NET project (e.g., C#, VB.NET).",
                Type = ParameterDefinitionTypes.String,
                DefaultValue = DotNetLanguages.CSharp.DotNetCommandLineArgument,
                Required = true
            });
            
            // Project Name Pattern
            settingsDescription.ParameterDefinitions.Add(new ParameterDefinition
            {
                Name = nameof(ProjectNamePattern),
                Description = "Pattern for naming the .NET project files.",
                Type = ParameterDefinitionTypes.ParameterisedString,
                DefaultValue = ProjectNamePattern_DefaultValue,
                PossibleValues = new List<object>
                {
                    new ParameterizedStringParameter{ Description = "Use {Layer} for the layer name.", Parameter=ProjectNamePattern_LayerParameter, ExampleValue="Application" },
                    new ParameterizedStringParameter{ Description = "Use {Scope} for the scope name.", Parameter=ProjectNamePattern_ScopeParameter, ExampleValue="Shared" },
                    new ParameterizedStringParameter{ Description = "Use {Language} for the programming language.", Parameter=ProjectNamePattern_LanguageParameter, ExampleValue="C#" },
                    new ParameterizedStringParameter{ Description = "Use {WorkspaceNamespace} for the root namespace.", Parameter=ProjectNamePattern_WorkspaceNamespaceParameter, ExampleValue=WorkspaceSettings.Instance.RootNamespace }
                },
                Required = true
            });

            if (generator.Layer == CodeArchitectureLayerArtifact.APPLICATION_LAYER)
            {
                settingsDescription.DependingGeneratorIds.Add(nameof(ApplicationLayerGenerator));
            }
            else if (generator.Layer == CodeArchitectureLayerArtifact.DOMAIN_LAYER)
            {
                settingsDescription.DependingGeneratorIds.Add(nameof(DomainLayerGenerator));
            }
            else if (generator.Layer == CodeArchitectureLayerArtifact.INFRASTRUCTURE_LAYER)
            {
                settingsDescription.DependingGeneratorIds.Add(nameof(InfrastructureLayerGenerator));
            }
            else if (generator.Layer == CodeArchitectureLayerArtifact.PRESENTATION_LAYER)
            {
                settingsDescription.DependingGeneratorIds.Add(nameof(PresentationLayerGenerator));
            }

            return settingsDescription;
        }
    }
}
