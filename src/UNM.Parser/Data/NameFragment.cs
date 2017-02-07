using System.Collections.Generic;
using UNM.Parser.ContextExpressions;

namespace UNM.Parser.Data
{
    /// <summary>
    /// A fragment to substitute into a pattern.
    /// </summary>
    public class NameFragment
    {
        /// <summary>
        /// The value of the fragment.
        /// </summary>
        public string Fragment { get; private set; }

        private IContextExpression _contextExpression;
        
        /// <summary>
        /// Constructs a new NameFragment.
        /// </summary>
        /// <param name="fragment">The value of the fragment.</param>
        /// <param name="contextExpression">The context expression for this fragment.</param>
        public NameFragment (string fragment, IContextExpression contextExpression)
        {
            Fragment = fragment;
            _contextExpression = contextExpression;
        }
        
        /// <summary>
        /// Check the fragment against a list of contexts.
        /// </summary>
        /// <param name="contextsToMatch">The contexts to check against.</param>
        /// <returns>True if the fragment matches any context out of <paramref name="contextsToMatch"/></returns>
        public bool MatchesContexts(IEnumerable<string> contextsToMatch)
        {
            return _contextExpression.Matches(contextsToMatch);
        }
    }
}