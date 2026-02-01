using CodeGenerator.Application.ViewModels;
using CodeGenerator.Core.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Events.Application
{
    public class ShowArtifactPreviewEvent : ApplicationEventArg
    {
        public ArtifactPreviewViewModel ViewModel { get; }
        public ShowArtifactPreviewEvent(ArtifactPreviewViewModel viewModel)
        {
            ViewModel = viewModel;
        }
    }
}
