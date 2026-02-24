using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.CodeElements.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DotNet.Winforms
{
    public class ButtonBaseElement<T> : ControlElement<T> where T : ButtonBase
    {
        public ButtonBaseElement(T control) : base(control)
        {

        }

        public override IEnumerable<StatementElement> CreateStatements(string variableName)
        {
            foreach(var statement in base.CreateStatements(variableName))
            {
                yield return statement;
            }

            if(!string.IsNullOrEmpty(Control.Text))
            {
                yield return new AssignmentStatement($"{variableName}.Text", $"\"{Control.Text}\"");
            }
            if (Control.UseVisualStyleBackColor)
            {
                yield return new AssignmentStatement($"{variableName}.UseVisualStyleBackColor", "true");
            }
        }
    }
}
