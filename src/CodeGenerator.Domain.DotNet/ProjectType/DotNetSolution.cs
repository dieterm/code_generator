using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DotNet.ProjectType
{
    public class DotNetSolution : DotNetSolutionFolder
    {
        public string FileTitle { get; }
        public string SolutionGuid { get; } = Guid.NewGuid().ToString().ToUpper();

        public DotNetSolution(string fileTitle)
        {
            FileTitle = fileTitle;

            HideSolutionNode = false;

            SolutionItems = new DotNetSolutionFolder("Solution Items", null);
            SubFolders.Add(SolutionItems);
        }

        public bool HideSolutionNode {
            get { return SolutionProperties["HideSolutionNode"] == "TRUE"; }
            set { SolutionProperties["HideSolutionNode"] = value ? "TRUE" : "FALSE"; }
        }

        /// <summary>
        /// Project root items that will be added to the special "Solution Items" folder in the solution explorer. 
        /// These items will be added to the .sln file, but not to any .csproj file.
        /// </summary>
        public DotNetSolutionFolder SolutionItems { get; } 

        /*
Project Types
ASP.NET (Web Application)	349C5851-65DF-11DA-9384-00065B846F21
ASP.NET MVC	603C0E0B-DB56-11DC-BE95-000D561079B0
ASP.NET Core (Empty)	9A19103F-16F7-4668-BE54-9A1E7A4F7556
Website	E24C65DC-7377-472B-9ABA-BC803B73C61A
WCF	3D9AD99F-2412-4246-B90B-4EAA41C64699
WPF (Browser Application)	60DC8134-EBA5-43B8-BCC9-BB4BC16C2548
Python (PTVS)	888888A0-9F3D-457C-B088-3A5042F75D52
Node.js	9092AA53-FB77-4645-B42D-1CCCA6BD08BD
         */

        /*
Speciale / virtuele types
Type	GUID
Solution Folder	2150E333-8FDC-42A3-9474-1A3956D46DE8
Shared Project	D954291E-2A0B-460D-934E-DC6B0785DB48
Database	C8D11400-126E-41CD-887F-60BD40844F9E
Test Project	3AC096D0-A1C2-E12C-1390-A8335801FDAB
Setup / Deployment	54435603-DBB4-11D2-8724-00A0C9A8B90C

         */

        public string GenerateSlnFile(string solutionFolderPath)
        {
            var content = new StringBuilder();
            AppendFileHeader(content);
            foreach (var projectReference in Projects)
            {
                AppendDotNetProject(solutionFolderPath, content, projectReference);
            }
            AppendSolutionFolder(solutionFolderPath, content, this);
            foreach (var solutionFolder in SubFolders)
            {
                AppendSolutionFolder(solutionFolderPath, content, solutionFolder);
            }
            AppendGlobal(content);
            return content.ToString();
        }

        private IEnumerable<DotNetProjectReference> GetAllProjectReferences()
        {
            var projectReferences = new List<DotNetProjectReference>();
            void AddProjectReferences(DotNetSolutionFolder solutionFolder)
            {
                projectReferences.AddRange(solutionFolder.Projects);
                foreach (var subFolder in solutionFolder.SubFolders)
                {
                    AddProjectReferences(subFolder);
                }
            }
            AddProjectReferences(this);
            return projectReferences;
        }

        private void AppendGlobal(StringBuilder content)
        {
            var buildConfigurations = new[] { "Debug", "Release" };
            var cpuArchitectures = new[] { "Any CPU", "x64", "x86" };

            content.AppendLine("Global");

            AppendGlobalSectionSolutionConfigurationPlatforms(content, buildConfigurations, cpuArchitectures);
            AppendGlobalSectionProjectConfigurationPlatforms(content, buildConfigurations, cpuArchitectures);
            AppendGlobalSectionSolutionProperties(content, this.SolutionProperties);
            AppendGlobalSectionNestedProjects(content);
            AppendGlobalSectionExtensibilityGlobals(content);
            content.AppendLine("EndGlobal");
        }

        private void AppendGlobalSectionExtensibilityGlobals(StringBuilder content)
        {
            /*
             * GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {AF76BCB8-B816-4DA2-B1B6-D1F900E7DD06}
	EndGlobalSection

            // use SolutionGuid
             */
            content.AppendLine("\tGlobalSection(ExtensibilityGlobals) = postSolution");
            content.AppendLine($"\t\tSolutionGuid = {{{SolutionGuid}}}");
            content.AppendLine("\tEndGlobalSection");
        }

        private void AppendGlobalSectionProjectConfigurationPlatforms(StringBuilder content, string[] buildConfigurations, string[] cpuArchitectures)
        {
            content.AppendLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
            // foreach project, we need to add the following lines:
            /*
             {A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D}.Debug|x64.ActiveCfg = Debug|Any CPU
		{A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D}.Debug|x64.Build.0 = Debug|Any CPU
		{A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D}.Debug|x86.ActiveCfg = Debug|Any CPU
		{A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D}.Debug|x86.Build.0 = Debug|Any CPU
             */
            foreach (var projectReference in GetAllProjectReferences())
            {
                foreach (var buildConfiguration in buildConfigurations)
                {
                    foreach (var cpuArchitecture in cpuArchitectures)
                    {
                        content.AppendLine($"\t\t{{{projectReference.ProjectArtifact.Id}}}.{buildConfiguration}|{cpuArchitecture}.ActiveCfg = {buildConfiguration}|Any CPU");
                        content.AppendLine($"\t\t{{{projectReference.ProjectArtifact.Id}}}.{buildConfiguration}|{cpuArchitecture}.Build.0 = {buildConfiguration}|Any CPU");
                    }
                }
            }
            content.AppendLine("\tEndGlobalSection");
        }

        private static void AppendGlobalSectionSolutionConfigurationPlatforms(StringBuilder content, string[] buildConfigurations, string[] cpuArchitectures)
        {
            content.AppendLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
            foreach (var buildConfiguration in buildConfigurations)
            {
                foreach (var cpuArchitecture in cpuArchitectures)
                {
                    content.AppendLine($"\t\t{buildConfiguration}|{cpuArchitecture} = {buildConfiguration}|{cpuArchitecture}");
                }
            }
            /*Debug|Any CPU = Debug|Any CPU
		Debug|x64 = Debug|x64
		Debug|x86 = Debug|x86
		Release|Any CPU = Release|Any CPU
		Release|x64 = Release|x64
		Release|x86 = Release|x86*/
            //content.AppendLine("\t\tDebug|Any CPU = Debug|Any CPU");
            //content.AppendLine("\t\tRelease|Any CPU = Release|Any CPU");
            content.AppendLine("\tEndGlobalSection");
        }

        private static void AppendGlobalSectionSolutionProperties(StringBuilder content, Dictionary<string, string> solutionProperties)
        {
            /*
                         GlobalSection(SolutionProperties) = preSolution
                    HideSolutionNode = FALSE
                EndGlobalSection
                         */
            content.AppendLine("\tGlobalSection(SolutionProperties) = preSolution");
            foreach(var property in solutionProperties)
            {
                content.AppendLine($"\t\t{property.Key} = {property.Value}");
            }
            content.AppendLine("\tEndGlobalSection");
        }
        public Dictionary<string, string> SolutionProperties { get; } = new Dictionary<string, string>();
        private void AppendGlobalSectionNestedProjects(StringBuilder content)
        {
            /*
             GlobalSection(NestedProjects) = preSolution
		{F6BC2510-EECF-EAFD-AFFD-E1F5C7D715D8} = {CD686E50-9A6A-408E-AE48-E7DEEAD3BAC8}
		{B45A9506-A877-33F0-9A9C-494A75A4559F} = {CD686E50-9A6A-408E-AE48-E7DEEAD3BAC8}
		{74C85979-AC42-41C6-8914-4A495E13F120} = {ED8AA5E0-9601-4995-910B-776F8204C303}
		{86F5D696-B17C-431E-861A-A76AB80C0FA8} = {ED8AA5E0-9601-4995-910B-776F8204C303}
            ...
             */
            content.AppendLine("\tGlobalSection(NestedProjects) = preSolution");
            void AppendNestedProjects(DotNetSolutionFolder solutionFolder)
            {
                foreach (var projectReference in solutionFolder.Projects)
                {
                    content.AppendLine($"\t\t{{{projectReference.ProjectArtifact.Id}}} = {{{solutionFolder.Id}}}");
                }
                foreach (var subFolder in solutionFolder.SubFolders)
                {
                    content.AppendLine($"\t\t{{{subFolder.Id}}} = {{{solutionFolder.Id}}}");
                    AppendNestedProjects(subFolder);
                }
            }
            AppendNestedProjects(this);
            content.AppendLine("\tEndGlobalSection");
        }

        private static void AppendDotNetProject(string solutionFolderPath, StringBuilder content, DotNetProjectReference projectReference)
        {
            var projectRelativePath = Path.GetRelativePath(solutionFolderPath, projectReference.ProjectArtifact.GetProjectFilePath());
            content.AppendLine($"Project(\"{{{projectReference.ProjectArtifact.Language.SolutionTypeGuid}}}\") = \"{projectReference.ProjectArtifact.Name}\", \"{projectRelativePath}\", \"{{{projectReference.ProjectArtifact.Id}}}\"");
            content.AppendLine("EndProject");
        }

        private void AppendFileHeader(StringBuilder content)
        {
            content.AppendLine("Microsoft Visual Studio Solution File, Format Version 12.00");
            content.AppendLine("# Visual Studio Version 17");
            content.AppendLine("VisualStudioVersion = 17.14.36804.6");
            content.AppendLine("MinimumVisualStudioVersion = 10.0.40219.1");
        }

        private void AppendSolutionFolder(string solutionFolderPath, StringBuilder content, DotNetSolutionFolder solutionFolder)
        {
            /*
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "Design Patterns", "Design Patterns", "{DC28153C-893D-412A-AAC7-1BB5F175F823}"
	ProjectSection(SolutionItems) = preProject
		..\docs\Design Patterns\README.md = ..\docs\Design Patterns\README.md
	EndProjectSection
EndProject
             */
            if (solutionFolder.IsRoot) return;

            content.AppendLine($"Project(\"{{{solutionFolder.SolutionTypeGuid}}}\") = \"{solutionFolder.Name}\", \"{solutionFolder.Name}\", \"{{{solutionFolder.Id}}}\"");

            if (solutionFolder.Items.Any())
            {
                content.AppendLine("\tProjectSection(SolutionItems) = preProject");
                foreach (var item in solutionFolder.Items)
                {
                    var itemRelativePath = Path.GetRelativePath(solutionFolderPath, item.FilePath);
                    content.AppendLine($"\t\t{itemRelativePath} = {itemRelativePath}");
                }
                content.AppendLine("\tEndProjectSection");
            }

            //if(solutionFolder.Projects.Any())
            //{
            //    content.AppendLine("\tProjectSection(ProjectDependencies) = postProject");
            //    foreach(var project in solutionFolder.Projects)
            //    {
            //        content.AppendLine($"\t\t{{{project.ProjectArtifact.Id}}} = {{{project.ProjectArtifact.Id}}}");
            //    }
            //    content.AppendLine("\tEndProjectSection");
            //}

            //if(solutionFolder.SubFolders.Any())
            //{
            //    content.AppendLine("\tProjectSection(SolutionItems) = preProject");
            //    foreach(var subFolder in solutionFolder.SubFolders)
            //    {
            //        content.AppendLine($"\t\t{subFolder.Name} = {subFolder.Name}");
            //    }
            //    content.AppendLine("\tEndProjectSection");
            //}

            content.AppendLine("EndProject");

            foreach (var projectReference in solutionFolder.Projects)
            {
                AppendDotNetProject(solutionFolderPath, content, projectReference);
            }
            foreach (var subFolder in solutionFolder.SubFolders)
            {
                AppendSolutionFolder(solutionFolderPath, content, subFolder);
            }
        }

        public string GenerateSlnxFile(string solutionFolderPath)
        {
            throw new NotImplementedException("TODO: add support for .slnx files");
        }

        public void SaveAsSlnFile(string folderPath)
        {
            if (folderPath == null) throw new ArgumentNullException(nameof(folderPath));
            var solutionFilePath = Path.Combine(folderPath, $"{FileTitle}.sln");
            var solutionFileContent = GenerateSlnFile(folderPath);
            File.WriteAllText(solutionFilePath, solutionFileContent);
        }

        public void SaveAsSlnxFile(string folderPath)
        {
            if (folderPath == null) throw new ArgumentNullException(nameof(folderPath));
            var solutionFilePath = Path.Combine(folderPath, $"{FileTitle}.slnx");
            var solutionFileContent = GenerateSlnxFile(folderPath);
            File.WriteAllText(solutionFilePath, solutionFileContent);
        }

        /// <summary>
        /// Adds a project reference to the current project.
        /// Take into account it's subfolder structure
        /// </summary>
        /// <param name="projectRef">The project reference to add. Cannot be null.</param>
        /// <exception cref="NotImplementedException">The method is not implemented.</exception>
        public void AddProjectReference(DotNetProjectReference projectRef)
        {
            var subfolder = projectRef.ProjectArtifact.SolutionSubFolder;
            var subFolders = new Stack<string>(subfolder?.Split(Path.DirectorySeparatorChar) ?? Array.Empty<string>());
            DotNetSolutionFolder targetFolder = this;
            while (subFolders.Any())
            {
                var subFolderName = subFolders.Pop();
                var existingSubFolder = targetFolder.SubFolders.FirstOrDefault(f => f.Name == subFolderName);
                if (existingSubFolder != null)
                {
                    targetFolder = existingSubFolder;
                }
                else
                {
                    targetFolder = targetFolder.CreateSubFolder(subFolderName);
                }
            }
            targetFolder.Projects.Add(projectRef);
        }

    }
}
