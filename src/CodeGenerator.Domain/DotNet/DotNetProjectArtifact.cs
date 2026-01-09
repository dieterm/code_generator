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
            Name = name;
            Language = language;
            ProjectType = projectType;
            TargetFramework = targetFramework;
            TreeNodeIcon = new ResourceManagerTreeNodeIcon($"{language.ImageKey}_project");
            NuGetPackages = new List<NuGetPackage>();
            ProjectReferences = new List<DotNetProjectReference>();
        }
        public override string Id => $"DotNetProject:{Name}";
        public string ProjectFileName { get { return $"{Name}.{Language.ProjectFileExtension}"; } }

        public string Name {
            get { return GetProperty<string>(nameof(Name)); }
            set { SetProperty(nameof(Name), value); }
        }

        /// <summary>
        /// eg: "classlib", "winforms"
        /// </summary>
        public string ProjectType
        {
            get { return GetProperty<string>(nameof(ProjectType)); }
            set { SetProperty(nameof(ProjectType), value); }
        }

        public DotNetLanguage Language
        {
            get { return GetProperty<DotNetLanguage>(nameof(Language)); }
            set { SetProperty(nameof(Language), value); }
        }

        public string TargetFramework
        {
            get { return GetProperty<string>(nameof(TargetFramework)); }
            set { SetProperty(nameof(TargetFramework), value); }
        }

        public List<NuGetPackage> NuGetPackages
        {
            get { return GetProperty<List<NuGetPackage>>(nameof(NuGetPackages)); }
            private set { SetProperty(nameof(NuGetPackages), value); }
        }

        public List<DotNetProjectReference> ProjectReferences
        {
            get { return GetProperty<List<DotNetProjectReference>>(nameof(ProjectReferences)); }
            private set { SetProperty(nameof(ProjectReferences), value); }
        }

        public override string TreeNodeText { get { return $"{Name} ({Language.DotNetCommandLineArgument}, {ProjectType}, {TargetFramework})"; } }

        public override ITreeNodeIcon TreeNodeIcon { get; }

        /// <summary>
        /// Get the resulting folderpath by traversing ancestors to find the FolderArtifactDecorator
        /// </summary>
        public string GetFolderPath()
        {
            var folderArtifact = FindAncestorArtifact<FolderArtifactDecorator>() as FolderArtifact;
            if(folderArtifact==null) throw new InvalidOperationException("Cannot determine relative folder path, no FolderArtifactDecorator ancestor found.");
            return folderArtifact.FullPath;
        }
    }
}
