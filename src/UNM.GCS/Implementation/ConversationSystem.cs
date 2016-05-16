using System;
using System.Collections.Generic;
using UNM.GCS.Data;
using UNM.GCS.Interfaces;

namespace UNM.GCS.Implementation
{
    /// <summary>
    /// Implementation of <see cref="IConversationSystem"/>. This is the workhorse of GCS.
    /// </summary>
    public class ConversationSystem : IConversationSystem
    {
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
        }

        /// <summary>
        /// Process an input set, and return the result.
        /// </summary>
        /// <param name="input">The inputs for the conversation.</param>
        /// <returns>The result of the conversation.</returns>
        public OutputSet Process(InputSet input)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// All available topics of conversation for a given set of variables.
        /// <param name="variables">The relevant conversation variables, the same as would be 
        /// passed in an <see cref="InputSet"/>.</param>
        /// </summary>
        public IEnumerable<string> AvailableTopics(Dictionary<string, string> variables)
        {
            throw new NotImplementedException();
        }
    }
}
