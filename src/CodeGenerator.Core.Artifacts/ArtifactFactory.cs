using CodeGenerator.Shared.Memento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts
{
    public sealed class ArtifactFactory : MementoObjectFactory<IArtifact, ArtifactState>
    {
        private static ArtifactFactory Instance { get; } = new ArtifactFactory();

        public static IArtifact? CreateArtifact(ArtifactState state, List<string> errors)
        {
            return Instance.CreateMementoObject(state, errors);
        }

    }
}
