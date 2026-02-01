using CodeGenerator.Application.ViewModels;
using CodeGenerator.Core.Events.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.MessageBus
{
    public class ApplicationMessageBus : MessageBus<ApplicationEventArg>
    {
        public void ReportApplicationStatus(string statusMessage)
        {
            Publish(new ReportApplicationStatusEvent(statusMessage));
        }

        public void ReportTaskProgress(string taskName, int? percentComplete)
        {
            Publish(new ReportTaskProgressEvent(taskName, percentComplete));
        }

        public void ShowArtifactPreview(ArtifactPreviewViewModel viewModel)
        {
            Publish(new ShowArtifactPreviewEvent(viewModel));
        }
    }
}
