namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents an enum declaration
    /// </summary>
    public class EnumElement : TypeElement
    {
        /// <summary>
        /// The underlying type of the enum (default is int)
        /// </summary>
        public TypeReference? UnderlyingType { get; set; }

        /// <summary>
        /// Enum members/values
        /// </summary>
        public List<EnumMemberElement> Members { get; set; } = new();

        /// <summary>
        /// Whether this enum has the [Flags] attribute
        /// </summary>
        public bool IsFlags { get; set; }

        public EnumElement() { }

        public EnumElement(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Add a member with auto-incrementing value
        /// </summary>
        public EnumElement AddMember(string name, string? documentation = null)
        {
            Members.Add(new EnumMemberElement(name) { Documentation = documentation });
            return this;
        }

        /// <summary>
        /// Add a member with explicit value
        /// </summary>
        public EnumElement AddMember(string name, object value, string? documentation = null)
        {
            Members.Add(new EnumMemberElement(name, value) { Documentation = documentation });
            return this;
        }
    }

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
