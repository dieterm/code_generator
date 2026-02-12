using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class PropertyElementArtifact : CodeElementArtifactBase<PropertyElement>
{
    public PropertyElementArtifact(PropertyElement propertyElement) 
        : base(propertyElement) 
    {
        AddChild(new CompositeStatementArtifact(propertyElement.GetterBody, true) { Name = nameof(GetterBody) });
        AddChild(new CompositeStatementArtifact(propertyElement.SetterBody, true) { Name = nameof(SetterBody) });   
    }
    public PropertyElementArtifact(ArtifactState artifactState) : base(artifactState) { }

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

    public bool IsInitOnly
    {
        get => CodeElement.IsInitOnly;
        set
        {
            if (CodeElement.IsInitOnly != value)
            {
                CodeElement.IsInitOnly = value;
                RaisePropertyChangedEvent(nameof(IsInitOnly));
            }
        }
    }

    public bool IsAutoImplemented
    {
        get => CodeElement.IsAutoImplemented;
        set
        {
            if (CodeElement.IsAutoImplemented != value)
            {
                CodeElement.IsAutoImplemented = value;
                RaisePropertyChangedEvent(nameof(IsAutoImplemented));
            }
        }
    }

    public string? InitialValue
    {
        get => CodeElement.InitialValue;
        set
        {
            if (CodeElement.InitialValue != value)
            {
                CodeElement.InitialValue = value;
                RaisePropertyChangedEvent(nameof(InitialValue));
            }
        }
    }

    public CompositeStatement GetterBody => CodeElement.GetterBody;

    public CompositeStatement SetterBody => CodeElement.SetterBody;
}
