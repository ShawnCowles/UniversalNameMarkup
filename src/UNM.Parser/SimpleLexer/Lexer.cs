using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/*
 * Adapted from SimpleLexer by Drew Miller at: http://blogs.msdn.com/b/drew/archive/2009/12/31/a-simple-lexer-in-c-that-uses-regular-expressions.aspx
 * License unknown.
 */
namespace UNM.Parser.SimpleLexer
{
    /// <summary>
    /// The default implementation of <see cref="ILexer"/>.
    /// </summary>
    public class Lexer : ILexer
    {
        private Regex endOfLineRegex = new Regex(@"\r\n|\r|\n", RegexOptions.Compiled);

        private IList<TokenDefinition> tokenDefinitions = new List<TokenDefinition>();

        /// <summary>
        /// Add a token definition to the lexer.
        /// </summary>
        /// <param name="tokenDefinition">The token defintion to add.</param>
        public void AddDefinition(TokenDefinition tokenDefinition)
        {
            tokenDefinitions.Add(tokenDefinition);
        }

        /// <summary>
        /// Parse an input string into a series of tokens.
        /// </summary>
        /// <param name="source">The input to parse.</param>
        /// <returns>An enumeration of all tokens found in <paramref name="source"/></returns>
        public IEnumerable<Token> Tokenize(string source)
        {
            int currentIndex = 0;
            int currentLine = 1;
            int currentColumn = 0;

            while (currentIndex < source.Length)
            {
                TokenDefinition matchedDefinition = null;
                int matchLength = 0;

                foreach (var rule in tokenDefinitions)
                {
                    var match = rule.Regex.Match(source, currentIndex);

                    if (match.Success && (match.Index - currentIndex) == 0)
                    {
                        matchedDefinition = rule;
                        matchLength = match.Length;
                        break;
                    }
                }

                if (matchedDefinition == null)
                {
                    throw new Exception(string.Format(
                        "Unrecognized symbol '{0}' at index {1} (line {2}, column {3}).",
                        source[currentIndex], currentIndex, currentLine, currentColumn));
                }
                else
                {
                    var value = source.Substring(currentIndex, matchLength);

                    if (!matchedDefinition.IsIgnored)
                        yield return new Token(
                            matchedDefinition.Type,
                            value,
                            new TokenPosition(currentIndex, currentLine, currentColumn));

                    var endOfLineMatch = endOfLineRegex.Match(value);
                    if (endOfLineMatch.Success)
                    {
                        currentLine += 1;
                        currentColumn = value.Length - (endOfLineMatch.Index + endOfLineMatch.Length);
                    }
                    else
                    {
                        currentColumn += matchLength;
                    }

                    currentIndex += matchLength;
                }
            }

            yield return new Token("(end)", null, new TokenPosition(currentIndex, currentLine, currentColumn));
        }
    }
}
