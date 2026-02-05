using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains
{
    /// <summary>
    /// Container artifact for entity views (EditView, ListView, SelectView)
    /// </summary>
    public class EntityViewsContainerArtifact : WorkspaceArtifactBase
    {
        public EntityViewsContainerArtifact()
            : base()
        {
        }

        public EntityViewsContainerArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => "Views";

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("layout-dashboard");

        /// <summary>
        /// Get all edit views in this container
        /// </summary>
        public IEnumerable<EntityEditViewArtifact> GetEditViews() =>
            Children.OfType<EntityEditViewArtifact>();

        /// <summary>
        /// Get the list view (there should be only one per entity)
        /// </summary>
        public EntityListViewArtifact? GetListView() =>
            Children.OfType<EntityListViewArtifact>().FirstOrDefault();

        /// <summary>
        /// Get the select view (there should be only one per entity)
        /// </summary>
        public EntitySelectViewArtifact? GetSelectView() =>
            Children.OfType<EntitySelectViewArtifact>().FirstOrDefault();

        /// <summary>
        /// Add an edit view to this container
        /// </summary>
        public void AddEditView(EntityEditViewArtifact editView)
        {
            AddChild(editView);
        }

        /// <summary>
        /// Remove an edit view from this container
        /// </summary>
        public void RemoveEditView(EntityEditViewArtifact editView)
        {
            RemoveChild(editView);
        }

        /// <summary>
        /// Set the list view for this container
        /// </summary>
        public void SetListView(EntityListViewArtifact listView)
        {
            // Remove existing list view if any
            var existing = GetListView();
            if (existing != null)
            {
                RemoveChild(existing);
            }
            AddChild(listView);
        }

        /// <summary>
        /// Set the select view for this container
        /// </summary>
        public void SetSelectView(EntitySelectViewArtifact selectView)
        {
            // Remove existing select view if any
            var existing = GetSelectView();
            if (existing != null)
            {
                RemoveChild(existing);
            }
            AddChild(selectView);
        }
    }
}
