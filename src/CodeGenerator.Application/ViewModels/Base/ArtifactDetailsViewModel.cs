using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.ViewModels.Workspace
{
    public class ArtifactDetailsViewModel : ViewModelBase
    {
        private ViewModelBase? _detailsViewModel;
        public ViewModelBase? DetailsViewModel
        {
            get { return _detailsViewModel; }
            set { SetProperty(ref _detailsViewModel, value); }
        }

    }
}
