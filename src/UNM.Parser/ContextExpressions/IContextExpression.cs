using System.Collections.Generic;

namespace UNM.Parser.ContextExpressions
{
    /// <summary>
    /// A context expression.
    /// </summary>
    public interface IContextExpression
    {
        /// <summary>
        /// Test if the provided context matches this expression.
        /// </summary>
        /// <param name="context">The context to used.</param>
        /// <returns>True if <paramref name="context"/> matches this expression.</returns>
        bool Matches(IEnumerable<string> context);
    }
}
