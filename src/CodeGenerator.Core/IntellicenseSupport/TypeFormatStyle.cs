using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.IntellicenseSupport
{

    public class TypeFormatStyle
    {

        public TypeFormatStyle(string name, string fontColor)
        {
            if(string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Custom format styles must have a valid name.", nameof(name));
            }
            if(name.Equals("Custom", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Custom format styles must have a valid name other than 'Custom'.", nameof(name));
            }
            if(Enum.TryParse<FormatType>(name, out var parsedType))
            {
                throw new ArgumentException("Use the other constructor for predefined format styles.", nameof(name));
            }
            Name = name;
            FontColor = fontColor;
            IsCustom = true;
            Type = FormatType.Custom;
        }
        public TypeFormatStyle(FormatType name, string fontColor)
        {
            if(name == FormatType.Custom)
            {
                throw new ArgumentException("Use the other constructor for custom format styles.", nameof(name));
            }
            Name = Enum.GetName(typeof(FormatType), name)!;
            FontColor = fontColor;
            IsCustom = false;
            Type = name;
        }
        public FormatType Type { get; set; }
        /// <summary>
        /// eg. "KeyWord", "String", "Comment", "Custom"
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// indicates whether this is a custom defined format style.<br />
        /// if true, renders as &lt;format Type=&quot;Custom&quot; FormatName=&quot;...&quot; /&gt; <br />
        /// </summary>
        public bool IsCustom { get; }
        /// <summary>
        /// Font family name
        /// eg. "Consolas"
        /// </summary>
        public string FontName { get; set; } = "Consolas";
        /// <summary>
        /// Font size in points 
        /// eg. 10 -> 10pt
        /// </summary>
        public int FontSize { get; set; } = 10;
        /// <summary>
        /// Color name or hex code
        /// eg. "Black" or "#A31515"
        /// </summary>
        public string? FontColor { get; } = null;
        /// <summary>
        /// eg. "Solid", "Dot", "Dash", "Wave"
        /// </summary>
        public UnderlineStyle? Underline { get; set; }
        /// <summary>
        /// Color name or hex code
        /// eg. "Black" or "#A31515"
        /// </summary>
        public string? LineColor { get; set; }
        /// <summary>
        /// Color name or hex code
        /// eg. "Black" or "#A31515"    
        /// </summary>
        public string? BorderColor { get; set; } = null;
        /// <summary>
        /// eg. "Solid", "Dot", "Dash"
        /// </summary>
        public FrameBorderStyle? BorderStyle { get; set; } = null;
        /// <summary>
        /// Color name or hex code
        /// eg. "Highlight" or "#FFFF00"
        /// </summary>
        public string? BackColor { get; set; } = null;
        public bool Italic { get; set; } = false;
        public bool Bold { get; set; } = false;

        /// <summary>
        /// Generate an XML fragment representing this format style. <br />
        /// Example output: <br />
        /// &lt;format name=&quot;Text&quot; Font=&quot;Consolas, 10pt, style=Bold&quot; ... /&gt;
        /// </summary>
        public string ToXml()
        {
            /*  sb.AppendLine("      <format name=\"Text\" Font=\"Consolas, 10pt\" FontColor=\"Black\" />");
    sb.AppendLine("      <format name=\"KeyWord\" Font=\"Consolas, 10pt, style=Bold\" FontColor=\"Blue\" />");
    sb.AppendLine("      <format name=\"String\" Font=\"Consolas, 10pt\" FontColor=\"#A31515\" />");
    sb.AppendLine("      <format name=\"Comment\" Font=\"Consolas, 10pt, style=Italic\" FontColor=\"Green\" />");
    sb.AppendLine("      <format name=\"Operator\" Font=\"Consolas, 10pt, style=Bold\" FontColor=\"#FF6600\" />");
    sb.AppendLine("      <format name=\"Parameter\" Font=\"Consolas, 10pt, style=Bold\" FontColor=\"Purple\" />");
    sb.AppendLine("      <format name=\"Function\" Font=\"Consolas, 10pt\" FontColor=\"#8B4513\" />");
    sb.AppendLine("      <format name=\"Number\" Font=\"Consolas, 10pt\" FontColor=\"DarkCyan\" />");
 BackColor="Highlight" underline="Solid" LineColor="Blue" BorderColor="Gray" BorderStyle="Solid" 
            <format name="CollapsedText" Font="Courier New, 10pt" FontColor="Black" BackColor="White"
				BorderColor="Gray" BorderStyle="Solid" />
			<format name="FormatXXXYYY" Font="Courier New, 10pt" FontColor="Pink" />
			<format name="Hyperlink" Font="Courier New, 10pt" FontColor="Blue" underline="Solid" LineColor="Blue" />
  */

            var sb = new StringBuilder();
            var fontStyle = Italic && Bold ? ", style=Italic Bold" : Italic ? ", style=Italic" : Bold ? ", style=Bold" : "";
            sb.Append($"<format name=\"{Name}\" Font=\"{FontName}, {FontSize}pt{fontStyle}\"");
            if (!string.IsNullOrEmpty(FontColor))
            {
                sb.Append($" FontColor=\"{FontColor}\"");
            }
            if (!string.IsNullOrEmpty(BackColor))
            {
                sb.Append($" BackColor=\"{BackColor}\"");
            }
            if (Underline.HasValue)
            {
                sb.Append($" underline=\"{Underline.Value}\"");
            }
            if (!string.IsNullOrEmpty(LineColor))
            {
                sb.Append($" LineColor=\"{LineColor}\"");
            }
            if (!string.IsNullOrEmpty(BorderColor))
            {
                sb.Append($" BorderColor=\"{BorderColor}\"");
            }
            if (BorderStyle.HasValue)
            {
                sb.Append($" BorderStyle=\"{BorderStyle.Value}\"");
            }
            sb.Append(" />");

            return sb.ToString();
        }
     }
}
