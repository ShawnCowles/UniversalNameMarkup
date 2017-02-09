using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UNM.Parser.ContextExpressions;
using UNM.Parser.Data;
using UNM.Parser.Interfaces;
using UNM.Parser.SimpleLexer;

namespace UNM.Parser.Implementation
{
    /// <summary>
    /// Default implementation of IContextEpressionParser, uses an internal <see cref="ILexer"/>
    /// to handle most of the work.
    /// </summary>
    public class ContextExpressionParser : IContextExpressionParser
    {
        private ILexer _lexer;

        private bool _initialized;

        /// <summary>
        /// Construct a new ContextExpressionParser.
        /// </summary>
        /// <param name="lexer">The internal <see cref="ILexer"/> that produces raw tokens.</param>
        public ContextExpressionParser(ILexer lexer)
        {
            _lexer = lexer;
        }

        /// <summary>
        /// Construct a new ContextExpressionParser using the default <see cref="ILexer"/>.
        /// </summary>
        public ContextExpressionParser()
        {
            _lexer = new Lexer();
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
                new Regex(@"\|\|")));

            _lexer.AddDefinition(new TokenDefinition(
                TokenType.EXPRESSION_NOT.ToString(),
                new Regex(@"!")));

            _lexer.AddDefinition(new TokenDefinition(
                TokenType.EXPRESSION_MATCH.ToString(),
                new Regex("[0-9A-Za-z-_]+")));

            _lexer.AddDefinition(new TokenDefinition(
                TokenType.WHITESPACE.ToString(),
                new Regex(@"\s")));

            _initialized = true;
        }

        /// <summary>
        /// Parse a context expression into an expression tree.
        /// </summary>
        /// <param name="expression">The context expression as a string.</param>
        /// <returns>The expression tree representation of <paramref name="expression"/>.</returns>
        public IContextExpression ParseExpression(string expression)
        {
            if(!_initialized)
            {
                throw new Exception("ContextExpressionParser must be initialized before use.");
            }

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
                            AttachToStack(stack, new MatchExpression(token.Value));
                            break;
                        case TokenType.EXPRESSION_NOT:
                            HandleNot(stack, new NotExpression());
                            break;
                        case TokenType.EXPRESSION_AND:
                            HandleNode(token, stack, new AndExpression());
                            break;
                        case TokenType.EXPRESSION_OR:
                            HandleNode(token, stack, new OrExpression());
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

        private void AttachToStack(Stack<IContextExpression> stack, IContextExpression newNode)
        {
            if (stack.Any())
            {
                var parent = stack.Pop();

                if (parent is NotExpression)
                {
                    if ((parent as NotExpression).Child != null)
                    {
                        throw new ExpressionParseException(string.Format(
                                "Parent for {0} already full.",
                                newNode.GetType().Name));
                    }

                    (parent as NotExpression).Child = newNode;

                    AttachToStack(stack, parent);
                }

                if (parent is NodeExpression)
                {
                    if ((parent as NodeExpression).RightChild != null)
                    {
                        throw new ExpressionParseException(string.Format(
                                "Parent for {0} already full.",
                                newNode.GetType().Name));
                    }

                    (parent as NodeExpression).RightChild = newNode;

                    AttachToStack(stack, parent);
                }
            }
            else
            {
                stack.Push(newNode);
            }
        }

        private void HandleNot(Stack<IContextExpression> stack, NotExpression node)
        {
            stack.Push(node);
        }

        private void HandleNode(Token token, Stack<IContextExpression> stack, NodeExpression node)
        {
            if(!stack.Any())
            {
                throw new ExpressionParseException(string.Format(
                    "AND without expression to left, position {0}",
                    token.Position));
            }

            node.LeftChild = stack.Pop();

            stack.Push(node);
        }
    }
}
