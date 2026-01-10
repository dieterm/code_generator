using CodeGenerator.Core.Artifacts.TreeNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Artifacts.CodeGeneration;
namespace CodeGenerator.Core.Artifacts.FileSystem
{
    public class FolderArtifact : Artifact
    {
        public const string FOLDER_PROPERTIES_DECORATOR_KEY = "FolderProperties";
        public FolderArtifact(string folderName)
        {
            if(string.IsNullOrWhiteSpace(folderName)) throw new ArgumentNullException(nameof(folderName));
            AddOrGetDecorator(() => new FolderArtifactDecorator(FolderArtifact.FOLDER_PROPERTIES_DECORATOR_KEY));
            FolderName = folderName;
        }

        override public string TreeNodeText
        {
            get { return FolderName; }
        }
        override public ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("folder");
        public string FolderName {
            get { return GetDecorator<FolderArtifactDecorator>().FolderName; }
            set { GetDecorator<FolderArtifactDecorator>().FolderName = value; }
        }
        public string FullPath
        {
            get
            {
                return this.GetFullPath();
                //if (Parent is RootArtifact rootArtifact) 
                //{ 
                //    return Path.Combine(rootArtifact.FolderPath, FolderName);
                //}
                //var parentFolder = this.GetParentFolderArtifact();
                //if (parentFolder == null) 
                //    return FolderName;

                //return $"{parentFolder.FullPath}{System.IO.Path.DirectorySeparatorChar}{FolderName}";
            }
        }
        public override string Id => FullPath;

    }
}
