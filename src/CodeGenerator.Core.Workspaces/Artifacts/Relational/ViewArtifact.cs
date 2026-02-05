using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts.Relational
{
    /// <summary>
    /// Represents a database view
    /// </summary>
    public class ViewArtifact : WorkspaceArtifactBase, IEditableTreeNode
    {
        public ViewArtifact(string name, string schema = TableArtifact.DEFAULT_SCHEMA)
        {
            Name = name;
            Schema = schema;
            Definition = string.Empty;
        }

        public ViewArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => string.IsNullOrEmpty(Schema) || Schema == TableArtifact.DEFAULT_SCHEMA
            ? Name 
            : $"{Schema}.{Name}";

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("eye");

        /// <summary>
        /// View name
        /// </summary>
        public string Name
        {
            get => GetValue<string>(nameof(Name));
            set { 
                if(SetValue(nameof(Name), value))
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        /// <summary>
        /// Schema name (e.g., "dbo")
        /// </summary>
        public string Schema
        {
            get => GetValue<string>(nameof(Schema));
            set { 
                if(SetValue(nameof(Schema), value))
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        /// <summary>
        /// SQL definition of the view
        /// </summary>
        public string Definition
        {
            get => GetValue<string>(nameof(Definition));
            set { 
                if(SetValue(nameof(Definition), value))
                    RaisePropertyChangedEvent(nameof(Definition));
            }
        }

        /// <summary>
        /// Get all columns
        /// </summary>
        public IEnumerable<ColumnArtifact> GetColumns() => 
            Children.OfType<ColumnArtifact>();

        /// <summary>
        /// Add a column to the view
        /// </summary>
        public ColumnArtifact AddColumn(string columnName, string dataType, bool isNullable = true)
        {
            var column = new ColumnArtifact(columnName, dataType, isNullable);
            AddChild(column);
            return column;
        }

        public bool CanBeginEdit()
        {
            return true;
        }

        public bool Validating(string newName)
        {
            return !string.IsNullOrWhiteSpace(newName);
        }

        public void EndEdit(string oldName, string newName)
        {
            Name = newName;
        }

        public IArtifactDecorator GetTemplateDatasourceProviderDecorator()
        {
            return GetDecoratorOfType<TemplateDatasourceProviderDecorator>()!;
        }
    }
}
