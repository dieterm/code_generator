using CodeGenerator.Shared.Memento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts
{
    public sealed class ArtifactDecoratorFactory: MementoObjectFactory<IArtifactDecorator, ArtifactDecoratorState>
    {
        private static ArtifactDecoratorFactory Instance { get; } = new ArtifactDecoratorFactory();

        public static IArtifactDecorator CreateArtifactDecorator(ArtifactDecoratorState state)
        {
            return Instance.CreateMementoObject(state);
        }
    }
}
