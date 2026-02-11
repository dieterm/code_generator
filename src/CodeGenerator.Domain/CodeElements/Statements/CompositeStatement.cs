using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.CodeElements
{
    public class CompositeStatement : StatementElement
    {
        public List<StatementElement> Statements { get; } = new();

        /// <summary>
        /// Whether this composite statement has any statements
        /// </summary>
        public bool HasStatements => Statements.Count > 0;

        public void AddCommentLine(string commentText)
        {
            Statements.Add(new CommentStatement(commentText));
        }

        public void AddRawStatement(string body)
        {
            Statements.Add(new RawStatementElement(body));
        }
    }
}
