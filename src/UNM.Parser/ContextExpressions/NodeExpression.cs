using System.Collections.Generic;

namespace UNM.Parser.ContextExpressions
{
    /// <summary>
    /// An abstract base class for expressions that contain left and right sub expressions.
    /// </summary>
    public abstract class NodeExpression : IContextExpression
    {
        /// <summary>
        /// The left sub expression.
        /// </summary>
        public IContextExpression LeftChild { get; set; }

        /// <summary>
        /// The right sub expression.
        /// </summary>
        public IContextExpression RightChild { get; set; }

        /// <summary>
        /// Construct a new NodeExpression.
        /// </summary>
        protected NodeExpression()
        {
        }

        /// <summary>
        /// Test if the provided context matches this expression.
        /// </summary>
        /// <param name="context">The context to used.</param>
        /// <returns>True if <paramref name="context"/> matches this expression.</returns>
        public abstract bool Matches(IEnumerable<string> context);
    }
}
