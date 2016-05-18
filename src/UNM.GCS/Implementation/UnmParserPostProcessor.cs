using System;
using System.Collections.Generic;
using UNM.GCS.Interfaces;
using UNM.Parser;

namespace UNM.GCS.Implementation
{
    /// <summary>
    /// A post processor that runs the response text through a UNM <see cref="NameParser"/>.
    /// </summary>
    public class UnmParserPostProcessor : IPostProcessor
    {
        private NameParser _nameParser;

        /// <summary>
        /// Construct a new UnmParserPostProcessor.
        /// </summary>
        /// <param name="namelistSource">The namelist source to use for the <see cref="NameParser"/>.</param>
        public UnmParserPostProcessor(INamelistSource namelistSource)
        {
            _nameParser = new NameParser(namelistSource, DateTime.Now.Millisecond);

            _nameParser.Initialize();
        }

        /// <summary>
        /// Process response text.
        /// </summary>
        /// <param name="response">The text of the response.</param>
        /// <param name="variables">The variables passed into the <see cref="IConversationSystem"/>.</param>
        /// <returns>The processed text of the response.</returns>
        public string Process(string response, Dictionary<string, string> variables)
        {
            var parameters = new PatternProcessingParameters(response)
            {
                CapitalizationScheme = CapitalizationScheme.BY_SENTENCE,
                Variables = variables
            };

            return _nameParser.Process(parameters);
        }
    }
}
