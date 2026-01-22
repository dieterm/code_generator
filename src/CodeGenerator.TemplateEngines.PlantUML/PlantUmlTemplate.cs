using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Templates;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.TemplateEngines.PlantUML;

/// <summary>
/// Represents a PlantUML diagram template
/// </summary>
public class PlantUmlTemplate : ITemplate
{
    public PlantUmlTemplate(string templateId, string diagramDefinition, bool useCaching = false)
    {
        TemplateId = templateId;
        DiagramDefinition = diagramDefinition;
        UseCaching = useCaching;
    }

    /// <summary>
    /// The PlantUML diagram definition text (e.g., "Bob -> Alice : Hello")
    /// </summary>
    public string DiagramDefinition { get; }

    public string TemplateId { get; }

    public TemplateType TemplateType => TemplateType.ImageFile;

    public bool UseCaching { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ITreeNodeIcon Icon { get; } =new AssemblyResourceTreeNodeIcon(typeof(PlantUmlTemplate).Assembly,"CodeGenerator.TemplateEngines.PlantUML.plantuml-icon.ico");
}
