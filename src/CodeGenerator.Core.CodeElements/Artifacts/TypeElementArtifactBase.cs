using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.CodeElements.Artifacts
{
    public abstract class TypeElementArtifactBase<TTypeElement> : CodeElementArtifactBase<TTypeElement>, IGenericParametersAndBaseTypesArtifact
        where TTypeElement : TypeElement
    {
        public TypeElementArtifactBase(TTypeElement codeElement) : base(codeElement)
        {
            AddChild(new GenericConstraintsContainerArtifact(codeElement.GenericConstraints));
            AddChild(new GenericTypeParametersContainerArtifact(codeElement.GenericTypeParameters));
            AddChild(new BaseTypesContainerArtifact(codeElement.BaseTypes));
        }

        public TypeElementArtifactBase(ArtifactState artifactState, List<string> errors) : base(artifactState, errors)
        {
        }

        public BaseTypesContainerArtifact BaseTypes => Children.OfType<BaseTypesContainerArtifact>().Single();
        public GenericConstraintsContainerArtifact GenericConstraints => Children.OfType<GenericConstraintsContainerArtifact>().Single();
        public GenericTypeParametersContainerArtifact GenericTypeParameters => Children.OfType<GenericTypeParametersContainerArtifact>().Single();
    }
}
