using System.Collections.Generic;
using UNM.Parser.SimpleLexer;

namespace UNM.GCS.Data.ExpressionNodes
{
    internal class EqualityNode : ExpressionNode
    {
        internal EqualityNode(Token token)
            :base(token)
        {
        }

        internal override bool Evaluate(Dictionary<string, string> variables)
        {
            if(Right == null)
            {
                throw new ExpressionParseException(string.Format(
                    "Equality operator at {0} without preceding variable.",
                    Token.Position.Index));
            }

            if (Left == null)
            {
                throw new ExpressionParseException(string.Format(
                    "Equality operator at {0} without following variable or value.",
                    Token.Position.Index));
            }

            var left = "";
            
            if(Left is VariableNode)
            {
                var leftKey = Left.Token.Value;

                if(variables.ContainsKey(leftKey))
                {
                    left = variables[leftKey];
                }
                else
                {
                    throw new ExpressionParseException(string.Format(
                        "Expected variable {0}, but it was not found.",
                        leftKey));
                }

                if(Right is ValueNode)
                {
                    var right = Right.Token.Value;

                    return left == right;
                }
                else if( Right is VariableNode)
                {
                    var rightKey = Right.Token.Value;

                    if (variables.ContainsKey(rightKey))
                    {
                        return left == variables[rightKey];
                    }
                    else
                    {
                        throw new ExpressionParseException(string.Format(
                            "Expected variable {0}, but it was not found.",
                            rightKey));
                    }
                }
                else
                {
                    throw new ExpressionParseException(string.Format(
                        "Variable expected at index {0} but {1} found instead.",
                        Right.Token.Position.Index,
                        Right.Token.Type));
                }
            }
            else
            {
                throw new ExpressionParseException(string.Format(
                        "Variable expected at index {0} but {1} found instead.",
                        Left.Token.Position.Index,
                        Left.Token.Type));
            }
        }
    }
}
