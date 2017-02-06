/*
 * Adapted from SimpleLexer by Drew Miller at: http://blogs.msdn.com/b/drew/archive/2009/12/31/a-simple-lexer-in-c-that-uses-regular-expressions.aspx
 * License unknown.
 */
namespace UNM.Parser.SimpleLexer
{
    /// <summary>
    /// Represents a token of the input.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// The type of this token.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The text matched from this token.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The position of the token within the input text.
        /// </summary>
        public TokenPosition Position { get; set; }

        /// <summary>
        /// Construct a new Token.
        /// </summary>
        /// <param name="type">The type of the Token.</param>
        /// <param name="value">The value matched for this Token.</param>
        /// <param name="position">The position of this Token within the input text.</param>
        public Token(string type, string value, TokenPosition position)
        {
            Type = type;
            Value = value;
            Position = position;
        }

        /// <summary>
        /// Return a string represetnation of this token.
        /// </summary>
        /// <returns>A string represetnation of this token.</returns>
        public override string ToString()
        {
            return string.Format(
                "Token: {{ Type: \"{0}\", Value: \"{1}\", {2} }}",
                Type,
                Value,
                Position.ToString());
        }

        /// <summary>
        /// Test for equality against another object.
        /// </summary>
        /// <param name="obj">The object to test for equality against.</param>
        /// <returns>True if <paramref name="obj"/> is a Token with identical Type, Value, and
        /// Position. False otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj != null && obj is Token)
            {
                var otherToken = (Token)obj;

                return Position.Equals(otherToken.Position)
                    && Type == otherToken.Type
                    && Value == otherToken.Value;
            }

            return false;
        }

        /// <summary>
        /// Get the hash code of this Token
        /// </summary>
        /// <returns>The hash code of this Token</returns>
        public override int GetHashCode()
        {
            return 593 ^ (Position.GetHashCode())
                + 593 ^ (Type.GetHashCode())
                + 593 ^ (Value.GetHashCode());
        }
    }
}
