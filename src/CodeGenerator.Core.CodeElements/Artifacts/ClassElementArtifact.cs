using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class ClassElementArtifact : CodeElementArtifactBase<ClassElement>
{
    public ClassElementArtifact(ClassElement classElement) : base(classElement)
    {
        AddChild(new FieldsContainerArtifact(classElement.Fields));
        AddChild(new PropertiesContainerArtifact(classElement.Properties));
        AddChild(new ConstructorsContainerArtifact(classElement.Constructors));
        AddChild(new MethodsContainerArtifact(classElement.Methods));
        AddChild(new EventsContainerArtifact(classElement.Events));
        AddChild(new IndexersContainerArtifact(classElement.Indexers));
        AddChild(new OperatorsContainerArtifact(classElement.Operators));
    }

    public ClassElementArtifact(ArtifactState artifactState) : base(artifactState) { }

    public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("braces");

    public bool IsRecord
    {
        get => CodeElement.IsRecord;
        set => CodeElement.IsRecord = value;
    }

    public FieldsContainerArtifact Fields => Children.OfType<FieldsContainerArtifact>().Single();
    public PropertiesContainerArtifact Properties => Children.OfType<PropertiesContainerArtifact>().Single();
    public ConstructorsContainerArtifact Constructors => Children.OfType<ConstructorsContainerArtifact>().Single();
    public MethodsContainerArtifact Methods => Children.OfType<MethodsContainerArtifact>().Single();
    public EventsContainerArtifact Events => Children.OfType<EventsContainerArtifact>().Single();
    public IndexersContainerArtifact Indexers => Children.OfType<IndexersContainerArtifact>().Single();
    public OperatorsContainerArtifact Operators => Children.OfType<OperatorsContainerArtifact>().Single();
}
