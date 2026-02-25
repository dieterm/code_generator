using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.Collections.ObjectModel;

namespace CodeGenerator.Core.Workspaces.ViewModels.Datasources
{
    /// <summary>
    /// FieldModel for the table data extraction functionality.
    /// Encapsulates LoadData command, PropertiesDistinctValues, and Create Entities/ValueTypes events.
    /// </summary>
    public class TableDataExtractionFieldModel : FieldViewModelBase
    {
        public TableDataExtractionFieldModel()
        {
            Label = "Data Extraction";
            Name = "DataExtraction";
            LoadDataCommand = new RelayCommand(
                (a) => RequestLoadData?.Invoke(this, EventArgs.Empty),
                (a) => Table?.HasDecorator<TemplateDatasourceProviderDecorator>() ?? false);
        }

        private TableArtifact? _table;
        public TableArtifact? Table
        {
            get => _table;
            set
            {
                if (SetProperty(ref _table, value))
                {
                    LoadDataCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public RelayCommand LoadDataCommand { get; }

        public ObservableCollection<MultiSelectFieldModel> PropertiesDistinctValues { get; } = new ObservableCollection<MultiSelectFieldModel>();

        public event EventHandler? RequestLoadData;
        public event EventHandler<CreateFromSelectionEventArgs>? RequestCreateEntities;
        public event EventHandler<CreateFromSelectionEventArgs>? RequestCreateValueTypes;

        public void OnCreateEntities(MultiSelectFieldModel multiSelectFieldModel, DomainArtifact domain)
        {
            RequestCreateEntities?.Invoke(this, new CreateFromSelectionEventArgs(multiSelectFieldModel, domain));
        }

        public void OnCreateValueTypes(MultiSelectFieldModel multiSelectFieldModel, DomainArtifact domain)
        {
            RequestCreateValueTypes?.Invoke(this, new CreateFromSelectionEventArgs(multiSelectFieldModel, domain));
        }
    }

    public class CreateFromSelectionEventArgs : EventArgs
    {
        public MultiSelectFieldModel MultiSelectFieldModel { get; }
        public DomainArtifact TargetDomain { get; }

        public CreateFromSelectionEventArgs(MultiSelectFieldModel multiSelectFieldModel, DomainArtifact targetDomain)
        {
            MultiSelectFieldModel = multiSelectFieldModel;
            TargetDomain = targetDomain;
        }
    }
}
