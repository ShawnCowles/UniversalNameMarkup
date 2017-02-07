namespace UNM.Parser.Data
{
    /// <summary>
    /// An enumeration of all possible tokens in an UNM pattern.
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// Tag defining a substitution for a name fragment.
        /// </summary>
        TAG_SUB_FRAGMENT,

        /// <summary>
        /// Tag defining a substitution for a variable.
        /// </summary>
        TAG_SUB_VARIABLE,

        /// <summary>
        /// Tag defining a branch based on percentage chance.
        /// </summary>
        TAG_BRANCH_CHANCE,

        /// <summary>
        /// Tag defining a branch based on context presence.
        /// </summary>
        TAG_BRANCH_CONTEXT,

        /// <summary>
        /// Tag defining a branch based on the presence of a variable.
        /// </summary>
        TAG_BRANCH_VARIABLE,

        /// <summary>
        /// Tag defining an else in a branch
        /// </summary>
        TAG_ELSE,

        /// <summary>
        /// Non-UNM content in a pattern.
        /// </summary>
        CONTENT,

        /// <summary>
        /// Start of a branch group.
        /// </summary>
        BRANCH_START,

        /// <summary>
        /// End of a branch group.
        /// </summary>
        BRANCH_END,

        /// <summary>
        /// Tag defining a pattern that should be looked up, processed, and substituted in.
        /// </summary>
        TAG_SUB_PATTERN,

        /// <summary>
        /// Token defining a NOT.
        /// </summary>
        EXPRESSION_NOT,

        /// <summary>
        /// Token defining an AND
        /// </summary>
        EXPRESSION_AND,

        /// <summary>
        /// Token defining an OR
        /// </summary>
        EXPRESSION_OR,

        /// <summary>
        /// Token defining match component of expression
        /// </summary>
        EXPRESSION_MATCH,

        /// <summary>
        /// Token defining whitespace
        /// </summary>
        WHITESPACE,
    }
}
