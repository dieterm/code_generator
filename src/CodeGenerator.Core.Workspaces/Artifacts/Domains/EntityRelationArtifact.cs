using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains
{

    /// <summary>
    /// Represents a relation between entities
    /// </summary>
    public class EntityRelationArtifact : WorkspaceArtifactBase, IEditableTreeNode
    {
        public EntityRelationArtifact(string name)
            : base()
        {
            Name = name;
            SourceCardinality = RelationCardinality.One;
            TargetCardinality = RelationCardinality.ZeroOrMany;
        }

        public EntityRelationArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => Name;

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("arrow-right-left");

        /// <summary>
        /// Relation name
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
        /// The Id of the target entity
        /// </summary>
        public string? TargetEntityId
        {
            get => GetValue<string?>(nameof(TargetEntityId));
            set => SetValue(nameof(TargetEntityId), value);
        }

        /// <summary>
        /// Helper property to get the target entity
        /// </summary>
        public EntityArtifact? TargetEntity
        {
            get
            {
                if (string.IsNullOrEmpty(TargetEntityId)) return null;
                return FindAncesterOfType<DomainArtifact>()?.Entities.GetEntities()
                    .FirstOrDefault(e => e.Id == TargetEntityId);
            }
        }

        /// <summary>
        /// The cardinality on the source side (this entity)
        /// </summary>
        public RelationCardinality SourceCardinality
        {
            get => GetValue<RelationCardinality>(nameof(SourceCardinality));
            set => SetValue(nameof(SourceCardinality), value);
        }

        /// <summary>
        /// The cardinality on the target side (related entity)
        /// </summary>
        public RelationCardinality TargetCardinality
        {
            get => GetValue<RelationCardinality>(nameof(TargetCardinality));
            set => SetValue(nameof(TargetCardinality), value);
        }

        /// <summary>
        /// The property name on the source entity that represents this relation
        /// </summary>
        public string? SourcePropertyName
        {
            get => GetValue<string?>(nameof(SourcePropertyName));
            set => SetValue(nameof(SourcePropertyName), value);
        }

        /// <summary>
        /// The property name on the target entity that represents the inverse relation
        /// </summary>
        public string? TargetPropertyName
        {
            get => GetValue<string?>(nameof(TargetPropertyName));
            set => SetValue(nameof(TargetPropertyName), value);
        }

        /// <summary>
        /// Gets the cardinality as a display string (e.g., "1 - 0..*")
        /// </summary>
        public string CardinalityDisplay =>
            $"{CardinalityToString(SourceCardinality)} - {CardinalityToString(TargetCardinality)}";

        private static string CardinalityToString(RelationCardinality cardinality)
        {
            return cardinality switch
            {
                RelationCardinality.One => "1",
                RelationCardinality.ZeroOrOne => "0..1",
                RelationCardinality.ZeroOrMany => "0..*",
                RelationCardinality.OneOrMany => "1..*",
                _ => "?"
            };
        }

        public bool CanBeginEdit() => true;

        public bool Validating(string newName) => !string.IsNullOrWhiteSpace(newName);

        public void EndEdit(string oldName, string newName)
        {
            Name = newName;
        }
    }
}
