using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.Events
{
    public class DecoratorAddedEventArgs : EventArgs
    {
        public IArtifactDecorator Decorator { get; }
        public DecoratorAddedEventArgs(IArtifactDecorator decorator)
        {
            Decorator = decorator;
        }
    }
}
