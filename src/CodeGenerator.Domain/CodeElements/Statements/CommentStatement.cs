using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.CodeElements
{
    public class CommentStatement : StatementElement
    {
        /// <summary>
        /// The comment text (without the comment delimiters)
        /// </summary>
        public string Text { get; set; } = string.Empty;

        public CommentStatement() { }

        public CommentStatement(string text)
        {
            Text = text;
        }
    }
}
