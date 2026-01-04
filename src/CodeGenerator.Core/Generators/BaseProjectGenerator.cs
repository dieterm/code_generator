using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Events;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Output;
using CodeGenerator.Core.Models.Schema;
using CodeGenerator.Core.Services;
using CodeGenerator.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Generators
{
    public abstract class BaseProjectGenerator : MessageBusAwareGeneratorBase
    {
        //private readonly string _id;
        //private readonly string _name;
        public BaseProjectGenerator(ILogger logger, ArchitectureLayer layer, string projectType) 
            : base(logger)
        {
            //Id = id;
            //Name = name;
            //Description = description ?? string.Empty;
            ProjectType = projectType;
            Layer = layer;
        }
        protected string ProjectType { get; }
        protected ArchitectureLayer Layer { get; }

        //public override string Id { get; }

        //public override string Name { get; }

        //public override string Description { get; }

        public override void SubscribeToEvents(IGeneratorMessageBus messageBus)
        {
            messageBus.Subscribe<CreatingSolutionEventArgs>(OnCreatingSolution);
            messageBus.Subscribe<CreatedProjectEventArgs>(OnCreatedProject);
        }

        /// <summary>
        /// Override to specify additional NuGet packages required by this project generator
        /// </summary>
        /// <returns></returns>
        protected virtual List<NuGetPackageInfo> GetRequiredNugetPackages()
        {
            return new List<NuGetPackageInfo>();
        }

        protected virtual List<NuGetPackageInfo> GetNugetPackagesFromSettings(GeneratorSettings settings)
        {
            return settings.NuGetPackages
                .Where(p => p.Layers.Contains(Layer))
                .Select(p => new NuGetPackageInfo { PackageId = p.PackageId, Version = p.Version }).ToList();
        }

        protected virtual List<NuGetPackageInfo> GetNugetPackages(GeneratorSettings settings)
        {
            var nugetPackages = GetRequiredNugetPackages();
            var settingsNugetPackages = GetNugetPackagesFromSettings(settings);
            nugetPackages.AddRange(settingsNugetPackages
                .Where(p => !nugetPackages.Any(np => np.PackageId == p.PackageId && p.Version == np.Version))); // skip duplicates
            return nugetPackages;
        }

        

        public virtual string GetProjectName(DomainSchema schema)
        {
            var settings = GetSettings();
            var rootNamespace = GetRootNamespace();
            var schemaNamespace = GetSchemaNamespace(schema);
            var projectSettings = schema.CodeGenMetadata?.ProjectSettings;
            switch (Layer)
                {
                case ArchitectureLayer.Domain:
                    return $"{schemaNamespace}.{projectSettings?.DomainProjectName ?? "Domain"}";
                case ArchitectureLayer.Application:
                    return $"{schemaNamespace}.{projectSettings?.ApplicationProjectName ?? "Application"}";
                case ArchitectureLayer.Infrastructure:
                    return $"{schemaNamespace}.{projectSettings?.InfrastructureProjectName ?? "Infrastructure"}";
                case ArchitectureLayer.Shared:
                    return $"{rootNamespace}.{projectSettings?.SharedProjectName ?? "Shared"}";
                case ArchitectureLayer.Presentation:
                    return $"{schemaNamespace}.{projectSettings?.PresentationProjectName ?? "Presentation"}";
                default:
                    throw new NotImplementedException($"'GetProjectName' not implemented for generator {Id}");
            }
        }

        private void OnCreatingSolution(CreatingSolutionEventArgs args)
        {
            var nugetPackages = GetNugetPackages(args.Settings);

            // eg. "Geoservice.Shared"
            var projectName = GetProjectName(args.Schema);
            args.ProjectRegistrations.Add(new ProjectRegistration
            {
                Layer = Layer,
                ProjectName = projectName,
                ProjectType = ProjectType,
                ProjectPath = Path.Combine(args.Solution.SolutionPath, projectName),
                RegisteredBy = Id,
                TargetFramework = args.Settings.TargetFramework,
                NuGetPackages = nugetPackages
            });
        }

        protected bool IsThisProject(ProjectRegistration project, DomainSchema schema)
        {
            // eg. "Geoservice.Shared"
            var projectName = GetProjectName(schema);
            return project.ProjectName.Equals(projectName, StringComparison.OrdinalIgnoreCase);
        }
        protected bool IsThisProject(GeneratedProject project, DomainSchema schema)
        {
            // eg. "Geoservice.Shared"
            var projectName = GetProjectName(schema);
            return project.Name.Equals(projectName, StringComparison.OrdinalIgnoreCase);
        }
        private void OnCreatedProject(CreatedProjectEventArgs args)
        {
            if (!IsThisProject(args.Project, args.Schema)) return;

            // delete "Class1.cs" if exists
            var folderPath = args.Project.Directory;
            var class1FilePath = Path.Combine(folderPath, "Class1.cs");
            if (File.Exists(class1FilePath))
            {
                File.Delete(class1FilePath);
                Logger.LogInformation("Deleted default Class1.cs file at {FilePath}", class1FilePath);
            }
        }
        protected async Task<List<FileRegistration>> ProcessFilesInTemplateFolder(string projectName, string templateSubFolder, string filesFilter, Func<string, FileInfo, string, Task<FileRegistration?>> fileHandler)
        {
            var settings = GetSettings();
            var sharedProjectTemplateFolder = Path.Combine(settings.TemplateFolder, templateSubFolder);
            var projectDirInfo = new DirectoryInfo(sharedProjectTemplateFolder);
            if (!projectDirInfo.Exists)
            {
                throw new ArgumentException("'ProjectXYZ.Shared' directory does not exist: " + sharedProjectTemplateFolder);
            }
            var fileRegistrations = new List<FileRegistration>();
            // Add common files to the Shared project
            var csFiles = projectDirInfo.GetFiles(filesFilter, SearchOption.AllDirectories);
            foreach (var csFile in csFiles)
            {
                var relativePath = Path.GetRelativePath(sharedProjectTemplateFolder, csFile.FullName);
                // remove filename from path:
                relativePath = relativePath.Substring(0, relativePath.Length - csFile.Name.Length);
                // determine what to do with file
                var fileRegistration = await fileHandler(projectName, csFile, relativePath);
                // if FileRegistration-object is returned, than register it
                if (fileRegistration != null)
                    fileRegistrations.Add(fileRegistration);
            }
            
            return fileRegistrations;
        }

    }
}
