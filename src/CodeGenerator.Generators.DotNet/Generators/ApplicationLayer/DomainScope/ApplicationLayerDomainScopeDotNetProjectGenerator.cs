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

namespace CodeGenerator.Generators.DotNet.Generators.ApplicationLayer.DomainScope
{
    public class ApplicationLayerDomainScopeDotNetProjectGenerator : DotNetProjectGenerator<ApplicationLayerArtifact>
    {
        public ApplicationLayerDomainScopeDotNetProjectGenerator()
            : base(CodeArchitectureLayerArtifact.APPLICATION_LAYER, null)
        {
            
        }
        public override bool LayerArtifactFilter(CreatedArtifactEventArgs e)
        {
            return e.Artifact is ApplicationLayerArtifact a && a.Scope != CodeArchitectureLayerArtifact.APPLICATION_SCOPE && a.Scope != CodeArchitectureLayerArtifact.SHARED_SCOPE;
        }

        //private void OnApplicationLayerApplicationScopeCreated(CreatedArtifactEventArgs args)
        //{
        //    var appLayerArtifact = args.Artifact as ApplicationLayerArtifact;
        //    if(appLayerArtifact == null) throw new ArgumentException("Artifact is not an ApplicationLayerArtifact");

        //    var projectName = $"{appLayerArtifact.Layer}.{appLayerArtifact.Scope}";
        //    // get from settings later
        //    var language = DotNetLanguages.CSharp;
        //    var targetFramework = TargetFrameworks.Net8;
        //    var projectType = DotNetProjectType.ClassLib;
        //    var dotNetProjectArtifact = new DotNetProjectArtifact(projectName, language, projectType, targetFramework);
        //    MessageBus.Publish(new CreatingArtifactEventArgs(args.Result, dotNetProjectArtifact));
        //    appLayerArtifact.AddChild(dotNetProjectArtifact);
        //    MessageBus.Publish(new CreatedArtifactEventArgs(args.Result, dotNetProjectArtifact));
        //}

        //public override void UnsubscribeFromEvents(GeneratorMessageBus messageBus)
        //{
        //    if(_unsubscribe_handler!=null)
        //        messageBus.Unsubscribe<CreatedArtifactEventArgs>(_unsubscribe_handler);
        //}

        //protected override GeneratorSettingsDescription ConfigureSettingsDescription()
        //{
        //    var id = $"ApplicationLayer.DomainScope.DotNetProject";
        //    var name = ".NET Application Layer Domain Scope Project Generator";
        //    var description = "Generates .NET projects for the Application Layer within the Domain Scope.";
        //    var templateRequirements = new List<TemplateRequirement>
        //    {
        //        // Define any template requirements specific to this generator
        //    };
        //    return new GeneratorSettingsDescription(id, name, description, templateRequirements);
        //}
    }
}
