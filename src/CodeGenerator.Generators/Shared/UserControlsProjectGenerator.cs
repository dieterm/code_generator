using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Events;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Models.Schema;
using CodeGenerator.Core.Services;
using CodeGenerator.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.Shared
{
    public class UserControlsProjectGenerator : BaseProjectGenerator
    {
        public UserControlsProjectGenerator(ILogger<UserControlsProjectGenerator> logger)
            : base(logger, ArchitectureLayer.Shared, DotNetProjectType.WinFormsLib, nameof(UserControlsProjectGenerator), "UserControls Generator", "Shared UserControls for the application")
        {

        }

        public override string GetProjectName(DomainSchema schema)
        {
            var rootNamespace = GetRootNamespace();
            var projectName = $"{rootNamespace}.UserControls";
            return projectName;
        }

        public override void SubscribeToEvents(IGeneratorMessageBus messageBus)
        {
            base.SubscribeToEvents(messageBus);

            messageBus.Subscribe<CreatingProjectEventArgs>(OnCreatingProject);
        }

        protected override List<NuGetPackageInfo> GetRequiredNugetPackages()
        {
            return new List<NuGetPackageInfo>{
                NuGetPackages.Microsoft_Extensions_Configuration,
                NuGetPackages.Microsoft_Extensions_Configuration_Json,
                NuGetPackages.Microsoft_Extensions_DependencyInjection,
                NuGetPackages.Microsoft_Extensions_Hosting,
                NuGetPackages.Microsoft_Extensions_Logging,
                NuGetPackages.Microsoft_Extensions_Logging_Debug,
                NuGetPackages.Syncfusion_Core_WinForms,
                NuGetPackages.Syncfusion_SfDataGrid_WinForms,
                NuGetPackages.Syncfusion_SfInput_WinForms,
                NuGetPackages.Syncfusion_SfListView_WinForms,
                NuGetPackages.Syncfusion_Shared_Base,
                NuGetPackages.Syncfusion_Tools_Windows
            };
        }

        private async void OnCreatingProject(CreatingProjectEventArgs args)
        {
            if (!IsThisProject(args.Project, args.Schema)) return;

            var projectName = GetProjectName(args.Schema);
            
            var sharedProjectGenerator = ServiceProviderHolder.GetRequiredService<SharedProjectGenerator>();
            var sharedProjectName = sharedProjectGenerator.GetProjectName(args.Schema);
            var processFileHandler = async (string projectName, FileInfo csFile, string relativePath) => {
                // skip bin and obj folders
                if (relativePath.StartsWith("bin") || relativePath.StartsWith("obj")) return null;
                // skip .csproj.user files (eg. ProjectXYZ.UserControls.csproj)
                if (csFile.Name.EndsWith(".csproj.user") || csFile.Name.EndsWith(".csproj")) return null;
                
                var fileContent = await File.ReadAllTextAsync(csFile.FullName);
                if (csFile.Name.EndsWith(".cs"))
                {
                    // replace namespace placeholder
                    fileContent = fileContent.Replace("namespace ProjectXYZ.UserControls", $"namespace {projectName}");
                    // replace using placeholders
                    fileContent = fileContent.Replace("using ProjectXYZ.UserControls", $"using {projectName}");
                    fileContent = fileContent.Replace("using ProjectXYZ.Shared", $"using {sharedProjectName}");
                }
                var fileRegistration = new FileRegistration
                {
                    FileName = csFile.Name,
                    RelativePath = relativePath,
                    Content = fileContent,
                    RegisteredBy = Id
                };
                return fileRegistration;
            };

            // copy all .cs files from the UserControl project template folder (and process them)
            var fileRegistrations = await ProcessFilesInTemplateFolder(projectName, "CSharp\\WinForms\\ProjectXYZ.UserControls", "*.*", processFileHandler);

            args.FileRegistrations.AddRange(fileRegistrations);
        }

    }
}
