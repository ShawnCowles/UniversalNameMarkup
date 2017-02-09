using UNM.Parser.SimpleLexer;

namespace UNM.GCS.Data.ExpressionNodes
{
    internal abstract class ExpressionNode : AbstractNode
    {
        internal ExpressionNode(Token token)
            :base(token)
        {
        }
    }
}
