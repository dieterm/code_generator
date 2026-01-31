namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a statement in code (for method bodies, etc.)
    /// </summary>
    public abstract class StatementElement : CodeElement
    {
    }

    /// <summary>
    /// Represents a raw code statement (as a string)
    /// </summary>
    public class RawStatementElement : StatementElement
    {
        /// <summary>
        /// The raw code statement
        /// </summary>
        public string Code { get; set; } = string.Empty;

        public RawStatementElement() { }

        public RawStatementElement(string code)
        {
            Code = code;
        }

        public static implicit operator RawStatementElement(string code) => new(code);
    }

    /// <summary>
    /// Represents a return statement
    /// </summary>
    public class ReturnStatementElement : StatementElement
    {
        /// <summary>
        /// The expression to return (null for void return)
        /// </summary>
        public string? Expression { get; set; }

        public ReturnStatementElement() { }

        public ReturnStatementElement(string expression)
        {
            Expression = expression;
        }
    }

    /// <summary>
    /// Represents a throw statement
    /// </summary>
    public class ThrowStatementElement : StatementElement
    {
        /// <summary>
        /// The exception expression to throw
        /// </summary>
        public string? Expression { get; set; }

        public ThrowStatementElement() { }

        public ThrowStatementElement(string expression)
        {
            Expression = expression;
        }
    }

    /// <summary>
    /// Represents an if statement
    /// </summary>
    public class IfStatementElement : StatementElement
    {
        /// <summary>
        /// The condition expression
        /// </summary>
        public string Condition { get; set; } = string.Empty;

        /// <summary>
        /// Statements to execute if condition is true
        /// </summary>
        public List<StatementElement> ThenStatements { get; set; } = new();

        /// <summary>
        /// Statements to execute if condition is false
        /// </summary>
        public List<StatementElement> ElseStatements { get; set; } = new();

        /// <summary>
        /// Else-if branches
        /// </summary>
        public List<ElseIfBranch> ElseIfBranches { get; set; } = new();

        public IfStatementElement() { }

        public IfStatementElement(string condition)
        {
            Condition = condition;
        }
    }

    /// <summary>
    /// Represents an else-if branch
    /// </summary>
    public class ElseIfBranch
    {
        public string Condition { get; set; } = string.Empty;
        public List<StatementElement> Statements { get; set; } = new();
    }

    /// <summary>
    /// Represents a for loop
    /// </summary>
    public class ForStatementElement : StatementElement
    {
        /// <summary>
        /// Loop initializer
        /// </summary>
        public string? Initializer { get; set; }

        /// <summary>
        /// Loop condition
        /// </summary>
        public string? Condition { get; set; }

        /// <summary>
        /// Loop incrementer
        /// </summary>
        public string? Incrementer { get; set; }

        /// <summary>
        /// Loop body statements
        /// </summary>
        public List<StatementElement> Body { get; set; } = new();
    }

    /// <summary>
    /// Represents a foreach loop
    /// </summary>
    public class ForEachStatementElement : StatementElement
    {
        /// <summary>
        /// Type of the iteration variable (null for var)
        /// </summary>
        public TypeReference? VariableType { get; set; }

        /// <summary>
        /// Name of the iteration variable
        /// </summary>
        public string VariableName { get; set; } = string.Empty;

        /// <summary>
        /// The collection to iterate over
        /// </summary>
        public string Collection { get; set; } = string.Empty;

        /// <summary>
        /// Loop body statements
        /// </summary>
        public List<StatementElement> Body { get; set; } = new();

        public ForEachStatementElement() { }

        public ForEachStatementElement(string variableName, string collection)
        {
            VariableName = variableName;
            Collection = collection;
        }
    }

    /// <summary>
    /// Represents a while loop
    /// </summary>
    public class WhileStatementElement : StatementElement
    {
        /// <summary>
        /// Loop condition
        /// </summary>
        public string Condition { get; set; } = string.Empty;

        /// <summary>
        /// Loop body statements
        /// </summary>
        public List<StatementElement> Body { get; set; } = new();

        public WhileStatementElement() { }

        public WhileStatementElement(string condition)
        {
            Condition = condition;
        }
    }

    /// <summary>
    /// Represents a try-catch-finally statement
    /// </summary>
    public class TryCatchStatementElement : StatementElement
    {
        /// <summary>
        /// Try block statements
        /// </summary>
        public List<StatementElement> TryStatements { get; set; } = new();

        /// <summary>
        /// Catch blocks
        /// </summary>
        public List<CatchBlock> CatchBlocks { get; set; } = new();

        /// <summary>
        /// Finally block statements
        /// </summary>
        public List<StatementElement> FinallyStatements { get; set; } = new();

        /// <summary>
        /// Whether there is a finally block
        /// </summary>
        public bool HasFinally => FinallyStatements.Count > 0;
    }

    /// <summary>
    /// Represents a catch block
    /// </summary>
    public class CatchBlock
    {
        /// <summary>
        /// Exception type to catch (null for catch-all)
        /// </summary>
        public TypeReference? ExceptionType { get; set; }

        /// <summary>
        /// Exception variable name
        /// </summary>
        public string? ExceptionVariable { get; set; }

        /// <summary>
        /// When filter expression (C# 6+)
        /// </summary>
        public string? WhenFilter { get; set; }

        /// <summary>
        /// Catch block statements
        /// </summary>
        public List<StatementElement> Statements { get; set; } = new();
    }

    /// <summary>
    /// Represents a using statement (resource disposal)
    /// </summary>
    public class UsingStatementElement : StatementElement
    {
        /// <summary>
        /// Resource variable declaration or expression
        /// </summary>
        public string Resource { get; set; } = string.Empty;

        /// <summary>
        /// Using block statements
        /// </summary>
        public List<StatementElement> Body { get; set; } = new();

        /// <summary>
        /// Whether this is a using declaration (C# 8+)
        /// </summary>
        public bool IsDeclaration { get; set; }
    }

    /// <summary>
    /// Represents a switch statement
    /// </summary>
    public class SwitchStatementElement : StatementElement
    {
        /// <summary>
        /// Expression being switched on
        /// </summary>
        public string Expression { get; set; } = string.Empty;

        /// <summary>
        /// Switch cases
        /// </summary>
        public List<SwitchCase> Cases { get; set; } = new();

        /// <summary>
        /// Default case statements
        /// </summary>
        public List<StatementElement> DefaultStatements { get; set; } = new();
    }

    /// <summary>
    /// Represents a switch case
    /// </summary>
    public class SwitchCase
    {
        /// <summary>
        /// Case label values
        /// </summary>
        public List<string> Labels { get; set; } = new();

        /// <summary>
        /// Case pattern (for pattern matching)
        /// </summary>
        public string? Pattern { get; set; }

        /// <summary>
        /// When clause (for pattern matching)
        /// </summary>
        public string? WhenClause { get; set; }

        /// <summary>
        /// Case statements
        /// </summary>
        public List<StatementElement> Statements { get; set; } = new();
    }
}
