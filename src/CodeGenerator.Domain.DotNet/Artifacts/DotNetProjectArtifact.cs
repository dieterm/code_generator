using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Artifacts.TreeNode;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
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
        public const string RESOURCE_FILE_EXTENSION = ".resx";
        public DotNetProjectArtifact(string name, DotNetLanguage language, DotNetProjectType projectType, TargetFramework targetFramework)
        {
            //Id = $"DotNetProject:{Name}";
            Name = name;
            Language = language;
            ProjectType = projectType;
            TargetFramework = targetFramework;
            _treeNodeIcon = new ResourceManagerTreeNodeIcon($"{language.ImageKey}_project");
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
        public DotNetProjectType ProjectType
        {
            get { return GetValue<DotNetProjectType>(nameof(ProjectType)); }
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

        private readonly List<NuGetPackage> _nuGetPackages = new();
        public IReadOnlyCollection<NuGetPackage> NuGetPackages
        {
            get { return _nuGetPackages.AsReadOnly(); }
        }

        private readonly List<DotNetProjectReference> _projectReferences = new();

        public IReadOnlyCollection<DotNetProjectReference> ProjectReferences
        {
            get { return _projectReferences.AsReadOnly(); }
        }

        private readonly List<EmbeddedResource> _embeddedResources = new();
        public IReadOnlyCollection<EmbeddedResource> EmbeddedResources
        {
            get { return _embeddedResources.AsReadOnly(); }
        }

        public override string TreeNodeText { get { return $"{Name} ({Language.DotNetCommandLineArgument}, {ProjectType.Id}, {TargetFramework.DotNetCommandLineArgument})"; } }

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

        public void AddNuGetPackage(NuGetPackage nugetPackage)
        {
            var existingPackage = _nuGetPackages.FirstOrDefault(p => p.PackageId.Equals(nugetPackage.PackageId, StringComparison.OrdinalIgnoreCase));
            if(existingPackage != null)
            {
                // check if the version is higher than existing
                var isHigherVersion = nugetPackage.IsHigherVersionThan(existingPackage);
                if (isHigherVersion)
                {
                    existingPackage.Version = nugetPackage.Version;
                }
            }
            else
            {
                _nuGetPackages.Add(nugetPackage);
            }
        }

        public void AddProjectReference(DotNetProjectReference projectReference)
        {
            if(!_projectReferences.Any(pr => pr.ProjectArtifact == projectReference.ProjectArtifact))
            {
                _projectReferences.Add(projectReference);
            }
        }

        public void AddEmbeddedResource(EmbeddedResource embeddedResource)
        {
            var existingResource = _embeddedResources.FirstOrDefault(er => er.FileName == embeddedResource.FileName);
            if (existingResource==null)
            {
                _embeddedResources.Add(embeddedResource);
            } 
            else
            {
                existingResource.BuildAction = embeddedResource.BuildAction;
                existingResource.AccessModifier = embeddedResource.AccessModifier;
            }
        }

        public string? SaveProjectFile(string filePath, Microsoft.Extensions.Logging.ILogger? logger)
        {
            try
            {
                var pc = new ProjectCollection();
                var project = new Project(pc);
                // SDK-style via Xml.Sdk
                project.Xml.Sdk = "Microsoft.NET.Sdk";
                // Let ProjectType set properties
                ProjectType.SetPropertyItems(this, project);
                // Add PackageReferences
                foreach(var pkg in NuGetPackages)
                {
                    var item = project.AddItem("PackageReference", pkg.PackageId);
                    item[0].SetMetadataValue("Version", pkg.Version);
                }
                var projectFolder = this.GetFolderPath();
                // Add ProjectReferences
                foreach(var projRef in ProjectReferences)
                {
                    var referencedProjectFilePath = projRef.ProjectArtifact.GetProjectFilePath();
                    var relativePath = Path.GetRelativePath(projectFolder, referencedProjectFilePath); 
                    project.AddItem("ProjectReference", relativePath);
                }
                var root = project.Xml;
                var resourceItemGroup = root.AddItemGroup();
                var compileItemGroup = root.AddItemGroup();
                //var allResourceFiles = FindDescendants<FileArtifact>().Where(fa => fa.FileName.EndsWith(RESOURCE_FILE_EXTENSION));
                foreach (var embeddedResource in EmbeddedResources)
                {
                    var relativePath = embeddedResource.FileName;
                    // Determine the designer file name (e.g., "Resources\LucideIcons_#ffffff.Designer.cs")
                    var designerFileName = Path.ChangeExtension(relativePath, ".Designer.cs");
                    // Add EmbeddedResource Update item for the .resx file
                    var item = resourceItemGroup.AddItem(
                        itemType: "EmbeddedResource",
                        include: relativePath
                    );
                    item.Include = null;
                    item.Update = relativePath;

                    // Metadata toevoegen
                    item.AddMetadata("Generator", "PublicResXFileCodeGenerator", expressAsAttribute: false);
                    item.AddMetadata("LastGenOutput", designerFileName, expressAsAttribute: false);


                    var compileItem = compileItemGroup.AddItem(
                        itemType: "Compile",
                        include: designerFileName,
                        metadata: new[]
                        {
                            new KeyValuePair<string, string>("DependentUpon", Path.GetFileName(relativePath)),
                            new KeyValuePair<string, string>("DesignTime", "True"),
                            new KeyValuePair<string, string>("AutoGen", "True")
                        }.ToList()
                    );
                    compileItem.Include = null;
                    compileItem.Update = designerFileName;
                    //// Add Compile Update item for the .Designer.cs file
                    //var compileItem = project.AddItem("Compile", designerFileName, new[]
                    //{
                    //    new KeyValuePair<string, string>("Update", designerFileName)
                    //}.ToList());

                    //// Set metadata for the Compile item
                    //compileItem[0].SetMetadataValue("DesignTime", "True");
                    //compileItem[0].SetMetadataValue("AutoGen", "True");
                    //compileItem[0].SetMetadataValue("DependentUpon", Path.GetFileName(relativePath));
                }
                // Save
                project.Save(filePath);

                return null;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error saving .NET project file '{FilePath}'", filePath);
                return $"Error saving .NET project file '{filePath}': {ex.Message}";
            }
        }

    }
}
