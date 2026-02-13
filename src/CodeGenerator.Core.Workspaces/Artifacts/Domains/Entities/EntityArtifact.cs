using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Events;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities
{
    /// <summary>
    /// Represents an entity within a domain
    /// </summary>
    public class EntityArtifact : WorkspaceArtifactBase, IEditableTreeNode
    {
        public EntityArtifact(string name)
            : base()
        {
            Name = name;
            EnsurePropertiesContainerExists();
            EnsureEntityStatesContainerExists();
            EnsureEntityRelationsContainerExists();
            EnsureEntityViewsContainerExists();

            EntityStates.ChildRemoved += States_ChildRemoved;
        }

        

        public EntityArtifact(ArtifactState state)
            : base(state)
        {
            EnsurePropertiesContainerExists();
            EnsureEntityStatesContainerExists();
            EnsureEntityRelationsContainerExists();
            EnsureEntityViewsContainerExists();

            EntityStates.ChildRemoved += States_ChildRemoved;
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
        /// Indicates if this entity is an aggregate root in DDD
        /// </summary>
        public bool IsAggregateRoot
        {
            get => GetValue<bool>(nameof(IsAggregateRoot));
            set => SetValue(nameof(IsAggregateRoot), value);
        }

        /// <summary>
        /// Default state ID for this entity, used to persist in memento-state
        /// </summary>
        public string? DefaultStateId
        {
            get => GetValue<string?>(nameof(DefaultStateId));
            set => SetValue(nameof(DefaultStateId), value);
        }

        /// <summary>
        /// Gets the default state for this entity
        /// </summary>
        public EntityStateArtifact? DefaultState
        {
            get => GetStates().FirstOrDefault(s => s.Id == DefaultStateId);
        }

        public PropertiesContainerArtifact EntityProperties => EnsurePropertiesContainerExists();

        private PropertiesContainerArtifact EnsurePropertiesContainerExists()
        {
            var existing = Children.OfType<PropertiesContainerArtifact>().FirstOrDefault();
            if (existing == null)
            {
                existing = new PropertiesContainerArtifact();
                AddChild(existing);
            }
            return existing;
        }

        /// <summary>
        /// Gets the EntityStatesContainerArtifact for this entity
        /// </summary>
        public EntityStatesContainerArtifact EntityStates => EnsureEntityStatesContainerExists();

        private EntityStatesContainerArtifact EnsureEntityStatesContainerExists()
        {
            var existing = Children.OfType<EntityStatesContainerArtifact>().FirstOrDefault();
            if (existing == null)
            {
                existing = new EntityStatesContainerArtifact();
                AddChild(existing);
            }
            return existing;
        }

        /// <summary>
        /// Gets the EntityRelationsContainerArtifact for this entity
        /// </summary>
        public EntityRelationsContainerArtifact EntityRelations => EnsureEntityRelationsContainerExists();

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

        /// <summary>
        /// Gets the EntityViewsContainerArtifact for this entity
        /// </summary>
        public EntityViewsContainerArtifact EntityViews => EnsureEntityViewsContainerExists();

        private EntityViewsContainerArtifact EnsureEntityViewsContainerExists()
        {
            var existing = Children.OfType<EntityViewsContainerArtifact>().FirstOrDefault();
            if (existing == null)
            {
                existing = new EntityViewsContainerArtifact();
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
                        .Where(r => r.TargetEntityId == Id)) 
                    ?? Enumerable.Empty<EntityRelationArtifact>(); 
            } 
        }

        private void States_ChildRemoved(object? sender, ChildRemovedEventArgs e)
        {
            if(e.ChildArtifact is EntityStateArtifact removedState)
            {
                // If the removed state was the default state, clear the default state
                if (DefaultStateId == removedState.Id)
                {
                    DefaultStateId = null;
                }
            }
        }

        /// <summary>
        /// Get all states for this entity
        /// </summary>
        public IEnumerable<EntityStateArtifact> GetStates() =>
            EntityStates.GetStates();

        /// <summary>
        /// Get all relations for this entity
        /// </summary>
        public IEnumerable<EntityRelationArtifact> GetRelations() =>
            EntityRelations.GetRelations();

        public bool CanBeginEdit() => true;

        public bool Validating(string newName) => !string.IsNullOrWhiteSpace(newName);

        public void EndEdit(string oldName, string newName)
        {
            Name = newName;
        }

        public EntityStateArtifact AddEntityState(string name)
        {
            var state = new EntityStateArtifact(name);
            EntityStates.AddState(state);
            return state;
        }

        public EntityRelationArtifact AddEntityRelation(string name)
        {
            var relation = new EntityRelationArtifact(name);
            EntityRelations.AddRelation(relation);
            return relation;
        }

        public PropertyArtifact AddProperty(PropertyArtifact createdProperty)
        {
            EntityProperties.AddProperty(createdProperty);
            return createdProperty;
        }
    }
}
