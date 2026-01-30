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
        public DotNetProjectArtifact(string name, DotNetLanguage language, string projectType, string targetFramework)
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

        //public override string Id => $"DotNetProject:{Name}";
        public string ProjectFileName { get { return $"{Name}.{Language.ProjectFileExtension}"; } }

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

        public string TargetFramework
        {
            get { return GetValue<string>(nameof(TargetFramework)); }
            set { 
                if(SetValue(nameof(TargetFramework), value))
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

        public override string TreeNodeText { get { return $"{Name} ({Language.DotNetCommandLineArgument}, {ProjectType}, {TargetFramework})"; } }

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
