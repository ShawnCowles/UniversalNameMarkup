using UNM.Parser.ContextExpressions;

namespace UNM.Parser.Interfaces
{
    /// <summary>
    /// Parses a context expression into an expression tree.
    /// </summary>
    public interface IContextExpressionParser
    {
        /// <summary>
        /// Perform any needed initialization steps.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Parse a context expression into an expression tree.
        /// </summary>
        /// <param name="expression">The context expression as a string.</param>
        /// <returns>The expression tree representation of <paramref name="expression"/>.</returns>
        IContextExpression ParseExpression(string expression);
    }
}
