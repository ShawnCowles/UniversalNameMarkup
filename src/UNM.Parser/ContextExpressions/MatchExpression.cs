using System.Collections.Generic;
using System.Linq;

namespace UNM.Parser.ContextExpressions
{
    /// <summary>
    /// A context expression that returns true if the context contains a specific string.
    /// </summary>
    public class MatchExpression : IContextExpression
    {
        private string _match;

        /// <summary>
        /// Construct a new MatchExpression.
        /// </summary>
        /// <param name="match">The string to search for.</param>
        public MatchExpression(string match)
        {
            _match = match;
        }

        /// <summary>
        /// Test if the provided context matches this expression.
        /// </summary>
        /// <param name="context">The context to used.</param>
        /// <returns>True if <paramref name="context"/> matches this expression.</returns>
        public bool Matches(IEnumerable<string> context)
        {
            return context.Contains(_match);
        }
    }
}
