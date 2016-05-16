using System.Collections.Generic;
using UNM.GCS.Interfaces;

namespace UNM.GCS.Data
{
    /// <summary>
    /// A collection of responses. When this topic is passed through the 
    /// <see cref="IConversationSystem"/> one of its responses will be chosen.
    /// </summary>
    public class Topic
    {
        /// <summary>
        /// The name of the topic, used to select it in conversation.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The responses within this topic, in priority order.
        /// </summary>
        public IEnumerable<Response> Responses { get; private set; }

        /// <summary>
        /// Construct a new topic.
        /// </summary>
        /// <param name="name">The name of the topic, used to select it in conversation.</param>
        /// <param name="responses">The responses within this topic, in priority order.</param>
        public Topic(string name, IEnumerable<Response> responses)
        {
            Name = name;
            Responses = responses;
        }
    }
}
