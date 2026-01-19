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
            Debug.WriteLine("");
            Debug.WriteLine(xmlContent);
            Debug.WriteLine("");
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
            sb.AppendLine("      <format name=\"Text\" Font=\"Consolas, 10pt\" FontColor=\"Black\" />");
            sb.AppendLine("      <format name=\"KeyWord\" Font=\"Consolas, 10pt, style=Bold\" FontColor=\"Blue\" />");
            sb.AppendLine("      <format name=\"String\" Font=\"Consolas, 10pt\" FontColor=\"#A31515\" />");
            sb.AppendLine("      <format name=\"Comment\" Font=\"Consolas, 10pt, style=Italic\" FontColor=\"Green\" />");
            sb.AppendLine("      <format name=\"Operator\" Font=\"Consolas, 10pt, style=Bold\" FontColor=\"#FF6600\" />");
            sb.AppendLine("      <format name=\"Parameter\" Font=\"Consolas, 10pt, style=Bold\" FontColor=\"Purple\" />");
            sb.AppendLine("      <format name=\"Function\" Font=\"Consolas, 10pt\" FontColor=\"#8B4513\" />");
            sb.AppendLine("      <format name=\"Number\" Font=\"Consolas, 10pt\" FontColor=\"DarkCyan\" />");
            sb.AppendLine("    </formats>");

            // Extensions
            sb.AppendLine("    <extensions>");
            sb.AppendLine("      <extension>scriban</extension>");
            sb.AppendLine("    </extensions>");

            // Lexems
            sb.AppendLine("    <lexems>");
            // Scriban delimiters - using BeginBlock + EndBlock pattern for {{ and }}
            sb.AppendLine("      <lexem BeginBlock=\"{\" EndBlock=\"}\" Type=\"Operator\" OnlyLocalSublexems=\"true\" IsComplex=\"true\">");
            sb.AppendLine("        <SubLexems>");
            sb.AppendLine("      <!-- support for multiline block -->");
            sb.AppendLine("      <lexem BeginBlock=\"\\n\" IsBeginRegex=\"true\" />");
            sb.AppendLine("      <lexem BeginBlock=\"{\" EndBlock=\"}\" Type=\"Operator\" OnlyLocalSublexems=\"false\" IsComplex=\"true\">");
            sb.AppendLine("        <SubLexems>");
            sb.AppendLine("          <!-- support for comments (multiline) block -->");
            sb.AppendLine("          <lexem BeginBlock=\"#\" Type=\"Comment\" IsComplex=\"true\" OnlyLocalSublexems=\"true\" >");
            sb.AppendLine("            <SubLexems>");
            sb.AppendLine("             <lexem BeginBlock=\"#\" Type=\"Comment\" IsComplex=\"true\" OnlyLocalSublexems=\"true\" >");
            sb.AppendLine("                 <SubLexems>");
            sb.AppendLine("                     <lexem BeginBlock=\"[^}\\r\\n]+\" IsBeginRegex=\"true\" Type=\"Comment\" />");
            sb.AppendLine("                 </SubLexems>");
            sb.AppendLine("             </lexem>"); 
            sb.AppendLine("            </SubLexems>");
            sb.AppendLine("          </lexem>");
            sb.AppendLine("          <!-- support for dot and pipe operators -->");
            sb.AppendLine("          <lexem BeginBlock=\".\" Type=\"Operator\" DropContextChoiceList=\"true\"/>");
            sb.AppendLine("          <lexem BeginBlock=\"|\" Type=\"Operator\" DropContextChoiceList=\"true\"/>");
            // Scriban keywords
            var keywords = new[] {
                "if", "else", "elseif", "end",  "in", "while", "break",
                "continue", "func", "ret", "capture", "readonly", "import",
                "with", "wrap", "include", "true", "false", "null", "empty",
                "blank", "this", "tablerow", "case", "when"
            };

            foreach (var keyword in keywords)
            {
                sb.AppendLine($"      <lexem BeginBlock=\"{keyword}\" Type=\"KeyWord\" DropContextChoiceList=\"false\" />");
            }

            foreach(var trigger in ScribanIntellisenceSupport.Triggers)
            {
                sb.AppendLine($"      <!-- Context trigger: {EscapeXml(trigger.TriggerText)} -->");
                if(trigger.Items.Count == 0)
                {
                    sb.AppendLine($"      <lexem BeginBlock=\"{EscapeXml(trigger.TriggerText)}\" Type=\"{(trigger.TriggerText=="for" || trigger.TriggerText=="while" ? "KeyWord" : "Function")}\" DropContextChoiceList=\"false\" />");
                } 
                else
                {
                    sb.AppendLine($"          <lexem BeginBlock=\"{EscapeXml(trigger.TriggerText)}\" Type=\"KeyWord\" IsComplex=\"true\" >");
                    sb.AppendLine("            <SubLexems>");
                    foreach (var item in trigger.Items)
                    {
                        sb.AppendLine($"                <lexem BeginBlock=\"{EscapeXml(item.Text)}\" Type=\"Function\" />");
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
            //sb.AppendLine("          <lexem BeginBlock=\"LivingOrganisms\" IsComplex=\"true\" Type=\"Custom\" FormatName=\"Parameter\">");
            //sb.AppendLine("            <SubLexems>");
            //sb.AppendLine("              <lexem BeginBlock=\"data\" Type=\"Custom\" FormatName=\"Parameter\" />");
            //sb.AppendLine("            </SubLexems>");
            //sb.AppendLine("          </lexem>");
            //sb.AppendLine("          <lexem BeginBlock=\"LivingOrganisms\" Type=\"Parameter\" IsComplex=\"true\" OnlyLocalSublexems=\"true\" >");
            //sb.AppendLine("            <SubLexems>"); // LivingOrganisms
            //sb.AppendLine("              <lexem BeginBlock=\"data\" Type=\"Parameter\"  />");
            //////sb.AppendLine("                <SubLexems>"); // data
            ////sb.AppendLine("                     <lexem BeginBlock=\"[\" EndBlock=\"]\" Type=\"KeyWord\"  IsComplex=\"true\" >");
            ////sb.AppendLine("                         <SubLexems>"); // indexer
            ////sb.AppendLine("                             <lexem BeginBlock=\"[0-9]+\" IsBeginRegex=\"true\" Type=\"Number\" IsComplex=\"true\" />");
            ////sb.AppendLine("                             <lexem BeginBlock=\"&quot;\" EndBlock=\"&quot;\" Type=\"String\" IsComplex=\"true\" >");
            ////sb.AppendLine("                                 <SubLexems>"); // string indexer
            ////sb.AppendLine("                                     <lexem BeginBlock=\"[^&quot;]+\" IsBeginRegex=\"true\" Type=\"String\" />");
            ////sb.AppendLine("                                 </SubLexems>"); // string indexer
            ////sb.AppendLine("                             </lexem>");
            ////sb.AppendLine("                         </SubLexems>"); // indexer
            ////sb.AppendLine("                     </lexem>");
            //////sb.AppendLine("                     <lexem BeginBlock=\"kingdoms\" Type=\"Parameter\" />");
            //////sb.AppendLine("                </SubLexems>"); // data
            //////sb.AppendLine("              </lexem>");
            //sb.AppendLine("            </SubLexems>"); // LivingOrganisms
            //sb.AppendLine("          </lexem>");
            // LivingOrganisms.data[0].kingdoms


            sb.AppendLine("        </SubLexems>");
            sb.AppendLine("      </lexem>");
            sb.AppendLine("        </SubLexems>");
            sb.AppendLine("      </lexem>");
            sb.AppendLine("    </lexems>");

            sb.AppendLine("     <splits>");
            //sb.AppendLine("         <split>{%{</split>");
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
