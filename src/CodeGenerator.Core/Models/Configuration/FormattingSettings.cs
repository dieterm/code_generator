using CodeGenerator.Core.Enums;

namespace CodeGenerator.Core.Models.Configuration;
/// <summary>
/// Code formatting settings
/// </summary>
public class FormattingSettings
{
    public int IndentSize { get; set; } = 4;
    public bool UseTabs { get; set; } = false;
    public string LineEnding { get; set; } = "\r\n";
    public bool AddTrailingNewline { get; set; } = true;
    public int MaxLineLength { get; set; } = 120;
}