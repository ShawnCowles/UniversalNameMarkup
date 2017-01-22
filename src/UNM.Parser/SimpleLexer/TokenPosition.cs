/*
 * Adapted from SimpleLexer by Drew Miller at: http://blogs.msdn.com/b/drew/archive/2009/12/31/a-simple-lexer-in-c-that-uses-regular-expressions.aspx
 * License unknown.
 */
namespace UNM.Parser.SimpleLexer
{
    /// <summary>
    /// Represents the position of a 
    /// </summary>
    public class TokenPosition
    {
        /// <summary>
        /// Index in the input string where this token begins.
        /// </summary>
        public int Index { get; private set; }
        
        /// <summary>
        /// Line of the input string where this token begins.
        /// </summary>
        public int Line { get; private set; }

        /// <summary>
        /// Column of the line in the input string where this token begins.
        /// </summary>
        public int Column { get; private set; }

        /// <summary>
        /// Construct a new TokenPosition.
        /// </summary>
        /// <param name="index">Index in the input string where this token begins.</param>
        /// <param name="line">Line of the input string where this token begins.</param>
        /// <param name="column">Column of the line in the input string where this token begins.</param>
        public TokenPosition(int index, int line, int column)
        {
            Index = index;
            Line = line;
            Column = column;
        }

        /// <summary>
        /// Return a string representation of this <see cref="TokenPosition"/>.
        /// </summary>
        /// <returns>A string representation of this <see cref="TokenPosition"/></returns>
        public override string ToString()
        {
            return string.Format(
                "Position: {{ Index: \"{0}\", Line: \"{1}\", Column: \"{2}\" }}",
                Index,
                Line,
                Column);
        }

        /// <summary>
        /// Test for equality against another object.
        /// </summary>
        /// <param name="obj">The object to test for equality against.</param>
        /// <returns>True if <paramref name="obj"/> is a TokenPosition with identical Index, Line,
        /// and Column. False otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj != null && obj is TokenPosition)
            {
                var otherPos = (TokenPosition)obj;

                return Index == otherPos.Index
                    && Line == otherPos.Line
                    && Column == otherPos.Column;
            }

            return false;
        }

        /// <summary>
        /// Get a hash code for this TokenPosition.
        /// </summary>
        /// <returns>The hash code for this TokenPosition</returns>
        public override int GetHashCode()
        {
            return 593 ^ (Index.GetHashCode())
                + 593 ^ (Line.GetHashCode())
                + 593 ^ (Column.GetHashCode());
        }

        /// <summary>
        /// Operator overload for equality.
        /// </summary>
        /// <param name="a">The right side of the equality test.</param>
        /// <param name="b">The left side of the equality test.</param>
        /// <returns>True if <paramref name="a"/> and <paramref name="b"/> have the same Line,
        /// Column, and Index values.</returns>
        public static bool operator ==(TokenPosition a, TokenPosition b)
        {
            if(ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return ReferenceEquals(a, b);
            }

            return a.Equals(b);
        }

        /// <summary>
        /// Operator overload for not-equals.
        /// </summary>
        /// <param name="a">The right side of the not-equals test.</param>
        /// <param name="b">The left side of the not-equals test.</param>
        /// <returns>True if <paramref name="a"/> and <paramref name="b"/> have a different Line,
        /// Column, or Index value.</returns>
        public static bool operator !=(TokenPosition a, TokenPosition b)
        {
            return !(a == b);
        }
    }
}
