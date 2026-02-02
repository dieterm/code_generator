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
                if (scope.DotNetProjectArtifacts.TryGetValue(CodeArchitectureLayerArtifact.APPLICATION_LAYER, out DotNetProjectArtifact? applicationProject))
                {
                    // Application -> Infrastructure
                    if (scope.DotNetProjectArtifacts.TryGetValue(CodeArchitectureLayerArtifact.INFRASTRUCTURE_LAYER, out DotNetProjectArtifact? infrastructureProject))
                    {
                        infrastructureProject.ProjectReferences.Add(new DotNetProjectReference(applicationProject));
                    }
                    // Application -> Domain
                    if (scope.DotNetProjectArtifacts.TryGetValue(CodeArchitectureLayerArtifact.DOMAIN_LAYER, out DotNetProjectArtifact? domainProject))
                    {
                        domainProject.ProjectReferences.Add(new DotNetProjectReference(applicationProject));
                    }

                    // Presentation -> Application
                    if (scope.DotNetProjectArtifacts.TryGetValue(CodeArchitectureLayerArtifact.PRESENTATION_LAYER, out DotNetProjectArtifact? presentationProject))
                    {
                        presentationProject.ProjectReferences.Add(new DotNetProjectReference(applicationProject));
                    }
                }
            }

            // 2. Cross-scope references (eg. Application.Shared -> Application.Application)
            var appScopeReferences = references[CodeArchitectureLayerArtifact.APPLICATION_SCOPE];

            foreach (var (layer, layerArtifact) in appScopeReferences.DotNetProjectArtifacts)
            {
                foreach (var targetScope in references.Where(s => s.ScopeArtifact.Name != CodeArchitectureLayerArtifact.APPLICATION_SCOPE))
                {
                    if (targetScope.DotNetProjectArtifacts.TryGetValue(layer, out DotNetProjectArtifact? targetProject))
                    {
                        layerArtifact.ProjectReferences.Add(new DotNetProjectReference(targetProject));
                    }
                }
            }

            // 3. Generate the updated project files with the references
            foreach (var scope in references)
            {
                foreach (var (_, projectArtifact) in scope.DotNetProjectArtifacts)
                {
                    var dotNetProjectTemplate = new DotNetProjectTemplate(projectArtifact.ProjectType, projectArtifact.Language, projectArtifact.TargetFramework);
                    var dotNetProjectTemplateInstance = new DotNetProjectTemplateInstance(dotNetProjectTemplate, projectArtifact.Name);
                    foreach (var nugetPackage in projectArtifact.NuGetPackages)
                    {
                        dotNetProjectTemplateInstance.Packages.Add(nugetPackage);
                    }
                    foreach (var projectReference in projectArtifact.ProjectReferences)
                    {
                        dotNetProjectTemplateInstance.ProjectReferences.Add(projectReference);
                    }
                    if (!e.Result.PreviewOnly)
                    {
                        dotNetProjectTemplateInstance.OutputDirectory = projectArtifact.FindAncesterOfType<FolderArtifact>()?.FullPath;
                    }
                    var messageBus = ServiceProviderHolder.GetRequiredService<ApplicationMessageBus>();
                    messageBus.Publish(new ReportTaskProgressEvent($"Updating {projectArtifact.Name} .NET project with references...", null));
                    // Use await for proper async handling - no deadlock risk
                    var result = _dotNetProjectTemplateEngine.RenderAsync(dotNetProjectTemplateInstance, CancellationToken.None).GetAwaiter().GetResult();
                    messageBus.Publish(new ReportTaskProgressEvent($"Finished updating {projectArtifact.Name} .NET project.", null));
                    if (result.Succeeded)
                    {
                        // for now, only add the project file as child artifact
                        var projectFileArtifact = result.Artifacts.OfType<FileArtifact>().FirstOrDefault(f => f.FileName.EndsWith(projectArtifact.Language.ProjectFileExtension));
                        AddChildArtifactToParent(projectArtifact, projectFileArtifact, e.Result);
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            e.Result.Errors.Add(error);
                        }
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
