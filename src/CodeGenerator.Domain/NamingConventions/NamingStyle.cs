namespace CodeGenerator.Domain.NamingConventions
{
    /// <summary>
    /// Represents a naming convention style
    /// </summary>
    public enum NamingStyle
    {
        /// <summary>
        /// PascalCase (e.g., "MyClassName")
        /// </summary>
        PascalCase,

        /// <summary>
        /// camelCase (e.g., "myVariableName")
        /// </summary>
        CamelCase,

        /// <summary>
        /// snake_case (e.g., "my_table_name")
        /// </summary>
        SnakeCase,

        /// <summary>
        /// UPPER_SNAKE_CASE (e.g., "MY_CONSTANT")
        /// </summary>
        UpperSnakeCase,

        /// <summary>
        /// kebab-case (e.g., "my-file-name")
        /// </summary>
        KebabCase,

        /// <summary>
        /// lowercase (e.g., "myname")
        /// </summary>
        LowerCase,

        /// <summary>
        /// UPPERCASE (e.g., "MYNAME")
        /// </summary>
        UpperCase
    }
}
