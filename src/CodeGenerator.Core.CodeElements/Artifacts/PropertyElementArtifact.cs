using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class PropertyElementArtifact : CodeElementArtifactBase<PropertyElement>
{
    public PropertyElementArtifact(PropertyElement propertyElement) : base(propertyElement) { }
    public PropertyElementArtifact(ArtifactState artifactState) : base(artifactState) { }

    public string TypeName
    {
        get => CodeElement.Type.TypeName;
        set => CodeElement.Type.TypeName = value;
    }

    public bool HasGetter
    {
        get => CodeElement.HasGetter;
        set => CodeElement.HasGetter = value;
    }

    public bool HasSetter
    {
        get => CodeElement.HasSetter;
        set => CodeElement.HasSetter = value;
    }

    public bool IsInitOnly
    {
        get => CodeElement.IsInitOnly;
        set => CodeElement.IsInitOnly = value;
    }

    public bool IsAutoImplemented
    {
        get => CodeElement.IsAutoImplemented;
        set => CodeElement.IsAutoImplemented = value;
    }

    public string? InitialValue
    {
        get => CodeElement.InitialValue;
        set => CodeElement.InitialValue = value;
    }

    public CompositeStatement GetterBody => CodeElement.GetterBody;

    public CompositeStatement SetterBody => CodeElement.SetterBody;
}
