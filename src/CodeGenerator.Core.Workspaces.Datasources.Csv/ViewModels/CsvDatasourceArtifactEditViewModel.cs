using CodeGenerator.Core.Workspaces.Datasources.Csv.Artifacts;
using CodeGenerator.Core.Workspaces.ViewModels.Common;
using CodeGenerator.UserControls.ViewModels;
using CodeGenerator.UserControls.Views.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Datasources.Csv.ViewModels
{
    public class CsvDatasourceArtifactEditViewModel : ArtifactEditViewModel<CsvDatasourceArtifact, CsvDatasourceGeneralTabViewModel>
    {
        public CsvDatasourceArtifactEditViewModel()
            : base("CSV Datasource", new CsvDatasourceGeneralTabViewModel())
        {
        }

        public CsvDatasourceArtifact? Datasource {
            get { return GeneralTab.EditViewModel.Datasource; }
            set { GeneralTab.EditViewModel.Datasource = value; }
        }
    }

    public class CsvDatasourceGeneralTabViewModel : ArtifactEditViewTabModel<CsvDatasourceArtifact>
    {
        public UserControlFieldModel UserControlField { get; set; }
        public CsvDatasourceGeneralTabViewModel() : base("General")
        {
            UserControlField = new UserControlFieldModel
            {
                Name = "UserControl",
                Label = "CSV Datasource Editor",
                Value = new CsvDatasourceEditViewModel()
            };
            FieldCollection.FieldModels.Add(UserControlField);
        }

        public CsvDatasourceEditViewModel EditViewModel => (CsvDatasourceEditViewModel)UserControlField.Value;
    }
}
