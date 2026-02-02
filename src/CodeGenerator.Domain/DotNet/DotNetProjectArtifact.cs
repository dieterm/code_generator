using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Artifacts.TreeNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeGenerator.Domain.DotNet
{
    public class DotNetProjectArtifact : Artifact
    {
        public DotNetProjectArtifact(string name, DotNetLanguage language, string projectType, TargetFramework targetFramework)
        {
            //Id = $"DotNetProject:{Name}";
            Name = name;
            Language = language;
            ProjectType = projectType;
            TargetFramework = targetFramework;
            _treeNodeIcon = new ResourceManagerTreeNodeIcon($"{language.ImageKey}_project");
            NuGetPackages = new List<NuGetPackage>();
            ProjectReferences = new List<DotNetProjectReference>();
        }

        public DotNetProjectArtifact(ArtifactState state)
            : base(state)
        {
            _treeNodeIcon = new ResourceManagerTreeNodeIcon($"{Language.ImageKey}_project");
            if(NuGetPackages == null)
                NuGetPackages = new List<NuGetPackage>();
            if(ProjectReferences == null)
                ProjectReferences = new List<DotNetProjectReference>();
        }

        /// <summary>
        /// Returns the project file name, e.g. 'MyProject.csproj'
        /// </summary>
        public string ProjectFileName { get { return $"{Name}.{Language.ProjectFileExtension}"; } }

        /// <summary>
        /// Returns the full project file path in the workspace folder, e.g. 'C:\Projects\MyWorkspace\MyProject\MyProject.csproj'
        /// </summary>
        public string GetProjectFilePath()
        {
            var folderPath = GetFolderPath();
            return System.IO.Path.Combine(folderPath, ProjectFileName);
        }

        public string Name {
            get { return GetValue<string>(nameof(Name)); }
            set { 
                if(SetValue(nameof(Name), value)) { 
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
                    RaisePropertyChangedEvent(nameof(ProjectFileName));
                }
            }
        }

        /// <summary>
        /// eg: "classlib", "winforms"
        /// </summary>
        public string ProjectType
        {
            get { return GetValue<string>(nameof(ProjectType)); }
            set { 
                if(SetValue(nameof(ProjectType), value)) 
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        /// <summary>
        /// eg. "{scopeName}" or "{parentScopeName}/{scopeName}"
        /// </summary>
        public string SolutionSubFolder
        {
            get { return GetValue<string>(nameof(SolutionSubFolder)); }
            set
            {
                SetValue(nameof(SolutionSubFolder), value);
            }
        }

        /// <summary>
        /// Get the programming language of the project.
        /// underlying value is stored as the command line argument representation (for serializablity)
        /// </summary>
        public DotNetLanguage Language
        {
            get { return DotNetLanguages.GetByCommandLineArgument(GetValue<string>(nameof(Language))); }
            set
            {
                if (SetValue<string>(nameof(Language), value.DotNetCommandLineArgument)) { 
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
                    RaisePropertyChangedEvent(nameof(ProjectFileName));
                    _treeNodeIcon = new ResourceManagerTreeNodeIcon($"{value.ImageKey}_project");
                }
            }
        }

        public TargetFramework TargetFramework
        {
            get { return TargetFrameworks.AllFrameworks.Single(f => f.Id ==GetValue<string>(nameof(TargetFramework))); }
            set { 
                if(SetValue(nameof(TargetFramework), value.Id))
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        public List<NuGetPackage> NuGetPackages
        {
            get { return GetValue<List<NuGetPackage>>(nameof(NuGetPackages)); }
            private set { SetValue(nameof(NuGetPackages), value); }
        }

        public List<DotNetProjectReference> ProjectReferences
        {
            get { return GetValue<List<DotNetProjectReference>>(nameof(ProjectReferences)); }
            private set { SetValue(nameof(ProjectReferences), value); }
        }

        public override string TreeNodeText { get { return $"{Name} ({Language.DotNetCommandLineArgument}, {ProjectType}, {TargetFramework.DotNetCommandLineArgument})"; } }

        private ITreeNodeIcon _treeNodeIcon;
        public override ITreeNodeIcon TreeNodeIcon { get { return _treeNodeIcon; } }

        

        /// <summary>
        /// Get the resulting folderpath by traversing ancestors to find the FolderArtifactDecorator
        /// </summary>
        public string GetFolderPath()
        {
            var folderArtifact = FindAncestorArtifact<FolderArtifactDecorator>() as FolderArtifact;
            if(folderArtifact==null) throw new InvalidOperationException("Cannot determine relative folder path, no FolderArtifactDecorator ancestor found.");
            return folderArtifact.FullPath;
        }

        public void AddNuGetPackage(NuGetPackage syncfusion_Edit_Windows)
        {
            var existingPackage = NuGetPackages.FirstOrDefault(p => p.PackageId.Equals(syncfusion_Edit_Windows.PackageId, StringComparison.OrdinalIgnoreCase));
            if(existingPackage != null)
            {
                // check if the version is higher than existing
                var isHigherVersion = syncfusion_Edit_Windows.IsHigherVersionThan(existingPackage);
                if (isHigherVersion)
                {
                    existingPackage.Version = syncfusion_Edit_Windows.Version;
                }
            }
            else
            {
                NuGetPackages.Add(syncfusion_Edit_Windows);
            }
        }
    }
}
