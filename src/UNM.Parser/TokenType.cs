namespace UNM.Parser
{
    /// <summary>
    /// An enumeration of all possible tokens in an UNM pattern.
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// Tag defining a subistution for a name fragment.
        /// </summary>
        TAG_SUB_FRAGMENT,

        /// <summary>
        /// Tag defining a subsitution for a variable.
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
        SUB_PATTERN
    }
}
