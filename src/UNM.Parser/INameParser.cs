namespace UNM.Parser
{
    /// <summary>
    /// An interface for a NameParser, processes patterns to produce names.
    /// </summary>
    public interface INameParser
    {
        /// <summary>
        /// Initialize the NameParser.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Process a pattern.
        /// </summary>
        /// <param name="parameters">The pattern processing parameters to use.</param>
        /// <throws>A PattternParseException if there is any error processing the pattern.</throws>
        /// <returns>The result of processing the pattern.</returns>
        string Process(PatternProcessingParameters parameters);
    }
}
