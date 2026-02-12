using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class StructElementArtifact : CodeElementArtifactBase<StructElement>
{
    public StructElementArtifact(StructElement structElement) : base(structElement)
    {
        AddChild(new FieldsContainerArtifact(structElement.Fields));
        AddChild(new PropertiesContainerArtifact(structElement.Properties));
        AddChild(new ConstructorsContainerArtifact(structElement.Constructors));
        AddChild(new MethodsContainerArtifact(structElement.Methods));
        AddChild(new EventsContainerArtifact(structElement.Events));
    }

    public StructElementArtifact(ArtifactState artifactState) : base(artifactState) { }

    public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("braces");

    public bool IsRecord
    {
        get => CodeElement.IsRecord;
        set
        {
            if (CodeElement.IsRecord != value)
            {
                CodeElement.IsRecord = value;
                RaisePropertyChangedEvent(nameof(IsRecord));
            }
        }
    }

    public bool IsReadonly
    {
        get => CodeElement.IsReadonly;
        set
        {
            if (CodeElement.IsReadonly != value)
            {
                CodeElement.IsReadonly = value;
                RaisePropertyChangedEvent(nameof(IsReadonly));
            }
        }
    }

    public bool IsRef
    {
        get => CodeElement.IsRef;
        set
        {
            if (CodeElement.IsRef != value)
            {
                CodeElement.IsRef = value;
                RaisePropertyChangedEvent(nameof(IsRef));
            }
        }
    }

    public FieldsContainerArtifact Fields => Children.OfType<FieldsContainerArtifact>().Single();
    public PropertiesContainerArtifact Properties => Children.OfType<PropertiesContainerArtifact>().Single();
    public ConstructorsContainerArtifact Constructors => Children.OfType<ConstructorsContainerArtifact>().Single();
    public MethodsContainerArtifact Methods => Children.OfType<MethodsContainerArtifact>().Single();
    public EventsContainerArtifact Events => Children.OfType<EventsContainerArtifact>().Single();
}
