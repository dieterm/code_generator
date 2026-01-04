using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Generators.MessageBus
{
    public class CreatedRootArtifactEventArgs : GeneratorContextEventArgs
    {
        public CreatedRootArtifactEventArgs(GenerationResult result) : base(result)
        {
        }
    }
}
