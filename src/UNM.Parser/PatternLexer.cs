using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UNM.Parser.SimpleLexer;

namespace UNM.Parser
{
    /// <summary>
    /// Default implementation of IPatternLexer, uses an internal <see cref="ILexer"/> to handle
    /// most of the work.
    /// </summary>
    public class PatternLexer : IPatternLexer
    {
        private ILexer _lexer;

        /// <summary>
        /// Construct a new PatternLexer.
        /// </summary>
        /// <param name="lexer">The internal <see cref="ILexer"/> that produces raw tokens.</param>
        public PatternLexer(ILexer lexer)
        {
            _lexer = lexer;
        }

        /// <summary>
        /// Initialize the lexer. Adds token definitions to the internal <see cref="ILexer"/>.
        /// </summary>
        public void Initialize()
        {
            #region Setup token definitions
            _lexer.AddDefinition(new TokenDefinition(
                TokenType.TAG_SUB_FRAGMENT.ToString(),
                new Regex(@"<[0-9a-zA-Z_-]+>")));

            _lexer.AddDefinition(new TokenDefinition(
                TokenType.TAG_SUB_VARIABLE.ToString(),
                new Regex(@"<#[0-9a-zA-Z_-]+>")));

            _lexer.AddDefinition(new TokenDefinition(
                TokenType.TAG_SUB_PATTERN.ToString(),
                new Regex(@"<\^[0-9a-zA-Z_-]+>")));

            _lexer.AddDefinition(new TokenDefinition(
                TokenType.TAG_BRANCH_CHANCE.ToString(),
                new Regex(@"<%[0-9]+>")));

            _lexer.AddDefinition(new TokenDefinition(
                TokenType.TAG_BRANCH_CONTEXT.ToString(),
                new Regex(@"<@[0-9a-zA-Z_-]+>")));

            _lexer.AddDefinition(new TokenDefinition(
                TokenType.TAG_BRANCH_VARIABLE.ToString(),
                new Regex(@"<\$[0-9a-zA-Z_-]+>")));

            _lexer.AddDefinition(new TokenDefinition(
                TokenType.TAG_ELSE.ToString(),
                new Regex(@"\|")));

            _lexer.AddDefinition(new TokenDefinition(
                TokenType.BRANCH_START.ToString(),
                new Regex(@"{")));

            _lexer.AddDefinition(new TokenDefinition(
                TokenType.BRANCH_END.ToString(),
                new Regex(@"}")));

            _lexer.AddDefinition(new TokenDefinition(
                TokenType.CONTENT.ToString(),
                new Regex(@"[^\|{}<>]+"))); 
            #endregion
        }

        /// <summary>
        /// Process a pattern into a series of PatternTokens.
        /// </summary>
        /// <param name="pattern">The pattern to process.</param>
        /// <returns>A collection of all PatternTokens found in the pattern.</returns>
        /// <throws><see cref="PatternParseException"/> if there is a syntax error in the pattern.</throws>
        public IEnumerable<PatternToken> Process(string pattern)
        {
            try
            {
                var tokens = _lexer.Tokenize(pattern);

                return tokens
                    .Where(x => x.Type != "(end)")
                    .Select(x => new PatternToken(
                            (TokenType)Enum.Parse(typeof(TokenType), x.Type),
                            x.Value,
                            x.Position.Index));
            }
            catch (Exception ex)
            {
                throw new PatternParseException("Exception in lexing pattern.", ex);
            }
        }
    }
}
