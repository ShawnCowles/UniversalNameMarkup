using System.Collections.Generic;
using UNM.Parser.SimpleLexer;

namespace UNM.GCS.Data.ExpressionNodes
{
    internal class OrNode : ExpressionNode
    {
        internal OrNode(Token token)
            :base(token)
        {
        }

        internal override bool Evaluate(Dictionary<string, string> variables)
        {
            if (Right == null)
            {
                throw new ExpressionParseException(string.Format(
                    "OR operator at {0} without preceding expression.",
                    Token.Position.Index));
            }

            if (Left == null)
            {
                throw new ExpressionParseException(string.Format(
                    "OR operator at {0} without following expression.",
                    Token.Position.Index));
            }

            return Right.Evaluate(variables) || Left.Evaluate(variables);
        }
    }
}
