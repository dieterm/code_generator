namespace CodeGenerator.Core.Workspaces.Artifacts.Domains
{
    /// <summary>
    /// Represents the cardinality of a relation endpoint
    /// </summary>
    public enum RelationCardinality
    {
        /// <summary>
        /// Exactly one (1)
        /// </summary>
        One = 0,

        /// <summary>
        /// Zero or one (0-1)
        /// </summary>
        ZeroOrOne = 1,

        /// <summary>
        /// Zero or more (0-*)
        /// </summary>
        ZeroOrMany = 2,

        /// <summary>
        /// One or more (1-*)
        /// </summary>
        OneOrMany = 3
    }
}
