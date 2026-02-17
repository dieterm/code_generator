namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Parameter modifiers
    /// </summary>
    public enum ParameterModifier
    {
        None,
        Ref,
        Out,
        In,
        Params,
        /// <summary>For Python *args</summary>
        VarArgs,
        /// <summary>For Python **kwargs</summary>
        KeywordArgs
    }
}
