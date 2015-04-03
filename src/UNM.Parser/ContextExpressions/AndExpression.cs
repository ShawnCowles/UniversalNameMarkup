using System.Collections.Generic;
using System.Linq;

namespace UNM.Parser.ContextExpressions
{
    /// <summary>
    /// A context expression that returns the logical AND of one or more sub expressions.
    /// </summary>
    public class AndExpression : NodeExpression
    {
        /// <summary>
        /// Construct a new AndExpression.
        /// </summary>
        public AndExpression()
        {
        }

        /// <summary>
        /// Test if the provided context matches this expression.
        /// </summary>
        /// <param name="context">The context to used.</param>
        /// <returns>True if <paramref name="context"/> matches this expression.</returns>
        public override bool Matches(IEnumerable<string> context)
        {
            return LeftChild.Matches(context) && RightChild.Matches(context);
        }
    }
}
