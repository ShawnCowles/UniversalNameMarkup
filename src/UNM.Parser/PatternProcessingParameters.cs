using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UNM.Parser
{
    /// <summary>
    /// A collection of parameters for processing UNM patterns.
    /// </summary>
    public class PatternProcessingParameters
    {
        /// <summary>
        /// The pattern to process.
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// The contexts to use to filter NameFragments.
        /// </summary>
        public IEnumerable<string> Context { get; set; }

        /// <summary>
        /// The variable values to use for external variables.
        /// </summary>
        public Dictionary<string, string> Variables { get; set; }

        /// <summary>
        /// The list of names to check against for uniqueness.
        /// </summary>
        public IEnumerable<string> UniqueCheck { get; set; }

        /// <summary>
        /// The capitalization scheme to use.
        /// </summary>
        public CapitalizationScheme CapitalizationScheme { get; set; }

        /// <summary>
        /// Construct a set of default pattern processing parameters.
        /// </summary>
        /// <param name="pattern">The pattern to process.</param>
        public PatternProcessingParameters(string pattern)
        {
            Pattern = pattern;

            CapitalizationScheme = CapitalizationScheme.NONE;

            Context = Enumerable.Empty<string>();

            Variables = new Dictionary<string, string>();

            UniqueCheck = Enumerable.Empty<string>();
        }
    }
}
