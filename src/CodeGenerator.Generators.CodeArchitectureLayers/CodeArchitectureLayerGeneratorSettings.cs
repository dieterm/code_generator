using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Settings;
using CodeGenerator.Core.Settings.Generators;
using CodeGenerator.Core.Workspaces.Settings;
using CodeGenerator.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.CodeArchitectureLayers
{
    public class CodeArchitectureLayerGeneratorSettings
    {
        public const string FolderNamePattern_WorkspaceNamespaceParameter = "WorkspaceNamespace";
        public const string FolderNamePattern_LayerParameter = "Layer";
        public const string FolderNamePattern_ScopeParameter = "Scope";
        public const string FolderNamePattern_DefaultValue = "{WorkspaceNamespace}.{Layer}.{Scope}";
        public const string FolderNamePattern_DomainNamespaceParameter = "DomainNamespace";

        private readonly GeneratorSettings _settings;

        public CodeArchitectureLayerGeneratorSettings(GeneratorSettings settings)
        {
            _settings = settings;
        }

        public string? FolderNamePattern
        {
            get => _settings.GetParameter<string>(nameof(FolderNamePattern))?? FolderNamePattern_DefaultValue;
            set => _settings.SetParameter<string>(nameof(FolderNamePattern), value);
        }

        public static GeneratorSettingsDescription CreateDescription(string id, string name, string description, string layerName)
        { 
            var settingsDescription = new GeneratorSettingsDescription(id, name, description);
            settingsDescription.ParameterDefinitions.Add(new ParameterDefinition
            {
                Name = nameof(FolderNamePattern),
                Description = "The pattern used to name folders for the generated layer artifacts. Supports parameters like {Layer}, {Scope}, and {WorkspaceNamespace}.",
                Type = ParameterDefinitionTypes.ParameterisedString,
                DefaultValue = FolderNamePattern_DefaultValue,
                PossibleValues = new List<object>
                {
                    new ParameterizedStringParameter{ Description = "Use {Layer} for the layer name.", Parameter=FolderNamePattern_LayerParameter, ExampleValue=layerName },
                    new ParameterizedStringParameter{ Description = "Use {Scope} for the scope name.", Parameter=FolderNamePattern_ScopeParameter, ExampleValue="Shared" },
                    new ParameterizedStringParameter{ Description = "Use {WorkspaceNamespace} for the root namespace.", Parameter=FolderNamePattern_WorkspaceNamespaceParameter, ExampleValue=WorkspaceSettings.Instance.RootNamespace },
                    new ParameterizedStringParameter{ Description = "Use {DomainNamespace} for the domain namespace. Only applicable for domain scopes.", Parameter=FolderNamePattern_DomainNamespaceParameter, ExampleValue=$"MyAppDomainA" },
                },
            });
            return settingsDescription;
        }
    }
}
