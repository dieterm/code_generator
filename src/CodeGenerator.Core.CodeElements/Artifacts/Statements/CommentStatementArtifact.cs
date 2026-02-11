using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts.Statements
{
    public class CommentStatementArtifact : StatementArtifactBase<CommentStatement>
    {
        
        public CommentStatementArtifact(CommentStatement comment)
            : base(comment)
        {
            
        }

        public string Text
        {
            get { return StatementElement.Text; }
            set
            {
                if (StatementElement.Text != value)
                {
                    StatementElement.Text = value;
                    RaisePropertyChangedEvent(nameof(Text));
                }
            }
        }   
       
    }
}