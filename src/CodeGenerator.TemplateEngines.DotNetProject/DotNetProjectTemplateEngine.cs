using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Settings;
using CodeGenerator.Core.Templates;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Models;
using CodeGenerator.TemplateEngines.DotNetProject.Models;
using CodeGenerator.TemplateEngines.DotNetProject.Services;
using CodeGenerator.TemplateEngines.Scriban;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.TemplateEngines.DotNetProject
{
    public class DotNetProjectTemplateEngine : TemplateEngine<DotNetProjectTemplate, DotNetProjectTemplateInstance>
    {
        private readonly string[] _filesToIgnore = new string[]
        {
            "Class1.cs",
        };
        private readonly DotNetProjectService _dotNetProjectService;
        public DotNetProjectTemplateEngine( DotNetProjectService dotNetProjectService, ILogger<DotNetProjectTemplateEngine> logger)
            : base(logger, "dotnet_project_template_engine", "DotNet Project Template Engine", TemplateType.DotNetProject)
        {
            _dotNetProjectService = dotNetProjectService;

            SettingsDescription.ParameterDefinitions.Add(new Core.Settings.ParameterDefinition()
            {
                Name = $"{DotNetProjectType.ClassLib}_{DotNetLanguages.CSharp.Id}_template_id", // "classlib_csharp_template_id"
                Description = "ClassLib Csharp Project File Scriban Template",
                Type = ParameterDefinitionTypes.Template,
                PossibleValues = (new[] { TemplateType.Scriban }).Cast<object>().ToList()
            });

            //SettingsDescription.Templates.Add(new Core.Templates.Settings.TemplateRequirement()
            //{
            //    TemplateId = TemplateIdParser.BuildTemplateEngineTemplateId(Id, "DotNetProjectTemplate"),
            //    Description = "Generates a .NET project with specified type, framework, language and NuGet packages.",
            //    OutputFileNamePattern = "{{ProjectName}}",
            //    TemplateType = TemplateType.Scriban
            //});
        }
        public override void Initialize()
        {
            var templateManager = ServiceProviderHolder.GetRequiredService<TemplateManager>();
            foreach (var projectType in DotNetProjectType.AllTypes)
            {
                var templateName = $"{projectType}_{DotNetLanguages.CSharp.Id}";
                templateManager.RegisterRequiredTemplate(TemplateIdParser.BuildTemplateEngineTemplateId(Id, templateName, projectType));
            }
        }

        public override ITemplateInstance CreateTemplateInstance(ITemplate template)
        {
            return new DotNetProjectTemplateInstance((DotNetProjectTemplate)template, "ProjectXYZ");
        }

        public override async Task<TemplateOutput> RenderAsync(DotNetProjectTemplateInstance templateInstance, CancellationToken cancellationToken)
        {
            string? outputDir = templateInstance.OutputDirectory;
            if(outputDir != null && !Directory.Exists(outputDir))
            {
                try
                {
                    Directory.CreateDirectory(outputDir);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to create output directory at {OutputDir}", outputDir);
                    outputDir = null;
                }
                
            }
            if (outputDir == null)
            {
                using var tempDir = new TemporaryDirectory();
                outputDir = tempDir.Path;
            }
            
            var dotNetTemplate = (DotNetProjectTemplate)templateInstance.Template;

            var scribanTemplateId = TemplateIdParser.BuildTemplateEngineTemplateId(Id, $"{dotNetTemplate.DotNetProjectType}_{dotNetTemplate.DotNetLanguage.Id}", dotNetTemplate.DotNetProjectType);
            var templateManager = ServiceProviderHolder.GetRequiredService<TemplateManager>();
            List<string> searchedLocations;
            var scribanTemplateFilePath = templateManager.ResolveTemplateIdToPath(scribanTemplateId, out searchedLocations);
            
            var scribanTemplate = templateManager.GetTemplateById(scribanTemplateId);
            if (scribanTemplate != null)
            {
                Logger.LogInformation("Using Scriban template at '{scribanTemplateFilePath}' to create DotNet project file", scribanTemplateFilePath);

                // Create the project-file using Scriban template
                var templateEngineManager = ServiceProviderHolder.GetRequiredService<TemplateEngineManager>();
                var templateEngine = templateEngineManager.GetTemplateEnginesForTemplate(scribanTemplate).FirstOrDefault();
                if (templateEngine != null)
                {
                    var scribanTemplateInstance = templateEngine.CreateTemplateInstance(scribanTemplate);
                    if(scribanTemplateInstance is ScribanTemplateInstance sti)
                    {
                        sti.OutputFileName = templateInstance.ProjectName + ".csproj";
                    }
                    scribanTemplateInstance.SetParameter("DotNetProject", templateInstance);
                    
                    var result = await templateEngine.RenderAsync(scribanTemplateInstance, cancellationToken);
                    if (result.Succeeded) { 
                        return result;
                    }
                }
            } 
            else
            {
                Logger.LogInformation("No Scriban template with id '{scribanTemplateId}' found to create DotNet project file", scribanTemplateId);

                foreach (var loc in searchedLocations)
                {
                    Logger.LogInformation("\tSearched for Scriban template at: {location}", loc);
                }
            }

                // if we get here:
                // Create the project using DotNet CLI
                var project = await _dotNetProjectService.CreateProjectAsync(templateInstance.ProjectName, outputDir, dotNetTemplate.DotNetProjectType, dotNetTemplate.DotNetTargetFramework, dotNetTemplate.DotNetLanguage.DotNetCommandLineArgument, cancellationToken);
            
            foreach (var package in templateInstance.Packages)
            {
                await _dotNetProjectService.AddPackageAsync(project.Directory, package.PackageId, package.Version, cancellationToken);
            }

            var fileAndFolderArtifacts = BuildFilesAndFoldersArtifact(outputDir);
            return new TemplateOutput(fileAndFolderArtifacts);
        }

        private IEnumerable<IArtifact> BuildFilesAndFoldersArtifact(string tempDirPath, FolderArtifact? parentFolder = null)
        {
            var artifacts = new List<IArtifact>();
            foreach (var folder in Directory.GetDirectories(tempDirPath, "*", SearchOption.TopDirectoryOnly).OrderBy(k => k))
            {
                var folderName = Path.GetFileName(folder);
                var folderArtifact = new FolderArtifact(folderName);
                BuildFilesAndFoldersArtifact(folder, folderArtifact);
                parentFolder?.AddChild(folderArtifact);
                artifacts.Add(folderArtifact);
            }
            foreach (var file in Directory.GetFiles(tempDirPath, "*.*", SearchOption.TopDirectoryOnly).OrderBy(k => k))
            {
                var fileName = Path.GetFileName(file);
                if( _filesToIgnore.Contains(fileName))
                {
                    continue;
                }
                var fileArtifact = new FileArtifact(fileName);
                fileArtifact.SetTextContent(File.ReadAllText(file));
                parentFolder?.AddChild(fileArtifact);
                artifacts.Add(fileArtifact);
            }

            return parentFolder?.Children ?? artifacts;
        }

        
    }
}
