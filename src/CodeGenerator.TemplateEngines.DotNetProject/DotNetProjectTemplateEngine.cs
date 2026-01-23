using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Templates;
using CodeGenerator.Shared.Models;
using CodeGenerator.TemplateEngines.DotNetProject.Models;
using CodeGenerator.TemplateEngines.DotNetProject.Services;
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
        public DotNetProjectTemplateEngine(DotNetProjectService dotNetProjectService, ILogger<DotNetProjectTemplateEngine> logger)
            : base(logger, "dotnet_project_template_engine", "DotNet Project Template Engine", TemplateType.DotNetProject)
        {
            _dotNetProjectService = dotNetProjectService;
        }

        /// <summary>
        /// This template engine does not have a default file extension since it creates projects from parameters.
        /// </summary>
        public override string DefaultFileExtension => null;

        public override ITemplate CreateTemplateFromFile(string filePath)
        {
            throw new NotImplementedException();
        }

        public override ITemplateInstance CreateTemplateInstance(ITemplate template)
        {
            throw new NotImplementedException();
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
