using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains
{
    /// <summary>
    /// Represents an entity within a domain
    /// </summary>
    public class EntityArtifact : Artifact, IEditableTreeNode
    {
        public EntityArtifact(string name)
            : base()
        {
            Name = name;

            EnsureEntityStatesContainerExists();
            EnsureEntityRelationsContainerExists();
        }

        public EntityArtifact(ArtifactState state)
            : base(state)
        {
            EnsureEntityStatesContainerExists();
            EnsureEntityRelationsContainerExists();
        }


        public override string TreeNodeText => Name;

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("club");

        /// <summary>
        /// Entity name
        /// </summary>
        public string Name
        {
            get => GetValue<string>(nameof(Name));
            set
            {
                if (SetValue(nameof(Name), value))
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        /// <summary>
        /// Entity description for documentation purposes
        /// </summary>
        public string? Description
        {
            get => GetValue<string?>(nameof(Description));
            set => SetValue(nameof(Description), value);
        }

        /// <summary>
        /// Gets the EntityStatesContainerArtifact for this entity
        /// </summary>
        public EntityStatesContainerArtifact States => EnsureEntityStatesContainerExists();

        private EntityStatesContainerArtifact EnsureEntityStatesContainerExists()
        {
            var existing = Children.OfType<EntityStatesContainerArtifact>().FirstOrDefault();
            if (existing == null)
            {
                existing = new EntityStatesContainerArtifact();
                // Automatically add EntitiesContainerArtifact
                AddChild(existing);
            }
            return existing;
        }

        /// <summary>
        /// Gets the EntityRelationsContainerArtifact for this entity
        /// </summary>
        public EntityRelationsContainerArtifact Relations => EnsureEntityRelationsContainerExists();

        private EntityRelationsContainerArtifact EnsureEntityRelationsContainerExists()
        {
            var existing = Children.OfType<EntityRelationsContainerArtifact>().FirstOrDefault();
            if (existing == null)
            {
                existing = new EntityRelationsContainerArtifact();
                AddChild(existing);
            }
            return existing;
        }

        public IEnumerable<EntityRelationArtifact> ReverseRelations 
        { 
            get 
            {
                return FindAncesterOfType<DomainArtifact>()?
                    .Entities
                    .GetEntities()
                    .SelectMany(e => e.GetRelations()
                        .Where(r => r.TargetEntityId == this.Id)) 
                    ?? Enumerable.Empty<EntityRelationArtifact>(); 
            } 
        }
            

        /// <summary>
        /// Get all states for this entity
        /// </summary>
        public IEnumerable<EntityStateArtifact> GetStates() =>
            States.GetStates();

        /// <summary>
        /// Get all relations for this entity
        /// </summary>
        public IEnumerable<EntityRelationArtifact> GetRelations() =>
            Relations.GetRelations();

        public bool CanBeginEdit() => true;

        public bool Validating(string newName) => !string.IsNullOrWhiteSpace(newName);

        public void EndEdit(string oldName, string newName)
        {
            Name = newName;
        }
        public EntityStateArtifact AddEntityState(string name)
        {
            var state = new EntityStateArtifact(name);
            States.AddState(state);
            return state;
        }

        public EntityRelationArtifact AddEntityRelation(string name)
        {
            var relation = new EntityRelationArtifact(name);
            Relations.AddRelation(relation);
            return relation;
        }
    }
}
