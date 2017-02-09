using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UNM.GCS.Data.ExpressionNodes;
using UNM.GCS.Interfaces;
using UNM.Parser.SimpleLexer;

namespace UNM.GCS.Implementation
{
    /// <summary>
    /// An implementation of <see cref="IAvailabilityExpressionEvaluator"/> that matches availability
    /// expressions by matching against variables passed into the <see cref="IConversationSystem"/>.
    /// 
    /// ex: race="Salax" &amp;&amp; location="Storm Point" would pass if the variables "race" and "location"
    /// were passed into the <see cref="IConversationSystem"/> with the values of "Salax" and "Storm Point" respectively.
    /// 
    /// AND (&amp;&amp;) OR (||) NOT (!) are all accounted for, ex: (foo="bar" &amp;&amp; ! up="down") || right="left"
    /// </summary>
    public class VariableAvailabilityExpressionEvaluator : IAvailabilityExpressionEvaluator
    {
        private const string TOKEN_VARIABLE = "variable";
        private const string TOKEN_VALUE = "value";
        private const string TOKEN_EQUALITY = "equality";
        private const string TOKEN_AND = "and";
        private const string TOKEN_WHITESPACE = "whitespace";
        private const string TOKEN_OR = "or";
        private const string TOKEN_NOT = "not";

        private ILexer _lexer;

        /// <summary>
        /// Construct a new VariableAvailabilityExpressionEvaluator.
        /// </summary>
        public VariableAvailabilityExpressionEvaluator()
        {
            _lexer = new Lexer();

            _lexer.AddDefinition(new TokenDefinition(
                TOKEN_WHITESPACE,
                new Regex("\\s+")));

            _lexer.AddDefinition(new TokenDefinition(
                TOKEN_EQUALITY,
                new Regex("=")));

            _lexer.AddDefinition(new TokenDefinition(
                TOKEN_AND,
                new Regex("&&")));

            _lexer.AddDefinition(new TokenDefinition(
                TOKEN_OR,
                new Regex("\\|\\|")));

            _lexer.AddDefinition(new TokenDefinition(
                TOKEN_NOT,
                new Regex("!")));

            _lexer.AddDefinition(new TokenDefinition(
                TOKEN_VALUE,
                new Regex("\"[\\s0-9a-zA-Z_-]+\"")));

            _lexer.AddDefinition(new TokenDefinition(
                TOKEN_VARIABLE,
                new Regex("[0-9a-zA-Z_-]+")));
        }

        /// <summary>
        /// Evaluate and availability expression.
        /// </summary>
        /// <param name="availabilityExpression">The expression to evaluate.</param>
        /// <param name="variables">The variables passed into the <see cref="IConversationSystem"/>.</param>
        /// <returns>True if the expression evaluates positively, false otherwise.</returns>
        public bool Evaluate(string availabilityExpression, Dictionary<string, string> variables)
        {
            var tokens = _lexer.Tokenize(availabilityExpression)
                .Where(x => x.Type != "(end)")
                .Where(x => x.Type != TOKEN_WHITESPACE)
                .ToArray();
            
            var nodeStack = new Stack<AbstractNode>();
            
            foreach(var token in tokens)
            {
                switch(token.Type)
                {
                    case TOKEN_VALUE:
                        if(!nodeStack.Any() || !(nodeStack.Peek() is EqualityNode))
                        {
                            throw new ExpressionParseException(string.Format(
                                "Value at {0} without preceding equality operator.",
                                token.Position.Index));
                        }

                        // trim quotes
                        token.Value = token.Value.Substring(1, token.Value.Length - 2);
                        nodeStack.Peek().Right = new ValueNode(token);
                        LinkExpressions(nodeStack);
                        break;
                    case TOKEN_VARIABLE:
                        if(nodeStack.Any() && nodeStack.Peek() is EqualityNode)
                        {
                            nodeStack.Peek().Right = new VariableNode(token);
                            LinkExpressions(nodeStack);
                        }
                        else
                        {
                            nodeStack.Push(new VariableNode(token));
                        }
                        break;
                    case TOKEN_EQUALITY:
                        if (!nodeStack.Any() || !(nodeStack.Peek() is VariableNode))
                        {
                            throw new ExpressionParseException(string.Format(
                                "Equality operator at {0} without preceding variable.",
                                token.Position.Index));
                        }

                        var eqNode = new EqualityNode(token);
                        eqNode.Left = nodeStack.Pop();

                        nodeStack.Push(eqNode);

                        break;
                    case TOKEN_AND:
                        if (!nodeStack.Any() || !(nodeStack.Peek() is ExpressionNode))
                        {
                            throw new ExpressionParseException(string.Format(
                                "AND operator at {0} without preceding expression.",
                                token.Position.Index));
                        }
                        var andNode = new AndNode(token);
                        andNode.Left = nodeStack.Pop();
                        nodeStack.Push(andNode);
                        break;
                    case TOKEN_OR:
                        if (!nodeStack.Any() || !(nodeStack.Peek() is ExpressionNode))
                        {
                            throw new ExpressionParseException(string.Format(
                                "OR operator at {0} without preceding expression.",
                                token.Position.Index));
                        }
                        var orNode = new OrNode(token);
                        orNode.Left = nodeStack.Pop();
                        nodeStack.Push(orNode);
                        break;
                    case TOKEN_NOT:
                        if (nodeStack.Any() && !(nodeStack.Peek() is ExpressionNode))
                        {
                            throw new ExpressionParseException(string.Format(
                                "NOT operator at {0} with preceding token that isn't an expression.",
                                token.Position.Index));
                        }
                        var notNode = new NotNode(token);

                        if(nodeStack.Any())
                        {
                            nodeStack.Peek().Right = notNode;
                        }

                        nodeStack.Push(notNode);
                        break;
                }
            }

            return !nodeStack.Any() || nodeStack.Peek().Evaluate(variables);
        }

        private void LinkExpressions(Stack<AbstractNode> nodeStack)
        {
            var expressionNode = nodeStack.Pop();

            if(nodeStack.Any() && nodeStack.Peek() is ExpressionNode)
            {
                nodeStack.Peek().Right = expressionNode;
                LinkExpressions(nodeStack);
            }
            else
            {
                nodeStack.Push(expressionNode);
            }
        }

        private enum ParseState
        {
            Element,
            Equal,
            Null
        }
    }
}
