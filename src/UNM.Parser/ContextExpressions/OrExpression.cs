using System.Collections.Generic;

namespace UNM.Parser.ContextExpressions
{
    /// <summary>
    /// A context expression that returns the logical OR of one or more sub expressions.
    /// </summary>
    public class OrExpression : NodeExpression
    {
        /// <summary>
        /// Construct a new OrExpression.
        /// </summary>
        public OrExpression()
        {
        }

        /// <summary>
        /// Test if the provided context matches this expression.
        /// </summary>
        /// <param name="context">The context to used.</param>
        /// <returns>True if <paramref name="context"/> matches this expression.</returns>
        public override bool Matches(IEnumerable<string> context)
        {
            return LeftChild.Matches(context) || RightChild.Matches(context);
        }
    }
}
