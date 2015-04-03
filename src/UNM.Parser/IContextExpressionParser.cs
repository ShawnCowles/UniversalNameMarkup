using UNM.Parser.ContextExpressions;

namespace UNM.Parser
{
    /// <summary>
    /// Parses a context expression into an expression tree.
    /// </summary>
    public interface IContextExpressionParser
    {
        /// <summary>
        /// Parse a context expression into an expression tree.
        /// </summary>
        /// <param name="expression">The context expression as a string.</param>
        /// <returns>The expression tree representation of <paramref name="expression"/>.</returns>
        IContextExpression ParseExpression(string expression);
    }
}
