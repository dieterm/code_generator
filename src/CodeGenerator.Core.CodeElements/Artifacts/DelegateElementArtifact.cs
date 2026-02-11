using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class DelegateElementArtifact : CodeElementArtifactBase<DelegateElement>
{
    public DelegateElementArtifact(DelegateElement delegateElement) : base(delegateElement)
    {
        AddChild(new ParametersContainerArtifact(delegateElement.Parameters));
    }

    public DelegateElementArtifact(ArtifactState artifactState) : base(artifactState) { }

    public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("braces");

    public string ReturnTypeName
    {
        get => CodeElement.ReturnType.TypeName;
        set => CodeElement.ReturnType.TypeName = value;
    }

    public ParametersContainerArtifact Parameters => Children.OfType<ParametersContainerArtifact>().Single();
}
