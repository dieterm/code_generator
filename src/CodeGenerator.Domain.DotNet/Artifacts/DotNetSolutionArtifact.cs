using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.DotNet.ProjectType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeGenerator.Domain.DotNet
{
    public class DotNetSolutionArtifact : Artifact
    {
        private List<DotNetProjectReference> _projectReferences = new List<DotNetProjectReference>();
        private ITreeNodeIcon _treeNodeIcon;

        public event EventHandler? AllProjectReferencesGenerated;

        public DotNetSolutionArtifact(string solutionName, DotNetSolutionType solutionType)
        {
            Name = solutionName;
            SolutionType = solutionType;
            Solution = new DotNetSolution(solutionName);
            _treeNodeIcon = new ResourceManagerTreeNodeIcon($"dotnet_solution");
        }

        public override string TreeNodeText => SolutionFileName;

       
        public override ITreeNodeIcon TreeNodeIcon { get { return _treeNodeIcon; } }

        /// <summary>
        /// Returns the project file name, e.g. 'MyProject.csproj'
        /// </summary>
        public string SolutionFileName { get { return $"{Name}.{SolutionType.ToString()}"; } }

        /// <summary>
        /// Returns the full project file path in the workspace folder, e.g. 'C:\Projects\MyWorkspace\MyProject\MyProject.csproj'
        /// </summary>
        public string? GetProjectFilePath()
        {
            var folderPath = GetFolderPath();
            if (folderPath == null) return null;
            return System.IO.Path.Combine(folderPath, SolutionFileName);
        }
        /// <summary>
        /// Get the resulting folderpath by traversing ancestors to find the FolderArtifactDecorator
        /// </summary>
        public string? GetFolderPath()
        {
            return this.GetFullPath();
        }
        public string Name
        {
            get { return GetValue<string>(nameof(Name))??string.Empty; }
            set
            {
                if (SetValue(nameof(Name), value))
                {
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
                    RaisePropertyChangedEvent(nameof(SolutionFileName));
                }
            }
        }
        public DotNetSolution Solution { get; }
        public DotNetSolutionType SolutionType
        {
            get { return GetValue<DotNetSolutionType>(nameof(SolutionType)); }
            set
            {
                if (SetValue(nameof(SolutionType), value))
                {
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
                    RaisePropertyChangedEvent(nameof(SolutionFileName));
                }
            }
        }

        

        public IReadOnlyCollection<DotNetProjectReference> ProjectReferences
        {
            get { return _projectReferences.AsReadOnly(); }
        }
        private readonly Dictionary<DotNetProjectArtifact, bool> _projectReferenceGenerationStatus = new Dictionary<DotNetProjectArtifact, bool>();
        public void AddProjectReference(DotNetProjectArtifact dotNetProjectArtifact)
        {
            if (!_projectReferences.Exists(pr => pr.ProjectArtifact==dotNetProjectArtifact))
            {
                _projectReferences.Add(new DotNetProjectReference(dotNetProjectArtifact));
                _projectReferenceGenerationStatus.Add(dotNetProjectArtifact, false);
                dotNetProjectArtifact.Generated += ReferenceProjectGenerated;
                RaisePropertyChangedEvent(nameof(ProjectReferences));
            }
        }
        public bool AreAllProjectReferencesGenerated
        {
            get { 
                return _projectReferenceGenerationStatus.Values.All(generated => generated);
            }
        }
        private void ReferenceProjectGenerated(object? sender, EventArgs e)
        {
            if (sender is DotNetProjectArtifact dotNetProjectArtifact && _projectReferenceGenerationStatus.ContainsKey(dotNetProjectArtifact))
            {
                _projectReferenceGenerationStatus[dotNetProjectArtifact] = true;

                // Check if all project references have been generated
                if (AreAllProjectReferencesGenerated)
                {
                    OnAllProjectReferencesGenerated();
                }
            }
        }

        private void OnAllProjectReferencesGenerated()
        {
            AllProjectReferencesGenerated?.Invoke(this, EventArgs.Empty);
        }
    }
}
