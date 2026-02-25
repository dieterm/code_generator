using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.ViewModels.Common;
using CodeGenerator.UserControls.ViewModels;

namespace CodeGenerator.Core.Workspaces.ViewModels.Datasources
{
    public class TableArtifactEditViewModel : ArtifactEditViewModel<TableArtifact, TableGeneralTabViewModel>
    {
        public TableArtifactEditViewModel()
            : base("Table", new TableGeneralTabViewModel())
        {
        }

        /// <summary>
        /// The data extraction field model containing LoadData, PropertiesDistinctValues, and Create commands.
        /// </summary>
        public TableDataExtractionFieldModel DataExtractionField => GeneralTab.DataExtractionField;

        public event EventHandler? RequestLoadData
        {
            add => DataExtractionField.RequestLoadData += value;
            remove => DataExtractionField.RequestLoadData -= value;
        }

        public event EventHandler<CreateFromSelectionEventArgs>? RequestCreateEntities
        {
            add => DataExtractionField.RequestCreateEntities += value;
            remove => DataExtractionField.RequestCreateEntities -= value;
        }

        public event EventHandler<CreateFromSelectionEventArgs>? RequestCreateValueTypes
        {
            add => DataExtractionField.RequestCreateValueTypes += value;
            remove => DataExtractionField.RequestCreateValueTypes -= value;
        }
    }

    public class TableGeneralTabViewModel : ArtifactEditViewTabModel<TableArtifact>
    {
        public SingleLineTextFieldModel NameField { get; }
        public SingleLineTextFieldModel SchemaField { get; }
        public TableDataExtractionFieldModel DataExtractionField { get; }

        public TableGeneralTabViewModel() : base("General")
        {
            NameField = new SingleLineTextFieldModel
            {
                Label = "Table Name",
                Name = nameof(TableArtifact.Name),
                Tooltip = "Name of the table",
                IsRequired = true,
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(NameField);

            SchemaField = new SingleLineTextFieldModel
            {
                Label = "Schema",
                Name = nameof(TableArtifact.Schema),
                Tooltip = "Schema of the table",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(SchemaField);

            DataExtractionField = new TableDataExtractionFieldModel() 
            {
                // make sure the data extraction field takes up the remaining space in the UI
                Dock = Shared.Views.FieldDockStyle.Fill
            };
            FieldCollection.FieldModels.Add(DataExtractionField);
        }

        public override void BindArtifact(WorkspaceArtifactBase? artifactBase)
        {
            base.BindArtifact(artifactBase);

            if (Artifact != null)
            {
                DataExtractionField.Table = Artifact;
            }
        }

    }
}
