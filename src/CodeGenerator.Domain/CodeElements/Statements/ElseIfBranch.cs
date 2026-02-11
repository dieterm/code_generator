namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents an else-if branch
    /// </summary>
    public class ElseIfBranch : StatementElement
    {
        public string Condition { get; set; } = string.Empty;
        public List<StatementElement> Statements { get; set; } = new();
    }
}
