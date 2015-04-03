using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UNM.Parser.ContextExpressions;
using UNM.Parser.SimpleLexer;

namespace UNM.Parser
{
    /// <summary>
    /// Default implementation of IContextEpressionParser, uses an internal <see cref="ILexer"/>
    /// to handle most of the work.
    /// </summary>
    public class ContextExpressionParser : IContextExpressionParser
    {
        private ILexer _lexer;

        /// <summary>
        /// Construct a new LexingContextExpressionParser.
        /// </summary>
        /// <param name="lexer">The internal <see cref="ILexer"/> that produces raw tokens.</param>
        public ContextExpressionParser(ILexer lexer)
        {
            _lexer = lexer;
        }

        /// <summary>
        /// Initialize the expression parser.
        /// </summary>
        public void Initialize()
        {
            _lexer.AddDefinition(new TokenDefinition(
                TokenType.EXPRESSION_AND.ToString(),
                new Regex("&&")));

            _lexer.AddDefinition(new TokenDefinition(
                TokenType.EXPRESSION_OR.ToString(),
                new Regex("||")));

            _lexer.AddDefinition(new TokenDefinition(
                TokenType.EXPRESSION_NOT.ToString(),
                new Regex("!")));

            _lexer.AddDefinition(new TokenDefinition(
                TokenType.EXPRESSION_GROUP_START.ToString(),
                new Regex("\\(")));

            _lexer.AddDefinition(new TokenDefinition(
                TokenType.EXPRESSION_GROUP_END.ToString(),
                new Regex("\\)")));

            _lexer.AddDefinition(new TokenDefinition(
                TokenType.EXPRESSION_MATCH.ToString(),
                new Regex(".+")));
        }

        /// <summary>
        /// Parse a context expression into an expression tree.
        /// </summary>
        /// <param name="expression">The context expression as a string.</param>
        /// <returns>The expression tree representation of <paramref name="expression"/>.</returns>
        public IContextExpression ParseExpression(string expression)
        {
            try
            {
                var tokens = _lexer.Tokenize(expression)
                    .Where(x => x.Type != "(end)")
                    .Reverse()
                    .ToArray();

                IContextExpression rootExpression = null;

                foreach(var token in tokens)
                {
                    var typeEnum = (TokenType)Enum.Parse(typeof(TokenType), token.Type);

                    switch(typeEnum)
                    {
                        case TokenType.EXPRESSION_AND:
                            HandleAnd
                    }
                }
            }
            catch(Exception ex)
            {
                throw new ExpressionParseException("Exception in lexing expression: " + expression,
                    ex);
            }

            return new EmptyExpression();
        }
    }
}
