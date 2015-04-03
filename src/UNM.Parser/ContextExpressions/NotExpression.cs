using System.Collections.Generic;
using System.Linq;

namespace UNM.Parser.ContextExpressions
{
    /// <summary>
    /// A context expression that returns the logical NOT of a sub expression.
    /// </summary>
    public class NotExpression : IContextExpression
    {
        private IContextExpression _subExpression;

        /// <summary>
        /// Construct a new NotExpression.
        /// </summary>
        /// <param name="subExpression">The sub expression to NOT.</param>
        public NotExpression(IContextExpression subExpression)
        {
            _subExpression = subExpression;
        }

        /// <summary>
        /// Test if the provided context matches this expression.
        /// </summary>
        /// <param name="context">The context to used.</param>
        /// <returns>True if <paramref name="context"/> matches this expression.</returns>
        public bool Matches(IEnumerable<string> context)
        {
            return !_subExpression.Matches(context);
        }
    }
}
