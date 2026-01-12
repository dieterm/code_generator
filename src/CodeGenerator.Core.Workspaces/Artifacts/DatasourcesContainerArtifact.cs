using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    /// <summary>
    /// Container artifact for all datasources in a workspace
    /// </summary>
    public class DatasourcesContainerArtifact : Artifact
    {
        public DatasourcesContainerArtifact()
        {
            Id = "datasources_container";
        }

        public DatasourcesContainerArtifact(ArtifactState state)
            : base(state)
        {
            Id = "datasources_container";
        }
        //public override string Id => "datasources_container";

        public override string TreeNodeText => "Datasources";

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("database");

        /// <summary>
        /// Get all datasources
        /// </summary>
        public IEnumerable<DatasourceArtifact> GetDatasources() => 
            Children.OfType<DatasourceArtifact>();

        /// <summary>
        /// Add a datasource
        /// </summary>
        public void AddDatasource(DatasourceArtifact datasource)
        {
            AddChild(datasource);
        }

        /// <summary>
        /// Remove a datasource
        /// </summary>
        public void RemoveDatasource(DatasourceArtifact datasource)
        {
            RemoveChild(datasource);
        }
    }
}
