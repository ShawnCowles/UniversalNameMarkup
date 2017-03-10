using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using UNM.GCS.Data;
using UNM.GCS.Interfaces;

namespace UNM.GCS.Implementation
{
    /// <summary>
    /// An ITopicSource that loads topics from a .csv file via a stream.
    /// 
    /// The expected CSV format is straightforward, one response to a row. The first column 
    /// should be the name of the topic the response belongs to, the second column should be 
    /// the body of the response itself, the third column should be an AvailabilityExpression
    /// for the response (or empty), the fourth column should be the ResponseActionScript for 
    /// the response (or empty).
    /// 
    /// Topic, Response, AvailabilityExpression, ResponseActionScript
    /// </summary>
    public class CsvTopicSource : ITopicSource
    {
        private List<Topic> _topics;
        
        /// <summary>
        /// Construct a new CsvTopicSource and load topics.
        /// </summary>
        /// <param name="sourceStream">The stream to load topics from.</param>
        /// <param name="hiddenTopicNames">The names of topics which shouldn't be user visible.</param>
        public CsvTopicSource(Stream sourceStream, IEnumerable<string> hiddenTopicNames = null)
        {
            LoadTopicsFromStream(sourceStream, hiddenTopicNames);
        }

        private void LoadTopicsFromStream(Stream sourceStream, IEnumerable<string> hiddenTopicNames)
        {
            _topics = new List<Topic>();

            var responsesPerTopic = new Dictionary<string, List<Response>>();

            using (var reader = new CsvReader(new StreamReader(sourceStream)))
            {
                while (reader.Read())
                {
                    var topicName = reader.GetField(0);
                    var body = reader.GetField(1);
                    var availabilityExpression = reader.GetField(2);
                    var actionScript = reader.GetField(3);

                    if (!responsesPerTopic.ContainsKey(topicName))
                    {
                        responsesPerTopic.Add(topicName, new List<Response>());
                    }

                    responsesPerTopic[topicName].Add(new Response(
                        body,
                        availabilityExpression,
                        actionScript));
                }
                
                foreach (var topicName in responsesPerTopic.Keys)
                {
                    var hidden = hiddenTopicNames != null && hiddenTopicNames.Contains(topicName);

                    _topics.Add(new Topic(topicName, responsesPerTopic[topicName], !hidden));
                }
            }
        }

        /// <summary>
        /// Return the topics loaded from the .csv file.
        /// </summary>
        /// <returns>The topics loaded from the .csv file.</returns>
        public IEnumerable<Topic> GetTopics()
        {
            return _topics;
        }
    }
}
