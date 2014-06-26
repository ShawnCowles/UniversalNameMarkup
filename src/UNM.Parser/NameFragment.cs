using System;
using System.Collections.Generic;
using System.Linq;

namespace UNM.Parser
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

		private IEnumerable<string> _contexts;
		
        /// <summary>
        /// Contstruct a new NameFragment.
        /// </summary>
        /// <param name="fragment">The value of the fragment.</param>
        /// <param name="contexts">The contexts applicable to the fragment.</param>
		public NameFragment (string fragment, IEnumerable<string> contexts)
		{
			Fragment = fragment;
			_contexts = contexts;
		}
		
        /// <summary>
        /// Check the fragment against a list of contexts.
        /// </summary>
        /// <param name="contextsToMatch">The contexts to check against.</param>
        /// <returns>True if the fragment matches any context out of <paramref name="contextsToMatch"/></returns>
		public bool MatchesContexts(IEnumerable<string> contextsToMatch)
		{
            if (!_contexts.Any())
            {
                return true;
            }

            return contextsToMatch.Intersect(_contexts).Any();
		}
	}
}