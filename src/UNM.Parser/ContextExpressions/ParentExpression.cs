using System.Collections.Generic;

namespace UNM.Parser.ContextExpressions
{
    /// <summary>
    /// An abstract base class for expressions that contain a child expression.
    /// </summary>
    public abstract class ParentExpression : IContextExpression
    {
        /// <summary>
        /// The child expression.
        /// </summary>
        public IContextExpression Child { get; set; }

        /// <summary>
        /// Test if the provided context matches this expression.
        /// </summary>
        /// <param name="context">The context to used.</param>
        /// <returns>True if <paramref name="context"/> matches this expression.</returns>
        public abstract bool Matches(IEnumerable<string> context);
    }
}
