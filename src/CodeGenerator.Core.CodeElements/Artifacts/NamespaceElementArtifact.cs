using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class NamespaceElementArtifact : CodeElementArtifactBase<NamespaceElement>
{
    public NamespaceElementArtifact(NamespaceElement namespaceElement)
        : base(namespaceElement)
    {
        AddChild(new NamespaceUsingsContainerArtifact(namespaceElement));
        AddChild(new TypesContainerArtifact(namespaceElement));
    }

    public NamespaceElementArtifact(ArtifactState artifactState) : base(artifactState) { }

    public override string TreeNodeText => CodeElement.FullName ?? "Namespace";

    public string FullName
    {
        get => CodeElement.FullName;
        set
        {
            CodeElement.FullName = value;
            RaisePropertyChangedEvent(nameof(FullName));
            RaisePropertyChangedEvent(nameof(TreeNodeText));
        }
    }

    public bool IsFileScoped
    {
        get { return CodeElement.IsFileScoped; }
        set
        {
            if (CodeElement.IsFileScoped != value)
            {
                CodeElement.IsFileScoped = value;
                RaisePropertyChangedEvent(nameof(IsFileScoped));
            }
        }
    }

    public TypesContainerArtifact Types => Children.OfType<TypesContainerArtifact>().Single();
    public NamespaceUsingsContainerArtifact Usings => Children.OfType<NamespaceUsingsContainerArtifact>().Single();
}
