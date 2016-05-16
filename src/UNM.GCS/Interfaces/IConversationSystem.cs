using System.Collections.Generic;
using UNM.GCS.Data;

namespace UNM.GCS.Interfaces
{
    /// <summary>
    /// Interface for the GenericConversationSystem.
    /// </summary>
    public interface IConversationSystem
    {
        /// <summary>
        /// The response to give when no match can be found for the provided input.
        /// Defaults to "I don't know about that."
        /// </summary>
        string UnmatchedResponse { get; set; }

        /// <summary>
        /// All available topics of conversation for a given set of variables.
        /// <param name="variables">The relevant conversation variables, the same as would be 
        /// passed in an <see cref="InputSet"/>.</param>
        /// </summary>
        IEnumerable<string> AvailableTopics(Dictionary<string, string> variables);

        /// <summary>
        /// Process an input set, and return the result.
        /// </summary>
        /// <param name="input">The inputs for the conversation.</param>
        /// <returns>The result of the conversation.</returns>
        OutputSet Process(InputSet input);
    }
}
