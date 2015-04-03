﻿using System;
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
                new Regex(@"&&")));

            _lexer.AddDefinition(new TokenDefinition(
                TokenType.EXPRESSION_OR.ToString(),
                new Regex(@"\|")));

            _lexer.AddDefinition(new TokenDefinition(
                TokenType.EXPRESSION_NOT.ToString(),
                new Regex(@"!")));

            _lexer.AddDefinition(new TokenDefinition(
                TokenType.EXPRESSION_GROUP_START.ToString(),
                new Regex(@"\(")));

            _lexer.AddDefinition(new TokenDefinition(
                TokenType.EXPRESSION_GROUP_END.ToString(),
                new Regex(@"\)")));

            _lexer.AddDefinition(new TokenDefinition(
                TokenType.EXPRESSION_MATCH.ToString(),
                new Regex("[0-9A-Za-z-_]+")));

            _lexer.AddDefinition(new TokenDefinition(
                TokenType.WHITESPACE.ToString(),
                new Regex(@"\s")));
        }

        /// <summary>
        /// Parse a context expression into an expression tree.
        /// </summary>
        /// <param name="expression">The context expression as a string.</param>
        /// <returns>The expression tree representation of <paramref name="expression"/>.</returns>
        public IContextExpression ParseExpression(string expression)
        {
            var stack = new Stack<IContextExpression>();

            try
            {
                var tokens = _lexer.Tokenize(expression)
                    .Where(x => x.Type != "(end)")
                    .ToArray();

                foreach(var token in tokens)
                {
                    var typeEnum = (TokenType)Enum.Parse(typeof(TokenType), token.Type);

                    switch(typeEnum)
                    {
                        case TokenType.WHITESPACE:
                            break;
                        case TokenType.EXPRESSION_MATCH:
                            HandleBasic(token, stack, new MatchExpression(token.Value));
                            break;
                        case TokenType.EXPRESSION_NOT:
                            HandleBasic(token, stack, new NotExpression());
                            break;
                        default:
                            throw new ExpressionParseException(string.Format(
                                "Unexpected token: {0} at position: {1}",
                                token.Type, token.Position));
                    }
                }
            }
            catch(Exception ex)
            {
                throw new ExpressionParseException("Exception in lexing expression: " + expression,
                    ex);
            }

            if (!stack.Any())
            {
                return new EmptyExpression();
            }

            if(stack.Count > 1)
            {
                throw new ExpressionParseException(string.Format(
                    "Exception in lexing expression {0}, expression contains multiple root level nodes.",
                    expression));
            }

            return stack.First();
        }

        private void HandleBasic(Token token, Stack<IContextExpression> stack,
            IContextExpression newNode)
        {
            if (stack.Any())
            {
                if (stack.Peek() is NotExpression)
                {
                    if ((stack.Peek() as NotExpression).SubExpression != null)
                    {
                        throw new ExpressionParseException(string.Format(
                                "Unexpected token: {0} at position: {1}",
                                token.Type, token.Position));
                    }

                    (stack.Peek() as NotExpression).SubExpression = newNode;
                }

                if (stack.Peek() is NodeExpression)
                {
                    if ((stack.Peek() as NodeExpression).LeftChild != null)
                    {
                        throw new ExpressionParseException(string.Format(
                                "Unexpected token: {0} at position: {1}",
                                token.Type, token.Position));
                    }

                    (stack.Peek() as NodeExpression).LeftChild = newNode;
                }
            }
            else
            {
                stack.Push(newNode);
            }
        }
    }
}
