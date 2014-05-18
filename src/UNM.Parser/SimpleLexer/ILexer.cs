using System;
using System.Collections.Generic;

/*
 * Adapted from SimpleLexer by Drew Miller at: http://blogs.msdn.com/b/drew/archive/2009/12/31/a-simple-lexer-in-c-that-uses-regular-expressions.aspx
 * License unknown.
 */
namespace UNM.Parser.SimpleLexer
{
    /// <summary>
    /// Interface for a lexer capable parsing an input string into a series of predefined tokens.
    /// </summary>
    public interface ILexer
    {
        /// <summary>
        /// Add a token definition to the lexer.
        /// </summary>
        /// <param name="tokenDefinition">The token defintion to add.</param>
        void AddDefinition(TokenDefinition tokenDefinition);

        /// <summary>
        /// Parse an input string into a series of tokens.
        /// </summary>
        /// <param name="source">The input to parse.</param>
        /// <returns>An enumeration of all tokens found in <paramref name="source"/></returns>
        IEnumerable<Token> Tokenize(string source);
    }
}
