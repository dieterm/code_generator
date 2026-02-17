namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents an enum member/value
    /// </summary>
    public class EnumMemberElement : CodeElement
    {
        /// <summary>
        /// Explicit value for this enum member (null means auto-increment)
        /// </summary>
        public object? Value { get; set; }

        /// <summary>
        /// Whether this member has an explicit value
        /// </summary>
        public bool HasExplicitValue => Value != null;

        public EnumMemberElement() { }

        public EnumMemberElement(string name)
        {
            Name = name;
        }

        public EnumMemberElement(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
