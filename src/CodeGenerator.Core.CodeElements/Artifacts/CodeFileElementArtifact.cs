using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.ProgrammingLanguages;
using CodeGenerator.Shared.Memento;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class CodeFileElementArtifact : CodeElementArtifactBase<CodeFileElement>
{
    public CodeFileElementArtifact(CodeFileElement codeFileElement)
        : base(codeFileElement)
    {
        AddChild(new CodeFileUsingsContainerArtifact(codeFileElement));
        AddChild(new NamespacesContainerArtifact(codeFileElement));
    }

    public CodeFileElementArtifact(ArtifactState artifactState) 
        : base(artifactState)
    {
        CodeElement = CodeFileElement.FromJson((string)artifactState.Properties["CodeElement"]!);

        AddChild(new CodeFileUsingsContainerArtifact(CodeElement));
        AddChild(new NamespacesContainerArtifact(CodeElement));
    }

    
    /// <summary>
    /// SingleLineTextField
    /// </summary>
    public string? FileHeader { 
        get => CodeElement.FileHeader; 
        set => CodeElement.FileHeader = value; 
    }
    /// <summary>
    /// ignore
    /// </summary>
    public List<string> TopLevelStatements => CodeElement.TopLevelStatements;


    /// <summary>
    /// BooleanField
    /// </summary>
    public bool? NullableContext
    {
        get => CodeElement.NullableContext;
        set => CodeElement.NullableContext = value;
    }

    /// <summary>
    /// BooleanField
    /// </summary>
    public bool UseImplicitUsings
    {
        get => CodeElement.UseImplicitUsings;
        set => CodeElement.UseImplicitUsings = value;
    }

    /// <summary>
    /// ComboboxField
    /// Get all languages from ProgrammingLanguages.ProgrammingLanguages.All
    /// </summary>
    public ProgrammingLanguage Language
    {
        get => CodeElement.Language;
        set => CodeElement.Language = value;
    }

    public CodeFileUsingsContainerArtifact Usings => Children.OfType<CodeFileUsingsContainerArtifact>().Single();
    public NamespacesContainerArtifact Namespaces => Children.OfType<NamespacesContainerArtifact>().Single();

    public override IMementoState CaptureState()
    {
        var state = base.CaptureState();
        state.Properties["CodeElement"] = CodeElement.ToJson();
        return state;
    }
}
