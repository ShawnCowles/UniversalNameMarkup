using System.Collections.Generic;

namespace UNM.Parser
{
    /// <summary>
    /// A single token from a UNM pattern.
    /// </summary>
    public class PatternToken
    {
        /// <summary>
        /// The type of this PatternToken.
        /// </summary>
        public TokenType Type { get; set; }

        /// <summary>
        /// The value contained within this PatternToken.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The index in the source pattern where this token begins.
        /// </summary>
        public int SourceIndex { get; set; }

        /// <summary>
        /// Construct a new PatternToken.
        /// </summary>
        /// <param name="type">The <see cref="TokenType"/> of this token.</param>
        /// <param name="value">The value contained within this token.</param>
        /// <param name="sourceIndex">The index in the source pattern where this token begins.</param>
        public PatternToken(TokenType type, string value, int sourceIndex)
        {
            Type = type;
            Value = value;
            SourceIndex = sourceIndex;
        }
    }
}
