using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.MessageBus
{
    public interface IWorkspaceMessageBusSubscriber
    {
        void Subscribe(WorkspaceMessageBus messageBus);
        void Unsubscribe(WorkspaceMessageBus messageBus);
    }
}
