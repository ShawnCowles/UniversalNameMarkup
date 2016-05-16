using System.Collections.Generic;

namespace UNM.GCS.Data
{
    /// <summary>
    /// A collection of outputs from processing a conversation topic.
    /// </summary>
    public class OutputSet
    {
        /// <summary>
        /// The text body of the response.
        /// </summary>
        public string Response { get; set; }
        
        /// <summary>
        /// Any non-response notifications, typically from response actions.
        /// </summary>
        public List<string> Notifications { get; set; }
    }
}
