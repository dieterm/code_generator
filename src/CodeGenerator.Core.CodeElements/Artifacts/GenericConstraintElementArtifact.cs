using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class GenericConstraintElementArtifact : CodeElementArtifactBase<GenericConstraintElement>
{
    public GenericConstraintElementArtifact(GenericConstraintElement constraintElement) : base(constraintElement) { }
    public GenericConstraintElementArtifact(ArtifactState artifactState, List<string> errors) : base(artifactState, errors) { }

    public override string TreeNodeText => $"where {CodeElement.TypeParameterName}";
    public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("filter");

    public string TypeParameterName
    {
        get => CodeElement.TypeParameterName;
        set
        {
            if (CodeElement.TypeParameterName != value)
            {
                CodeElement.TypeParameterName = value;
                RaisePropertyChangedEvent(nameof(TypeParameterName));
                RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }
    }

    public GenericConstraintKind ConstraintKind
    {
        get => CodeElement.ConstraintKind;
        set
        {
            if (CodeElement.ConstraintKind != value)
            {
                CodeElement.ConstraintKind = value;
                RaisePropertyChangedEvent(nameof(ConstraintKind));
            }
        }
    }

    public List<TypeReference> ConstraintTypes => CodeElement.ConstraintTypes;
}
