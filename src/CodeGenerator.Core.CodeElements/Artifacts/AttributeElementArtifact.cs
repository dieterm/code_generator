using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts
{
    public class AttributeElementArtifact : CodeElementArtifactBase<AttributeElement>
    {
        public AttributeElementArtifact(AttributeElement attribute)
            : base(attribute)
        {
        }

        public string AttributeName
        {
            get => CodeElement.AttributeName;
            set
            {
                CodeElement.AttributeName = value;
                RaisePropertyChangedEvent(nameof(AttributeName));
                RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        public override string TreeNodeText => AttributeName;

        /// <summary>
        /// ComboboxField with values from AttributeTarget enum
        /// </summary>
        public AttributeTarget Target
        {
            get => CodeElement.Target;
            set => CodeElement.Target = value;
        }

        public List<string> Arguments => CodeElement.Arguments;

        public Dictionary<string, string> NamedArguments => CodeElement.NamedArguments;
    }
}