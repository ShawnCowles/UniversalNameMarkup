using System.Collections.Generic;
using UNM.Parser.SimpleLexer;

namespace UNM.GCS.Data.ExpressionNodes
{
    internal abstract class AbstractNode
    {
        internal Token Token { get; private set; }

        internal virtual AbstractNode Left { get; set; }

        internal virtual AbstractNode Right { get; set; }

        internal AbstractNode(Token token)
        {
            Token = token;
        }

        internal abstract bool Evaluate(Dictionary<string, string> variables);
    }
}
