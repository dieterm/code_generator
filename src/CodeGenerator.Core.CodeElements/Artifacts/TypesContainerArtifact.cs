using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;
using System.Collections;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class TypesContainerArtifact : CodeElementArtifactBase<NamespaceElement>, IEnumerable<CodeElementArtifactBase>
{
    
    public TypesContainerArtifact(NamespaceElement namespaceElement) : base(namespaceElement)
    {
        foreach (var type in namespaceElement.Types)
        {
            AddChild(CreateTypeArtifact(type));
        }
    }

    public TypesContainerArtifact(ArtifactState artifactState) : base(artifactState) { }

    public override string TreeNodeText => "Types";
    public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("folder");

    public static CodeElementArtifactBase CreateTypeArtifact(TypeElement type)
    {
        return type switch
        {
            ClassElement cls => new ClassElementArtifact(cls),
            InterfaceElement iface => new InterfaceElementArtifact(iface),
            StructElement strct => new StructElementArtifact(strct),
            EnumElement enm => new EnumElementArtifact(enm),
            DelegateElement del => new DelegateElementArtifact(del),
            _ => throw new NotSupportedException($"Type element {type.GetType().Name} is not supported.")
        };
    }

    public IEnumerable<ClassElementArtifact> Classes => Children.OfType<ClassElementArtifact>();
    public IEnumerable<InterfaceElementArtifact> Interfaces => Children.OfType<InterfaceElementArtifact>();
    public IEnumerable<StructElementArtifact> Structs => Children.OfType<StructElementArtifact>();
    public IEnumerable<EnumElementArtifact> Enums => Children.OfType<EnumElementArtifact>();
    public IEnumerable<DelegateElementArtifact> Delegates => Children.OfType<DelegateElementArtifact>();

    public IEnumerator<CodeElementArtifactBase> GetEnumerator() => Children.OfType<CodeElementArtifactBase>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
