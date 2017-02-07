using System;
using System.Linq;
using UNM.GCS.Data;
using UNM.GCS.Interfaces;
using UNM.Parser;
using UNM.Parser.Implementation;
using UNM.Parser.Interfaces;

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
        /// <param name="input">The input set passed into the <see cref="IConversationSystem"/>.</param>
        /// <param name="output">The output set being returned from the <see cref="IConversationSystem"/>.</param>
        public void Process(InputSet input, OutputSet output)
        {
            var parameters = new PatternProcessingParameters(output.Response)
            {
                CapitalizationScheme = CapitalizationScheme.BY_SENTENCE,
                Variables = input.Variables,
                Context = input.Variables.Keys.ToArray()
            };

            output.Response = _nameParser.Process(parameters);
        }
    }
}
