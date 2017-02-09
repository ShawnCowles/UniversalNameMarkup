using System.Collections.Generic;

namespace UNM.GCS.Data
{
    /// <summary>
    /// A collection of parameters for processing a conversation topic.
    /// </summary>
    public class InputSet
    {
        /// <summary>
        /// The name of the topic being asked about.
        /// </summary>
        public string Topic { get; set; }
        
        /// <summary>
        /// Any relevant variables to the conversation, things like location, speaker race, 
        /// the speaker's disposition.
        /// </summary>
        public Dictionary<string, string> Variables { get; set; }
    }
}
