using System.Collections.Generic;
using System.Linq;

namespace UNM.Parser.ContextExpressions
{
    /// <summary>
    /// A context expression that returns the logical OR of one or more sub expressions.
    /// </summary>
    public class OrExpression : IContextExpression
    {
        private IEnumerable<IContextExpression> _subExpressions;

        /// <summary>
        /// Construct a new OrExpression.
        /// </summary>
        /// <param name="subExpression">The sub expressions to AND together.</param>
        public OrExpression(IEnumerable<IContextExpression> subExpression)
        {
            _subExpressions = subExpression;
        }

        /// <summary>
        /// Test if the provided context matches this expression.
        /// </summary>
        /// <param name="context">The context to used.</param>
        /// <returns>True if <paramref name="context"/> matches this expression.</returns>
        public bool Matches(IEnumerable<string> context)
        {
            return _subExpressions.Any(e => e.Matches(context));
        }
    }
}
