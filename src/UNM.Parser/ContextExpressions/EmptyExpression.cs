using System.Collections.Generic;

namespace UNM.Parser.ContextExpressions
{
    /// <summary>
    /// A context expression that matches any context.
    /// </summary>
    public class EmptyExpression : IContextExpression
    {
        /// <summary>
        /// Test if the provided context matches this expression.
        /// </summary>
        /// <param name="context">The context to used.</param>
        /// <returns>True if <paramref name="context"/> matches this expression.</returns>S
        public bool Matches(IEnumerable<string> context)
        {
            return true;
        }
    }
}
