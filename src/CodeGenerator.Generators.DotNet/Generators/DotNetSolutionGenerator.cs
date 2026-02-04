using CodeGenerator.Core.Artifacts.CodeGeneration;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Events.Application;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Core.Workspaces.Generators;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Generators.DotNet.Events;
using CodeGenerator.Shared;
using CodeGenerator.TemplateEngines.DotNetProject;
using CodeGenerator.TemplateEngines.DotNetProject.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Generators
{
    public class DotNetSolutionGenerator : GeneratorBase
    {
        private Action<DotNetProjectArtifactCreatedEventArgs>? _unsubscribe_dotnet_project_created_handler;
        private Func<CreatedArtifactEventArgs, Task>? _unsubscribe_created_root_artifact_handler;

        private readonly ILogger<DotNetSolutionGenerator> _logger;
        private readonly DotNetProjectTemplateEngine _dotNetProjectTemplateEngine;
        private readonly DotNetProjectService _dotNetProjectService;
        public DotNetSolutionGenerator(DotNetProjectTemplateEngine dotNetProjectTemplateEngine, DotNetProjectService dotNetProjectService,ILogger<DotNetSolutionGenerator> logger)
            : base()
        {
            _logger = logger;
            _dotNetProjectTemplateEngine = dotNetProjectTemplateEngine;
            _dotNetProjectService = dotNetProjectService;
        }

        public override void SubscribeToEvents(GeneratorMessageBus messageBus)
        {
            _unsubscribe_dotnet_project_created_handler = messageBus.Subscribe<DotNetProjectArtifactCreatedEventArgs>(
               (e) => OnDotNetProjectCreated(e)
           );
            _unsubscribe_created_root_artifact_handler = messageBus.Subscribe<CreatedArtifactEventArgs>(

                async (e) => await OnCreatedRootArtifact(e),
                (e) => e.Artifact is RootArtifact
            );
        }

        protected void OnDotNetProjectCreated(DotNetProjectArtifactCreatedEventArgs e)
        {
            if (!Enabled)
                return;

            e.Result.DotNetProjectReferences[e.Scope].DotNetProjectArtifacts[e.Layer] = e.DotNetProjectArtifact;
        }

        /// <summary>
        /// When we get here, the entire workspace is generated (including all DotNetProjectArtifacts
        /// </summary>
        protected async Task OnCreatedRootArtifact(CreatedArtifactEventArgs e)
        {
            if (!Enabled)
                return;

            // Create all required project references between the generated projects
            var references = e.Result.DotNetProjectReferences;

            // 1. Scope layers
            foreach (var scope in references)
            {
                if (scope.DotNetProjectArtifacts.TryGetValue(OnionCodeArchitecture.APPLICATION_LAYER, out DotNetProjectArtifact? applicationProject))
                {
                    // Application -> Infrastructure
                    if (scope.DotNetProjectArtifacts.TryGetValue(OnionCodeArchitecture.INFRASTRUCTURE_LAYER, out DotNetProjectArtifact? infrastructureProject))
                    {
                        applicationProject.AddProjectReference(new DotNetProjectReference(infrastructureProject));
                    }
                    // Application -> Domain
                    if (scope.DotNetProjectArtifacts.TryGetValue(OnionCodeArchitecture.DOMAIN_LAYER, out DotNetProjectArtifact? domainProject))
                    {
                        applicationProject.AddProjectReference(new DotNetProjectReference(domainProject));
                    }
                }

                if (scope.DotNetProjectArtifacts.TryGetValue(OnionCodeArchitecture.INFRASTRUCTURE_LAYER, out DotNetProjectArtifact? infrastructureProject2))
                {
                    // Infrastructure -> Domain
                    if (scope.DotNetProjectArtifacts.TryGetValue(OnionCodeArchitecture.DOMAIN_LAYER, out DotNetProjectArtifact? domainProject))
                    {
                        infrastructureProject2.AddProjectReference(new DotNetProjectReference(domainProject));
                    }
                }

                if (scope.DotNetProjectArtifacts.TryGetValue(OnionCodeArchitecture.PRESENTATION_LAYER, out DotNetProjectArtifact? presentationProject))
                {
                    // Presentation -> Application
                    if (scope.DotNetProjectArtifacts.TryGetValue(OnionCodeArchitecture.APPLICATION_LAYER, out DotNetProjectArtifact? applicationProject2))
                    {
                        presentationProject.AddProjectReference(new DotNetProjectReference(applicationProject2));
                    }
                }
                if(scope.ScopeArtifact.Name!= CodeArchitectureScopes.SHARED_SCOPE)
                {
                    // Other scopes (eg. ScopeA, ScopeB, etc) -> Shared
                    var sharedScope = references.FirstOrDefault(s => s.ScopeArtifact.Name == CodeArchitectureScopes.SHARED_SCOPE);
                    if (sharedScope != null)
                    {
                        foreach (var (layer, sharedProjectArtifact) in sharedScope.DotNetProjectArtifacts)
                        {
                            if (scope.DotNetProjectArtifacts.TryGetValue(layer, out DotNetProjectArtifact? targetProject))
                            {
                                targetProject.AddProjectReference(new DotNetProjectReference(sharedProjectArtifact));
                            }
                        }
                    }
                }
            }

            // 2. Cross-scope references (eg. Application.Shared -> Application.Application)
            var appScopeReferences = references[CodeArchitectureScopes.APPLICATION_SCOPE];

            // loop over <ns>.Application.Domain, <ns>.Application.Infrastructure, <ns>.Application.Presentation, <ns>.Application.Application
            foreach (var (layer, layerArtifact) in appScopeReferences.DotNetProjectArtifacts)
            {
                // get all other scopes except Application (eg. Shared, ScopeA, ScopeB, ScopeB/SubScope1, etc)
                foreach (var targetScope in references.Where(s => s.ScopeArtifact.Name != CodeArchitectureScopes.APPLICATION_SCOPE))
                {
                    // Shared.<layer> -> Application.<layer>
                    // ScopeA.<layer> -> Application.<layer>
                    // ...
                    if (targetScope.DotNetProjectArtifacts.TryGetValue(layer, out DotNetProjectArtifact? targetProject))
                    {
                        layerArtifact.AddProjectReference(new DotNetProjectReference(targetProject));
                    }
                }
            }
            // Application layer should have a reference to the Domain and Infrastructure layer
            // TODO: add reference from <scope>.Application -> <scope>.Domain



            // 3. Generate the updated project files with the references
            foreach (var scope in references)
            {
                foreach (var (_, projectArtifact) in scope.DotNetProjectArtifacts)
                {
                    var messageBus = ServiceProviderHolder.GetRequiredService<ApplicationMessageBus>();
                    messageBus.Publish(new ReportTaskProgressEvent($"Updating {projectArtifact.Name} .NET project with references...", null));
                    var projectFilePath = projectArtifact.GetProjectFilePath();
                    var errorMessage = projectArtifact.SaveProjectFile(projectFilePath, _logger);

                    messageBus.Publish(new ReportTaskProgressEvent($"Finished updating {projectArtifact.Name} .NET project.", null));
                    
                    if (errorMessage==null)
                    {
                        // for now, only add the project file as child artifact
                        var projectFileArtifact = new ExistingFileArtifact(projectFilePath);
                        projectFileArtifact.SetTextContent(File.ReadAllText(projectFilePath));    
                        AddChildArtifactToParent(projectArtifact, projectFileArtifact, e.Result);
                    }
                    else
                    {
                        e.Result.Errors.Add(errorMessage);
                    }
                }
            }

            // Generate solution file
            var solutionArtifact = new DotNetSolutionArtifact(e.Result.Workspace.Name, DotNetSolutionType.sln);
            foreach (var scope in references)
            {
                foreach (var (_, projectArtifact) in scope.DotNetProjectArtifacts)
                {
                    solutionArtifact.AddProjectReference(projectArtifact);
                }
            }
            AddChildArtifactToParent(e.Artifact, solutionArtifact, e.Result);

            solutionArtifact.AllProjectReferencesGenerated += SolutionArtifact_AllProjectReferencesGenerated;
        }

        private async void SolutionArtifact_AllProjectReferencesGenerated(object? sender, EventArgs e)
        {
            var applicationMessageBus = ServiceProviderHolder.GetRequiredService<ApplicationMessageBus>();
            applicationMessageBus.ReportApplicationStatus("Generating .NET solution file...");
            var solutionArtifact = sender as DotNetSolutionArtifact;
            if (solutionArtifact == null)
                return;
            solutionArtifact.AllProjectReferencesGenerated -= SolutionArtifact_AllProjectReferencesGenerated;
            _logger.LogInformation($"All project references generated for solution {solutionArtifact.Name}.");
            var projectFilePaths = solutionArtifact.ProjectReferences
                .Select(pr => pr.ProjectArtifact.GetProjectFilePath())
                .ToList();

            var solutionFilePath = await _dotNetProjectService.CreateSolutionAsync(solutionArtifact.Name, solutionArtifact.GetFolderPath(), solutionArtifact.SolutionType.ToString(), new List<string>());
            int totalProjects = solutionArtifact.ProjectReferences.Count;
            int currentProjectIndex = 0;
            foreach (var projectRef in solutionArtifact.ProjectReferences)
            {
                var projectFolderArtifact = projectRef.ProjectArtifact.Parent as FolderArtifact;
                //var scopeArtifact = projectFolderArtifact?.GetDecoratorOfType<LayerArtifactRefDecorator>()?.LayerArtifact.Parent as ScopeArtifact;
                var solutionSubFolder = projectRef.ProjectArtifact.SolutionSubFolder;
                _logger.LogInformation($"Adding project {projectRef.ProjectArtifact.Name} to solution {solutionArtifact.Name} in folder {solutionSubFolder}.");
                var projectFilePath = projectRef.ProjectArtifact.GetProjectFilePath();
                await _dotNetProjectService.AddProjectToSolutionAsync(solutionFilePath, solutionSubFolder, projectFilePath);
                currentProjectIndex++;
                int percentComplete = (int)((currentProjectIndex / (double)totalProjects) * 100);
             
                applicationMessageBus.ReportTaskProgress($"Added project {projectRef.ProjectArtifact.Name} to solution.", percentComplete);
            }
            applicationMessageBus.ReportApplicationStatus(".NET Solution Generation complete.");
        }

        public override void UnsubscribeFromEvents(GeneratorMessageBus messageBus)
        {
            if (_unsubscribe_dotnet_project_created_handler != null)
                messageBus.Unsubscribe<DotNetProjectArtifactCreatedEventArgs>(_unsubscribe_dotnet_project_created_handler);
            if (_unsubscribe_created_root_artifact_handler != null)
                messageBus.Unsubscribe<CreatedArtifactEventArgs>(_unsubscribe_created_root_artifact_handler);
        }

        protected override GeneratorSettingsDescription ConfigureSettingsDescription()
        {
            var id = nameof(DotNetSolutionGenerator);
            var name = $".NET Solution Generator";
            var description = $"Generates .NET solution file & handles project references.";
            var settingsDescription = new GeneratorSettingsDescription(id, name, description);
            return settingsDescription;
        }
    }
}
