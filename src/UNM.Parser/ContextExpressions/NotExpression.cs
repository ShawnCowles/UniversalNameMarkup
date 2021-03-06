﻿using System.Collections.Generic;

namespace UNM.Parser.ContextExpressions
{
    /// <summary>
    /// A context expression that returns the logical NOT of a sub expression.
    /// </summary>
    public class NotExpression : ParentExpression
    {
        
        /// <summary>
        /// Construct a new NotExpression.
        /// </summary>
        public NotExpression()
        {
        }

        /// <summary>
        /// Test if the provided context matches this expression.
        /// </summary>
        /// <param name="context">The context to used.</param>
        /// <returns>True if <paramref name="context"/> matches this expression.</returns>
        public override bool Matches(IEnumerable<string> context)
        {
            return !Child.Matches(context);
        }
    }
}
