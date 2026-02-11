using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.CodeElements.Statements
{
    public class AssignmentStatement : StatementElement
    {
        /// <summary>
        /// The left-hand side of the assignment
        /// </summary>
        public string Left { get; set; } = string.Empty;
        /// <summary>
        /// The right-hand side of the assignment
        /// </summary>
        public string Right { get; set; } = string.Empty;
        public AssignmentStatement() { }
        public AssignmentStatement(string left, string right)
        {
            Left = left;
            Right = right;
        }
    }
}
