using System.Text.RegularExpressions;

/*
 * Adapted from SimpleLexer by Drew Miller at: http://blogs.msdn.com/b/drew/archive/2009/12/31/a-simple-lexer-in-c-that-uses-regular-expressions.aspx
 * License unknown.
 */
namespace UNM.Parser.SimpleLexer
{
    /// <summary>
    /// A regular expression based definition of a token.
    /// </summary>
    public class TokenDefinition
    {
        /// <summary>
        /// The type of the token.
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// The regular expression pattern defining the token.
        /// </summary>
        public Regex Regex { get; private set; }

        /// <summary>
        /// If true, this token will not be returend from the lexer, but will still be matched against.
        /// </summary>
        public bool IsIgnored { get; private set; }
        
        /// <summary>
        /// Construct a new TokenDefinition
        /// </summary>
        /// <param name="type">The type of the token.</param>
        /// <param name="regex">The regular expression pattern defining the token.</param>
        /// <param name="isIgnored">If set to true, this token will not be returned from the lexer.</param>
        public TokenDefinition(
            string type,
            Regex regex,
            bool isIgnored = false)
        {
            Type = type;
            Regex = regex;
            IsIgnored = isIgnored;
        }
    }
}
