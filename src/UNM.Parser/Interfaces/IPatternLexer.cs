using System.Collections.Generic;
using UNM.Parser.Data;

namespace UNM.Parser.Interfaces
{
    /// <summary>
    /// Interface for a UNM pattern lexer which can pull tokens from a pattern.
    /// </summary>
    public interface IPatternLexer
    {
        /// <summary>
        /// Initialize the lexer, performing any computationally expensive setup tasks.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Process a pattern into a series of PatternTokens
        /// </summary>
        /// <param name="pattern">The pattern to process.</param>
        /// <returns>A collection of all PatternTokens found in the pattern.</returns>
        /// <throws><see cref="PatternParseException"/> if there is a syntax error in the pattern.</throws>
        IEnumerable<PatternToken> Process(string pattern);
    }
}
