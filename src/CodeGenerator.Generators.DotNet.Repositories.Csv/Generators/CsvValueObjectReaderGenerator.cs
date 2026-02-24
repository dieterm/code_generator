using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Workspaces.Artifacts.CodeArchitecture.OnionArchitecture;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Generators.DotNet.Events;
using CodeGenerator.Generators.DotNet.Repositories.Csv.Workspace.Artifacts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Repositories.Csv
{
    public class CsvValueObjectReaderGenerator : GeneratorBase
    {
        public const string GENERATOR_ID = "DotNet.Services.CsvValueObjectReader";
        private readonly ILogger _logger;
        private Func<DotNetProjectArtifactCreatedEventArgs, Task>? _dotnetprojectcreated_unsubscribe_handler;

        public CsvValueObjectReaderGenerator(ILogger<CsvValueObjectReaderGenerator> logger)
        {
            _logger = logger;
        }

        public override void SubscribeToEvents(GeneratorMessageBus messageBus)
        {
            _dotnetprojectcreated_unsubscribe_handler = messageBus.Subscribe<DotNetProjectArtifactCreatedEventArgs>(
                async (e) => await OnDotNetProjectCreated(e),
                InfrastructureLayerFilter
            );
        }

        private bool InfrastructureLayerFilter(DotNetProjectArtifactCreatedEventArgs args)
        {
            if (!Enabled) return false;
            return args.Result.Workspace.CodeArchitecture is OnionCodeArchitecture onionCodeArchitecture
                && args.Layer == onionCodeArchitecture.InfrastructureLayer.LayerName;
        }

        private async Task OnDotNetProjectCreated(DotNetProjectArtifactCreatedEventArgs e)
        {
            if (!Enabled) return;
            if (!(e.Result.Workspace.CodeArchitecture is OnionCodeArchitecture))
            {
                // The code architecture of the workspace is not Onion, this generator is only applicable for Onion architecture, so we skip generation
                _logger.LogWarning("The code architecture of the workspace is not Onion, skipping generation");
            }

            var scope = e.Result.Workspace.FindScope(e.Scope) as OnionScopeArtifact;
            if (scope == null)
            {
                _logger.LogWarning("The scope is not an OnionScopeArtifact, skipping generation");
                return;
            }

            var infrastructureLayer = scope.Infrastructure;
            if (infrastructureLayer == null)
            {
                _logger.LogInformation("No infrastructure layer defined, nothing to generate");
                return;
            }
            var csvReaderBase = infrastructureLayer.Services.FindDescendants<CsvValueObjectReaderArtifact>().SingleOrDefault();
            var csvReaderImplementations = infrastructureLayer.Services.FindDescendants<CsvValueObjectReaderImplementationArtifact>().ToList();
            if(csvReaderBase==null && csvReaderImplementations.Count == 0)
            {
                _logger.LogInformation("No CsvValueObjectReaderArtifact or CsvValueObjectReaderImplementationArtifact found in the infrastructure layer, nothing to generate");
                return;
            }
            var servicesFolderArtifact = e.DotNetProjectArtifact.Children.OfType<FolderArtifact>().FirstOrDefault(f => f.FolderName == "Services");
            if (servicesFolderArtifact == null)
            {
                servicesFolderArtifact = new FolderArtifact("Services");
                AddChildArtifactToParent(e.DotNetProjectArtifact, servicesFolderArtifact, e.Result);
            }

            if (csvReaderBase != null) {
                var codeFileArtifact = new CodeFileArtifact(csvReaderBase.CodeFileElement);
                AddChildArtifactToParent(servicesFolderArtifact, codeFileArtifact, e.Result);
                e.DotNetProjectArtifact.AddNuGetPackage(new NuGetPackage { PackageId = "CsvHelper", Version = "33.1.0" });
            }

            foreach (var csvReaderImplementation in csvReaderImplementations)
            {
                var implementationCodeFileArtifact = new CodeFileArtifact(csvReaderImplementation.CodeFileElement);
                AddChildArtifactToParent(servicesFolderArtifact, implementationCodeFileArtifact, e.Result);
            }
        }

        public override void UnsubscribeFromEvents(GeneratorMessageBus messageBus)
        {
            if (_dotnetprojectcreated_unsubscribe_handler != null)
            {
                messageBus.Unsubscribe(_dotnetprojectcreated_unsubscribe_handler);
                _dotnetprojectcreated_unsubscribe_handler = null;
            }
        }

        protected override GeneratorSettingsDescription ConfigureSettingsDescription()
        {
            var id = GENERATOR_ID;
            var name = $"CsvValueObjectReader Generator";
            var description = $"Generates ValueType classes for domain layers";
            var settingsDescription = new GeneratorSettingsDescription(id, name, description);
            return settingsDescription;
        }
    }
}
