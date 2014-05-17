namespace UNM.Parser
{
    /// <summary>
    /// Lists capitalization schemes to use when processing patterns.
    /// </summary>
    public enum CapitalizationScheme
    {
        /// <summary>
        /// Capitalize the beginning of each NameFragment
        /// </summary>
        BY_FRAGMENT,

        /// <summary>
        /// Capitalize each word (text preceeded by a space).
        /// </summary>
        BY_WORDS,

        /// <summary>
        /// Capitalize the first letter of the result.
        /// </summary>
        FIRST_LETTER,

        /// <summary>
        /// Don't perform any capitalization.
        /// </summary>
        NONE,

        /// <summary>
        /// Capitalize the first letter of the pattern, and any letter following a punctuation
        /// mark and a space.
        /// </summary>
        BY_SENTENCE
    }
}
