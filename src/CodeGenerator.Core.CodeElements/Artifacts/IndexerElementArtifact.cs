using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class IndexerElementArtifact : CodeElementArtifactBase<IndexerElement>
{
    public IndexerElementArtifact(IndexerElement indexerElement) : base(indexerElement)
    {
        AddChild(new ParametersContainerArtifact(indexerElement.Parameters));
        AddChild(new CompositeStatementArtifact(indexerElement.GetterBody, true) { Name = nameof(GetterBody) });
        AddChild(new CompositeStatementArtifact(indexerElement.SetterBody, true) { Name = nameof(SetterBody) });
    }

    public IndexerElementArtifact(ArtifactState artifactState) : base(artifactState) { }

    public override string TreeNodeText => $"this[{string.Join(", ", CodeElement.Parameters.Select(p => p.Type.TypeName))}]";

    public string TypeName
    {
        get => CodeElement.Type.TypeName;
        set
        {
            if (CodeElement.Type.TypeName != value)
            {
                CodeElement.Type.TypeName = value;
                RaisePropertyChangedEvent(nameof(TypeName));
            }
        }
    }

    public bool HasGetter
    {
        get => CodeElement.HasGetter;
        set
        {
            if (CodeElement.HasGetter != value)
            {
                CodeElement.HasGetter = value;
                RaisePropertyChangedEvent(nameof(HasGetter));
            }
        }
    }

    public bool HasSetter
    {
        get => CodeElement.HasSetter;
        set
        {
            if (CodeElement.HasSetter != value)
            {
                CodeElement.HasSetter = value;
                RaisePropertyChangedEvent(nameof(HasSetter));
            }
        }
    }

    public ParametersContainerArtifact Parameters => Children.OfType<ParametersContainerArtifact>().Single();

    public CompositeStatement GetterBody => CodeElement.GetterBody;

    public CompositeStatement SetterBody => CodeElement.SetterBody;
}
