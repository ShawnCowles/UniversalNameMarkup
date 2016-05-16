using System.Collections.Generic;

namespace UNM.GCS.Interfaces
{
    /// <summary>
    /// A post processor that operates on conversation responses after selection.
    /// </summary>
    public interface IPostProcessor
    {
        /// <summary>
        /// Process response text.
        /// </summary>
        /// <param name="response">The text of the response.</param>
        /// <param name="variables">The variables passed into the <see cref="IConversationSystem"/>.</param>
        /// <returns>The processed text of the response.</returns>
        string Process(string response, Dictionary<string, string> variables);
    }
}
