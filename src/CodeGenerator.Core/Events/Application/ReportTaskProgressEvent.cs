using CodeGenerator.Core.MessageBus;

namespace CodeGenerator.Core.Events.Application
{
    public class ReportTaskProgressEvent : ApplicationEventArg
    {
        public string TaskName { get; }
        public int? PercentComplete { get; }
        public ReportTaskProgressEvent(string taskName, int? percentComplete)
        {
            TaskName = taskName;
            PercentComplete = percentComplete;
        }
    }
}
