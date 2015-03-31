using System;

namespace UNM.Parser
{
    /// <summary>
    /// An exception indicating that a UNM pattern has failed to parse properly.
    /// </summary>
    public class PatternParseException : Exception
    {
        /// <summary>
        /// Construct a new PatternParseException.
        /// </summary>
        /// <param name="message">Message explaining the exception.</param>
        public PatternParseException (string message)
            :base(message)
        {
        }

        /// <summary>
        /// Construct a new PatternParseException.
        /// </summary>
        /// <param name="message">Message explaining the exception.</param>
        /// <param name="innerException">The inner exception.</param>
        public PatternParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

