using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.CodeElements.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DotNet.Winforms
{
    public class ButtonElement : ButtonBaseElement<Button>
    {
        public ButtonElement(Button control) : base(control)
        {

        }

        override public IEnumerable<StatementElement> CreateStatements(string variableName)
        {
            foreach (var statement in base.CreateStatements(variableName))
            {
                yield return statement;
            }
            
            if(Control.AutoSizeMode != AutoSizeMode.GrowOnly)
            {
                yield return new AssignmentStatement($"{variableName}.AutoSizeMode", $"System.Windows.Forms.AutoSizeMode.{Control.AutoSizeMode}");
            }
        }
    }
}
