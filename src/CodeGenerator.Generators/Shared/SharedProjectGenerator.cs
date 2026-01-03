using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Events;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;
using CodeGenerator.Core.Models.Schema;
using CodeGenerator.Core.Services;
using CodeGenerator.Generators.Presentation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.Shared
{
    public class SharedProjectGenerator : BaseProjectGenerator
    {
        public SharedProjectGenerator(ILogger<SharedProjectGenerator> logger) 
            : base(logger, ArchitectureLayer.Shared, DotNetProjectType.ClassLib, nameof(SharedProjectGenerator), "Shared Project Generator", "Generates a common project structure")
        {
        }

        protected override List<NuGetPackageInfo> GetRequiredNugetPackages()
        {
            return new List<NuGetPackageInfo>
            {
                NuGetPackages.Microsoft_EntityFrameworkCore,
                NuGetPackages.Microsoft_EntityFrameworkCore_Relational,
                NuGetPackages.Microsoft_Extensions_Configuration,
                NuGetPackages.Microsoft_Extensions_Configuration_Json,
                NuGetPackages.Microsoft_Extensions_DependencyInjection,
                NuGetPackages.Microsoft_Extensions_Hosting,
                NuGetPackages.Microsoft_Extensions_Logging,
                NuGetPackages.Microsoft_Extensions_Logging_Debug
            };
        }

        public override void SubscribeToEvents(IGeneratorMessageBus messageBus)
        {
            base.SubscribeToEvents(messageBus);

            messageBus.Subscribe<CreatingProjectEventArgs>(OnCreatingProject);
        }


        private async void OnCreatingProject(CreatingProjectEventArgs args)
        {
            var projectName = GetProjectName(args.Schema);
            if (!IsThisProject(args.Project, args.Schema))
            {
                // other projects should have a reference to this Shared project
                args.AdditionalProjectReferences.Add(projectName);
                // nothing else to do
                return;
            }
            // copy all .cs files from the Shared project template folder (and process them)
            var fileRegistrations = await ProcessFilesInTemplateFolder(projectName, "CSharp\\ProjectXYZ.Shared", "*.cs", ProcessFileInTemplate);
            
            args.FileRegistrations.AddRange(fileRegistrations);
        }

        //private async Task<List<FileRegistration>> ProcessFilesInTemplateFolder(string projectName, string templateSubFolder, string filesFilter)
        //{
        //    var settings = GetSettings();
        //    var sharedProjectTemplateFolder = Path.Combine(settings.TemplateFolder, templateSubFolder);
        //    var projectDirInfo = new DirectoryInfo(sharedProjectTemplateFolder);
        //    if (!projectDirInfo.Exists)
        //    {
        //        throw new ArgumentException("'ProjectXYZ.Shared' directory does not exist: " + sharedProjectTemplateFolder);
        //    }
        //    var fileRegistrations = new List<FileRegistration>();
        //    // Add common files to the Shared project
        //    var csFiles = projectDirInfo.GetFiles(filesFilter, SearchOption.AllDirectories);
        //    foreach (var csFile in csFiles)
        //    {
        //        var relativePath = Path.GetRelativePath(sharedProjectTemplateFolder, csFile.FullName);
        //        // remove filename from path:
        //        relativePath = relativePath.Substring(0, relativePath.Length - csFile.Name.Length);
        //        // determine what to do with file
        //        var fileRegistration = await ProcessFileInTemplate(projectName, csFile, relativePath);
        //        // if FileRegistration-object is returned, than register it
        //        if (fileRegistration != null)
        //            fileRegistrations.Add(fileRegistration);
        //    }
        //    return fileRegistrations;
        //}

        private async Task<FileRegistration?> ProcessFileInTemplate(string projectName, FileInfo csFile, string relativePath)
        {
            if (relativePath.StartsWith("bin") || relativePath.StartsWith("obj"))
            {
                return null; // skip bin and obj folders
            }
            var fileContent = await File.ReadAllTextAsync(csFile.FullName);
            if (csFile.Name.EndsWith(".cs"))
            {
                // replace namespace placeholder
                fileContent = fileContent.Replace("namespace ProjectXYZ.Shared", $"namespace {projectName}");
            }
            var fileRegistration = new FileRegistration
            {
                FileName = csFile.Name,
                RelativePath = relativePath,
                Content = fileContent,
                RegisteredBy = Id
            };
            return fileRegistration;
        }
    }
}
