namespace CodeGenerator.Shared.Ribbon;

/// <summary>
/// ViewModel for a ToolStrip button
/// </summary>
public class RibbonButtonViewModel : RibbonItemViewModel
{
    public RibbonButtonDisplayStyle DisplayStyle { get; set; } = RibbonButtonDisplayStyle.ImageAndText;
    public RibbonButtonSize Size { get; set; } = RibbonButtonSize.Large;

}

public enum RibbonButtonDisplayStyle
{
    Image,
    Text,
    ImageAndText
}

public enum RibbonButtonSize
{
    Small,
    Large
}
