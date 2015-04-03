using System;

namespace UNM.Parser
{
    /// <summary>
    /// An exception indicating that a context expression has failed to parse properly.
    /// </summary>
    public class ExpressionParseException : Exception
    {
        /// <summary>
        /// Construct a new ExpressionParseException.
        /// </summary>
        /// <param name="message">Message explaining the exception.</param>
        public ExpressionParseException(string message)
            :base(message)
        {
        }

        /// <summary>
        /// Construct a new ExpressionParseException.
        /// </summary>
        /// <param name="message">Message explaining the exception.</param>
        /// <param name="innerException">The inner exception.</param>
        public ExpressionParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
