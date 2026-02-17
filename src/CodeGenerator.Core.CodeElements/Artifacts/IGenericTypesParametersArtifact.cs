using CodeGenerator.Domain.CodeElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.CodeElements.Artifacts
{
    public interface IGenericTypesParametersArtifact
    {
        GenericConstraintsContainerArtifact GenericConstraints { get; }
        GenericTypeParametersContainerArtifact GenericTypeParameters { get; }
    }
}
