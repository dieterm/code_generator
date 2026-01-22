using CodeGenerator.Core.Templates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace CodeGenerator.TemplateEngines.Scriban
{
    public class EditControlScribanXmlConfig
    {
        public Stream GenerateScribanXmlConfigStream(ScribanTemplateInstance templateInstance)
        {
            var xmlContent = GenerateScribanXmlConfig(templateInstance);
            //File.WriteAllText(@"D:\Cloud\GitHub\code_generator\src\CodeGenerator.Core\IntellicenseSupport\Example_Config.xml", xmlContent);
            //Debug.WriteLine("");
            //Debug.WriteLine(xmlContent);
            //Debug.WriteLine("");
            var byteArray = Encoding.UTF8.GetBytes(xmlContent);
            return new MemoryStream(byteArray);
        }
        private string GenerateScribanXmlConfig(ScribanTemplateInstance templateInstance)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<ArrayOfConfigLanguage>");
            sb.AppendLine("  <ConfigLanguage name=\"Scriban\">");

            // Formats
            sb.AppendLine("    <formats>");
            foreach(var format in ScribanIntellisenceSupport.Formats)
            {
                sb.AppendLine("      " + format.ToXml());
            }
            //sb.AppendLine("      <format name=\"Text\" Font=\"Consolas, 10pt\" FontColor=\"Black\" />");
            //sb.AppendLine("      <format name=\"KeyWord\" Font=\"Consolas, 10pt, style=Bold\" FontColor=\"Blue\" />");
            //sb.AppendLine("      <format name=\"String\" Font=\"Consolas, 10pt\" FontColor=\"#A31515\" />");
            //sb.AppendLine("      <format name=\"Comment\" Font=\"Consolas, 10pt, style=Italic\" FontColor=\"Green\" />");
            //sb.AppendLine("      <format name=\"Operator\" Font=\"Consolas, 10pt, style=Bold\" FontColor=\"#FF6600\" />");
            //sb.AppendLine("      <format name=\"Parameter\" Font=\"Consolas, 10pt, style=Bold\" FontColor=\"Purple\" />");
            //sb.AppendLine("      <format name=\"Function\" Font=\"Consolas, 10pt\" FontColor=\"#8B4513\" />");
            //sb.AppendLine("      <format name=\"Number\" Font=\"Consolas, 10pt\" FontColor=\"DarkCyan\" />");
            sb.AppendLine("    </formats>");

            // Extensions
            sb.AppendLine("    <extensions>");
            sb.AppendLine("      <extension>scriban</extension>");
            sb.AppendLine("    </extensions>");

            // examples:
            /*
             <lexem BeginBlock="www" ContinueBlock="\S+" EndBlock="[\s\n]+" IsEndRegex="true" IsContinueRegex="true"
						IsComplex="true" OnlyLocalSublexems="true" IsPseudoEnd="true" Type="Hyperlink" />
            <lexem BeginBlock="[IQMiqm][0-9]+" ContinueBlock="[.]" EndBlock="[0-7]" IsBeginRegex="true"
				IsContinueRegex="true" IsEndRegex="true" IsComplex="true" OnlyLocalSublexems="true"
				Type="S7Addr" Priority="5" AutoNameExpression="^\s*(?&lt;text>.{0,20}\w).*\n" AutoNameTemplate="${text}..."
				IsCollapseAutoNamed="true" IsCollapsable="true" CollapseName="Empty Paragraph" NextID="441" />
			<lexem ID="441" BeginBlock=".+" IsBeginRegex="true" EndBlock="\n" IsEndRegex="true" Type="Text"
				IsComplex="true" IsPseudoEnd="true" AutoNameExpression="^\s*(?&lt;text>.{0,20}\w).*\n"
				AutoNameTemplate="${text}..." IsCollapseAutoNamed="true" IsCollapsable="true" CollapseName="Empty Paragraph"
				OnlyLocalSublexems="true">
             */

            // Lexems
            sb.AppendLine("    <lexems>");
            // Scriban delimiters - using BeginBlock + EndBlock pattern for {{ and }}
            sb.AppendLine("      <lexem BeginBlock=\"{\" EndBlock=\"}\" Type=\"Operator\" OnlyLocalSublexems=\"true\" IsComplex=\"true\">");
            sb.AppendLine("        <SubLexems>");
            sb.AppendLine("      <!-- support for multiline block -->");
            sb.AppendLine("      <lexem BeginBlock=\"\\n\" IsBeginRegex=\"true\" />");
            sb.AppendLine("      <lexem BeginBlock=\"{\" EndBlock=\"}\" Type=\"Operator\" OnlyLocalSublexems=\"false\" IsComplex=\"true\">");
            sb.AppendLine("        <SubLexems>");

            sb.AppendLine("          <!-- support for comments block (multi line) -->");
            sb.AppendLine("          <lexem BeginBlock=\"##\" EndBlock=\"##\" Type=\"Comment\" IsComplex=\"true\" OnlyLocalSublexems=\"true\" >");
            sb.AppendLine("              <SubLexems>");
            sb.AppendLine("                   <lexem BeginBlock=\"[^#]+\" IsBeginRegex=\"true\" Type=\"Comment\" />");
            sb.AppendLine("              </SubLexems>");
            sb.AppendLine("          </lexem>");

            //sb.AppendLine("          <!-- support for comments block (single line) -->");
            //sb.AppendLine("          <lexem BeginBlock=\"#\" EndBlock=\"\\n\" Type=\"Comment\" />");
            sb.AppendLine("          <!-- support for comments block (single line) -->");
            sb.AppendLine("          <lexem BeginBlock=\"#\" Type=\"Comment\" IsComplex=\"true\" OnlyLocalSublexems=\"true\" >");
            sb.AppendLine("              <SubLexems>");
            sb.AppendLine("                   <lexem BeginBlock=\"[^#]+\" EndBlock=\"[^}\\r\\n]+\" IsBeginRegex=\"true\" IsEndRegex=\"true\"  Type=\"Comment\" />");
            sb.AppendLine("              </SubLexems>");
            sb.AppendLine("          </lexem>");
            sb.AppendLine();



            sb.AppendLine();
            sb.AppendLine("          <!-- support for dot and pipe operators -->");
            sb.AppendLine("          <lexem BeginBlock=\".\" Type=\"Operator\" DropContextChoiceList=\"true\"/>");
            sb.AppendLine("          <lexem BeginBlock=\"|\" Type=\"Operator\" DropContextChoiceList=\"true\"/>");

            foreach(var trigger in ScribanIntellisenceSupport.Triggers)
            {
                sb.AppendLine();
                // for some reason "--" gives an xml error inside the commend block <!-- Context trigger: -- -->
                if (trigger.TriggerText != "--") 
                { 
                    if( trigger.Items.Count > 0)
                    { 
                        sb.AppendLine($"      <!-- Context trigger: {EscapeXml(trigger.TriggerText)} -->");
                    } else {
                        //sb.AppendLine($"      <!-- {trigger.FormatStyle.Name} '{EscapeXml(trigger.TriggerText)}' - {EscapeXml(trigger.Tooltip)} -->");
                    }
                }
                if (trigger.Items.Count == 0)
                {
                    sb.AppendLine($"      <lexem BeginBlock=\"{EscapeXml(trigger.TriggerText)}\" Type=\"{trigger.FormatStyle.Name}\" />");
                }
                else
                {
                    sb.AppendLine($"          <lexem BeginBlock=\"{EscapeXml(trigger.TriggerText)}\" Type=\"KeyWord\" OnlyLocalSublexems=\"true\" IsComplex=\"true\" >");
                    sb.AppendLine("            <SubLexems>");
                    sb.AppendLine("                 <lexem BeginBlock=\".\" Type=\"Operator\" />");
                    foreach (var item in trigger.Items)
                    {
                        sb.AppendLine($"                 <lexem BeginBlock=\"{EscapeXml(item.Text)}\" Type=\"Custom\" FormatName=\"Function\" />");
                    }
                    sb.AppendLine("            </SubLexems>");
                    sb.AppendLine("          </lexem>");
                }
            }
            foreach(var func in templateInstance.Functions.Keys)
            {
                sb.AppendLine($"      <lexem BeginBlock=\"{func}\" Type=\"Function\" DropContextChoiceList=\"false\" />");
            }

            // Parameters - example for LivingOrganisms.data
            foreach (var parameter in templateInstance.Parameters.Keys)
            {
                sb.AppendLine($"      <lexem BeginBlock=\"{parameter}\" Type=\"Custom\" FormatName=\"Parameter\" />");
            }

            // allow whitspaces
            sb.AppendLine("          <!-- Support whitespaces -->");
            sb.AppendLine("          <lexem BeginBlock=\"\\s+\" Type=\"Whitespace\" IsBeginRegex=\"true\" />");
            sb.AppendLine();
            // allow newlines
            sb.AppendLine("          <!-- Support newlines -->");
            sb.AppendLine("          <lexem BeginBlock=\"[\\r\\n]+\" Type=\"Whitespace\" IsBeginRegex=\"true\" />");
            // support for string blocks
            sb.AppendLine("          <!-- Support string blocks -->");
            sb.AppendLine("          <lexem BeginBlock=\"'\" EndBlock=\"'\" Type=\"String\" IsComplex=\"true\" />");
            sb.AppendLine("          <lexem BeginBlock=\"\\&quot;\" EndBlock=\"\\&quot;\" Type=\"String\" IsComplex=\"true\" />");
            sb.AppendLine("          <lexem BeginBlock=\"`\" EndBlock=\"`\" Type=\"String\" IsComplex=\"true\" />");
            sb.AppendLine();
            // support for numbers and text
            sb.AppendLine("          <!-- Support numbers and words blocks -->");
            sb.AppendLine("          <lexem BeginBlock=\"[0-9]+\" Type=\"Number\" IsBeginRegex=\"true\" IsComplex=\"true\" />");
            sb.AppendLine("          <lexem BeginBlock=\"[^\\}]+\" Type=\"Text\" IsBeginRegex=\"true\" IsComplex=\"true\" />");
            sb.AppendLine();

            sb.AppendLine("                 </SubLexems>");
            sb.AppendLine("             </lexem>");
            sb.AppendLine("        </SubLexems>");
            sb.AppendLine("      </lexem>");
            sb.AppendLine("    </lexems>");

            sb.AppendLine("     <splits>");
            foreach (var trigger in ScribanIntellisenceSupport.Triggers.Where(t => t.UseSplitter))
            {
                sb.AppendLine($"      <split>{EscapeXml(trigger.TriggerText)}</split>");
            }
            sb.AppendLine("         <split>##</split>");
            sb.AppendLine("         <split>{{</split>");
            sb.AppendLine("         <split>}}</split>");
            sb.AppendLine("     </splits>");
            sb.AppendLine("  </ConfigLanguage>");
            sb.AppendLine("</ArrayOfConfigLanguage>");

            return sb.ToString();
        }

        private void WriteScribanBlockSubLexems(StringBuilder sb)
        {
            // Temporarily disabled for testing

        }

        private string EscapeXml(string value)
        {
            return value
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }
    }
}
