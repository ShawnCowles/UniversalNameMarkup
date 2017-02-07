using System.Collections.Generic;
using System.Linq;
using UNM.GCS.Data;
using UNM.GCS.Interfaces;

namespace UNM.GCS.Implementation
{
    /// <summary>
    /// Implementation of <see cref="IConversationSystem"/>. This is the workhorse of GCS.
    /// </summary>
    public class ConversationSystem : IConversationSystem
    {
        private readonly IEnumerable<ITopicSource> _topicSources;
        private readonly IEnumerable<IAvailabilityExpressionEvaluator> _expressionEvaluators;
        private readonly IEnumerable<IPostProcessor> _postProcessors;
        private readonly IEnumerable<IResponseActionProcessor> _actionProcessors;

        /// <summary>
        /// The response to give when no match can be found for the provided input.
        /// Defaults to "I don't know about that."
        /// </summary>
        public string UnmatchedResponse { get; set; }
        
        /// <summary>
        /// Create a new ConversationSystem.
        /// </summary>
        /// <param name="topicSources">The topic sources to use.</param>
        /// <param name="expressionEvaluators">The expression evaluators to use.</param>
        /// <param name="postProcessors">The post processors to use.</param>
        /// <param name="actionProcessors">The action processors to use.</param>
        public ConversationSystem(
            IEnumerable<ITopicSource> topicSources,
            IEnumerable<IAvailabilityExpressionEvaluator> expressionEvaluators,
            IEnumerable<IPostProcessor> postProcessors,
            IEnumerable<IResponseActionProcessor> actionProcessors)
        {
            UnmatchedResponse = "I don't know about that.";
            _topicSources = topicSources;
            _expressionEvaluators = expressionEvaluators;
            _postProcessors = postProcessors;
            _actionProcessors = actionProcessors;
        }

        /// <summary>
        /// Process an input set, and return the result.
        /// </summary>
        /// <param name="input">The inputs for the conversation.</param>
        /// <returns>The result of the conversation.</returns>
        public OutputSet Process(InputSet input)
        {
            var topic = _topicSources
                .SelectMany(ts => ts.GetTopics())
                .FirstOrDefault(ts => ts.Name == input.Topic);
            
            Response chosenResponse = null;
            
            if(topic != null)
            {
                foreach(var response in topic.Responses)
                {
                    var matches = _expressionEvaluators
                        .Any(e => e.Evaluate(response.AvailabilityExpression, input.Variables));

                    if(matches)
                    {
                        chosenResponse = response;
                        break;
                    }
                }

            }

            var output = new OutputSet();

            if (chosenResponse != null)
            {
                output.Response = chosenResponse.Body;

                foreach(var postProcessor in _postProcessors)
                {
                    postProcessor.Process(input, output);
                }

                foreach(var processor in _actionProcessors)
                {
                    processor.Process(input, output, chosenResponse.ResponseActionScript);
                }
            }
            else
            {
                output.Response = UnmatchedResponse;
            }

            return output;
        }

        /// <summary>
        /// All available topics of conversation for a given set of variables.
        /// <param name="variables">The relevant conversation variables, the same as would be 
        /// passed in an <see cref="InputSet"/>.</param>
        /// </summary>
        public IEnumerable<string> AvailableTopics(Dictionary<string, string> variables)
        {
            return _topicSources
                .SelectMany(ts => ts.GetTopics())
                .Where(t => t.Responses
                    .Any(r => _expressionEvaluators
                        .Any(e => e.Evaluate(r.AvailabilityExpression, variables))))
                .Select(t => t.Name)
                .ToArray();
        }
    }
}
