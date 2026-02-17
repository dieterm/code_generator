using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class InterfaceElementArtifact : TypeElementArtifactBase<InterfaceElement>
{
    public InterfaceElementArtifact(InterfaceElement interfaceElement) : base(interfaceElement)
    {
        AddChild(new PropertiesContainerArtifact(interfaceElement.Properties));
        AddChild(new MethodsContainerArtifact(interfaceElement.Methods));
        AddChild(new EventsContainerArtifact(interfaceElement.Events));
        AddChild(new IndexersContainerArtifact(interfaceElement.Indexers));
    }

    public InterfaceElementArtifact(ArtifactState artifactState, List<string> errors) : base(artifactState, errors) { }

    public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("braces");

    public PropertiesContainerArtifact Properties => Children.OfType<PropertiesContainerArtifact>().Single();
    public MethodsContainerArtifact Methods => Children.OfType<MethodsContainerArtifact>().Single();
    public EventsContainerArtifact Events => Children.OfType<EventsContainerArtifact>().Single();
    public IndexersContainerArtifact Indexers => Children.OfType<IndexersContainerArtifact>().Single();

    
}
