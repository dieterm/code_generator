using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    /// <summary>
    /// Base class for all datasource artifacts
    /// </summary>
    public abstract class DatasourceArtifact : Artifact, IEditableTreeNode
    {
        //private string _name;
        //private string _description;

        protected DatasourceArtifact(string name)
        {
            //Id = $"datasource_{Name.ToLowerInvariant().Replace(" ", "_")}_{GetType().Name}";
            Name = name;
            Description = string.Empty;
        }

        protected DatasourceArtifact(ArtifactState state)
            : base(state)
        {
            //Id = $"datasource_{Name.ToLowerInvariant().Replace(" ", "_")}_{GetType().Name}";
        }

        //public override string Id => 

       /* private void UpdateId()
        {
            Id = $"datasource_{Name.ToLowerInvariant().Replace(" ", "_")}_{GetType().Name}";
        }*/

        public override string TreeNodeText { get { return Name; } }

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon(IconKey);

        /// <summary>
        /// Icon key for the tree node (override in derived classes)
        /// </summary>
        protected virtual string IconKey => "database";

        /// <summary>
        /// Display name of the datasource
        /// </summary>
        public string Name
        {
            get { return GetValue<string>(nameof(Name)); }
            set { 
                SetValue<string>(nameof(Name), value); 
                RaisePropertyChangedEvent(nameof(TreeNodeText)); 
            }
        }

        /// <summary>
        /// Description of the datasource
        /// </summary>
        public string Description
        {
            get { return GetValue<string>(nameof(Description)); }
            set { 
                SetValue<string>(nameof(Description), value); 
            }
        }

        /// <summary>
        /// Type identifier for the datasource (e.g., "SqlServer", "MySql", "Json", etc.)
        /// </summary>
        public abstract string DatasourceType { get; }

        /// <summary>
        /// Category of the datasource (e.g., "Relational Database", "Non-Relational Database", "File")
        /// </summary>
        public abstract string DatasourceCategory { get; }

        /// <summary>
        /// Check if the datasource connection/file is valid
        /// </summary>
        public abstract Task<bool> ValidateAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Refresh the schema/structure from the datasource
        /// </summary>
        public abstract Task RefreshSchemaAsync(CancellationToken cancellationToken = default);
    }
}
