using System;

namespace UNM.GCS
{
    /// <summary>
    /// Exception for failures in parsing an availability expression.
    /// </summary>
    public class ExpressionParseException : Exception
    {
        /// <summary>
        /// Create a new ExpressionParseException.
        /// </summary>
        /// <param name="message"></param>
        public ExpressionParseException(string message)
            : base(message)
        {
        }
    }
}
