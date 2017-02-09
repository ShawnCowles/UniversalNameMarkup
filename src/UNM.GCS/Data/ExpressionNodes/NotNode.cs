using System;
using System.Collections.Generic;
using UNM.Parser.SimpleLexer;

namespace UNM.GCS.Data.ExpressionNodes
{
    internal class NotNode : ExpressionNode
    {
        internal NotNode(Token token)
            :base(token)
        {
        }

        internal override AbstractNode Left
        {
            get
            {
                throw new NotImplementedException("NotNodes cannot have a left subtree.");
            }

            set
            {
                throw new NotImplementedException("NotNodes cannot have a left subtree.");
            }
        }

        internal override bool Evaluate(Dictionary<string, string> variables)
        {
            if (Right == null)
            {
                throw new ExpressionParseException(string.Format(
                    "OR operator at {0} without preceding expression.",
                    Token.Position.Index));
            }
            
            return ! Right.Evaluate(variables);
        }
    }
}
