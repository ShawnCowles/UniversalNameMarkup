using System.Collections.Generic;
using UNM.GCS.Data;

namespace UNM.GCS.Interfaces
{
    /// <summary>
    /// A source of topics and responses for the <see cref="IConversationSystem"/>.
    /// </summary>
    public interface ITopicSource
    {
        /// <summary>
        /// Return the topics from within this topic source.
        /// </summary>
        /// <returns>The topics in this topic source.</returns>
        IEnumerable<Topic> GetTopics();
    }
}
