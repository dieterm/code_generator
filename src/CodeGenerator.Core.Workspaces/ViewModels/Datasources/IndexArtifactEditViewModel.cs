using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.ViewModels.Common;
using CodeGenerator.UserControls.ViewModels;

namespace CodeGenerator.Core.Workspaces.ViewModels.Datasources
{
    public class IndexArtifactEditViewModel : ArtifactEditViewModel<IndexArtifact, IndexGeneralTabViewModel>
    {
        public IndexArtifactEditViewModel()
            : base("Index", new IndexGeneralTabViewModel())
        {
            ColumnSelectionField.ColumnsChanged += (s, e) => OnValueChanged(s, e);
        }

        /// <summary>
        /// The column selection field model for managing index columns.
        /// </summary>
        public IndexColumnSelectionFieldModel ColumnSelectionField => GeneralTab.ColumnSelectionField;
    }

    public class IndexGeneralTabViewModel : ArtifactEditViewTabModel<IndexArtifact>
    {
        public SingleLineTextFieldModel NameField { get; }
        public BooleanFieldModel IsUniqueField { get; }
        public BooleanFieldModel IsClusteredField { get; }
        public IndexColumnSelectionFieldModel ColumnSelectionField { get; }

        public IndexGeneralTabViewModel() : base("General")
        {
            NameField = new SingleLineTextFieldModel
            {
                Label = "Index Name",
                Name = nameof(IndexArtifact.Name),
                Tooltip = "Name of the index",
                IsRequired = true,
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(NameField);

            IsUniqueField = new BooleanFieldModel
            {
                Label = "Unique",
                Name = nameof(IndexArtifact.IsUnique),
                Tooltip = "Whether this is a unique index",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(IsUniqueField);

            IsClusteredField = new BooleanFieldModel
            {
                Label = "Clustered",
                Name = nameof(IndexArtifact.IsClustered),
                Tooltip = "Whether this is a clustered index",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(IsClusteredField);

            ColumnSelectionField = new IndexColumnSelectionFieldModel();
            FieldCollection.FieldModels.Add(ColumnSelectionField);
        }

        public override void BindArtifact(WorkspaceArtifactBase? artifactBase)
        {
            base.BindArtifact(artifactBase);

            if (Artifact != null)
            {
                ColumnSelectionField.Index = Artifact;
            }
        }

        protected override void OnArtifactPropertyChanged(IndexArtifact artifact, string propertyName)
        {
        }
    }
}
