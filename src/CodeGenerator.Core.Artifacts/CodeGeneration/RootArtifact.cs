using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Artifacts.TreeNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.CodeGeneration
{
    /// <summary>
    /// This artifact represents the root of an artifact tree.
    /// </summary>
    public sealed class RootArtifact : Artifact
    {
        public const string ROOT_ARTIFACT_ID = "ROOT_ARTIFACT";
        private readonly string _treeNodeText = "Output";
        public override string TreeNodeText { get { return _treeNodeText; } }

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("box");

        public RootArtifact(ArtifactState state) : base(state) { }
        public RootArtifact(string treeNodeText, string outputDirectory)
        {
            _treeNodeText = treeNodeText;
            Id = ROOT_ARTIFACT_ID;
            AddDecorator(new ExistingFolderArtifactDecorator(ExistingFolderArtifact.EXISTING_FOLDER_PROPERTIES_DECORATOR_KEY));
            FolderPath = outputDirectory;
        }

        public string FolderPath
        {
            get { return GetDecorator<ExistingFolderArtifactDecorator>()!.FolderPath; }
            set { GetDecorator<ExistingFolderArtifactDecorator>()!.FolderPath = value; }
        }
    }
}
