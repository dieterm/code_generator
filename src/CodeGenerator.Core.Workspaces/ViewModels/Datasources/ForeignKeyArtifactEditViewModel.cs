using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.ViewModels.Common;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.Workspaces.ViewModels.Datasources
{
    public class ForeignKeyArtifactEditViewModel : ArtifactEditViewModel<ForeignKeyArtifact, ForeignKeyGeneralTabViewModel>
    {
        public ForeignKeyArtifactEditViewModel()
            : base("Foreign Key", new ForeignKeyGeneralTabViewModel())
        {
            ColumnMappingField.MappingsChanged += (s, e) => OnValueChanged(s, e);
        }

        /// <summary>
        /// The column mapping field model for managing foreign key column mappings.
        /// </summary>
        public ForeignKeyColumnMappingFieldModel ColumnMappingField => GeneralTab.ColumnMappingField;
    }

    public class ForeignKeyGeneralTabViewModel : ArtifactEditViewTabModel<ForeignKeyArtifact>
    {
        private DatasourceArtifact? _datasource;
        private bool _isBinding;

        public SingleLineTextFieldModel NameField { get; }
        public ComboboxFieldModel ReferencedTableField { get; }
        public ComboboxFieldModel OnDeleteActionField { get; }
        public ComboboxFieldModel OnUpdateActionField { get; }
        public ForeignKeyColumnMappingFieldModel ColumnMappingField { get; }

        public ForeignKeyGeneralTabViewModel() : base("General")
        {
            NameField = new SingleLineTextFieldModel
            {
                Label = "Foreign Key Name",
                Name = nameof(ForeignKeyArtifact.Name),
                Tooltip = "Name of the foreign key constraint",
                IsRequired = true,
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(NameField);

            ReferencedTableField = new ComboboxFieldModel
            {
                Label = "Referenced Table",
                Name = "ReferencedTable",
                Tooltip = "The table this foreign key references",
                AutoBind = false,
                AutoUpdate = false
            };
            FieldCollection.FieldModels.Add(ReferencedTableField);

            OnDeleteActionField = new ComboboxFieldModel
            {
                Label = "On Delete",
                Name = "OnDeleteAction",
                Tooltip = "Action to take when a referenced row is deleted",
                AutoBind = false,
                AutoUpdate = false
            };
            InitializeForeignKeyActionItems(OnDeleteActionField, "delete");
            FieldCollection.FieldModels.Add(OnDeleteActionField);

            OnUpdateActionField = new ComboboxFieldModel
            {
                Label = "On Update",
                Name = "OnUpdateAction",
                Tooltip = "Action to take when a referenced row is updated",
                AutoBind = false,
                AutoUpdate = false
            };
            InitializeForeignKeyActionItems(OnUpdateActionField, "update");
            FieldCollection.FieldModels.Add(OnUpdateActionField);

            ColumnMappingField = new ForeignKeyColumnMappingFieldModel { Dock = Shared.Views.FieldDockStyle.Fill };
            FieldCollection.FieldModels.Add(ColumnMappingField);

            ReferencedTableField.PropertyChanged += OnReferencedTableChanged;
            OnDeleteActionField.PropertyChanged += OnActionFieldChanged;
            OnUpdateActionField.PropertyChanged += OnActionFieldChanged;
        }

        private static void InitializeForeignKeyActionItems(ComboboxFieldModel field, string verb)
        {
            field.Items.Add(new ComboboxItem { DisplayName = "No Action", Value = ForeignKeyAction.NoAction, Tooltip = "No action specified (database default behavior)" });
            field.Items.Add(new ComboboxItem { DisplayName = "Cascade", Value = ForeignKeyAction.Cascade, Tooltip = $"Automatically {verb} rows in the child table" });
            field.Items.Add(new ComboboxItem { DisplayName = "Set Null", Value = ForeignKeyAction.SetNull, Tooltip = "Set the foreign key column to NULL" });
            field.Items.Add(new ComboboxItem { DisplayName = "Restrict", Value = ForeignKeyAction.Restrict, Tooltip = $"Prevent the {verb} if there are referencing rows" });
        }

        public override void BindArtifact(WorkspaceArtifactBase? artifactBase)
        {
            _isBinding = true;
            try
            {
                base.BindArtifact(artifactBase);

                if (Artifact != null)
                {
                    _datasource = Artifact.FindAncesterOfType<DatasourceArtifact>();
                    ColumnMappingField.ForeignKey = Artifact;

                    LoadAvailableTables();
                    LoadSelections();
                }
            }
            finally
            {
                _isBinding = false;
            }
        }

        private void LoadAvailableTables()
        {
            ReferencedTableField.Items.Clear();

            if (_datasource == null) return;

            var parentTable = Artifact?.Parent as TableArtifact;
            var tables = _datasource.GetAllDescendants()
                .OfType<TableArtifact>()
                .Where(t => t.Id != parentTable?.Id)
                .OrderBy(t => t.Name);

            foreach (var table in tables)
            {
                ReferencedTableField.Items.Add(new ComboboxItem { DisplayName = table.Name, Value = table.Id });
            }
        }

        private void LoadSelections()
        {
            if (Artifact == null) return;

            var selectedTable = ReferencedTableField.Items
                .FirstOrDefault(t => t.Value?.ToString() == Artifact.ReferencedTableId);
            ReferencedTableField.SelectedItem = selectedTable;

            var selectedOnDelete = OnDeleteActionField.Items
                .FirstOrDefault(i => i.Value is ForeignKeyAction action && action == Artifact.OnDeleteAction);
            OnDeleteActionField.SelectedItem = selectedOnDelete;

            var selectedOnUpdate = OnUpdateActionField.Items
                .FirstOrDefault(i => i.Value is ForeignKeyAction action && action == Artifact.OnUpdateAction);
            OnUpdateActionField.SelectedItem = selectedOnUpdate;

            ColumnMappingField.LoadColumnsAndMappings();
        }

        private void OnReferencedTableChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isBinding || Artifact == null) return;

            if (e.PropertyName == nameof(ComboboxFieldModel.SelectedItem))
            {
                var selectedTableId = ReferencedTableField.SelectedItem?.Value?.ToString();
                Artifact.ReferencedTableId = selectedTableId;

                Artifact.ColumnMappings = new List<ForeignKeyColumnMapping>();
                ColumnMappingField.LoadColumnsAndMappings();
            }
        }

        private void OnActionFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isBinding || Artifact == null) return;

            if (e.PropertyName == nameof(ComboboxFieldModel.SelectedItem) && sender is ComboboxFieldModel field)
            {
                if (field == OnDeleteActionField && OnDeleteActionField.SelectedItem?.Value is ForeignKeyAction deleteAction)
                {
                    Artifact.OnDeleteAction = deleteAction;
                }
                else if (field == OnUpdateActionField && OnUpdateActionField.SelectedItem?.Value is ForeignKeyAction updateAction)
                {
                    Artifact.OnUpdateAction = updateAction;
                }
            }
        }

        protected override void OnArtifactPropertyChanged(ForeignKeyArtifact artifact, string propertyName)
        {
        }
    }
}
