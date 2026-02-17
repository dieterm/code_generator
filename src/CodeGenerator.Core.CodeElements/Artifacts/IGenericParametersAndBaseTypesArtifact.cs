using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.CodeElements.Artifacts
{
    public interface IGenericParametersAndBaseTypesArtifact : IGenericTypesParametersArtifact
    {
        BaseTypesContainerArtifact BaseTypes { get; }
    }
}
