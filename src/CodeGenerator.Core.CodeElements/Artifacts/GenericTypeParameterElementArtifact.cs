using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class GenericTypeParameterElementArtifact : CodeElementArtifactBase<GenericTypeParameterElement>
{
    public GenericTypeParameterElementArtifact(GenericTypeParameterElement parameterElement) : base(parameterElement) { }
    public GenericTypeParameterElementArtifact(ArtifactState artifactState, List<string> errors) : base(artifactState, errors) { }

    public override string TreeNodeText => CodeElement.Variance switch
    {
        GenericVariance.Covariant => $"out {CodeElement.Name}",
        GenericVariance.Contravariant => $"in {CodeElement.Name}",
        _ => CodeElement.Name ?? string.Empty
    };

    public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("circle-dashed");

    public GenericVariance Variance
    {
        get => CodeElement.Variance;
        set
        {
            if (CodeElement.Variance != value)
            {
                CodeElement.Variance = value;
                RaisePropertyChangedEvent(nameof(Variance));
                RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }
    }
}
