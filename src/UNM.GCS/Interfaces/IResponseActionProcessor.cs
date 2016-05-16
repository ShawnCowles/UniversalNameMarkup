using System.Collections.Generic;
using UNM.GCS.Data;

namespace UNM.GCS.Interfaces
{
    /// <summary>
    /// Processes the response action scripts of a chosen <see cref="Response"/>.
    /// </summary>
    public interface IResponseActionProcessor
    {
        /// <summary>
        /// Process a response action.
        /// </summary>
        /// <param name="responseActionScript">The response action script from the chosen <see cref="Response"/>.</param>
        /// <param name="variables">The variables passed into the <see cref="IConversationSystem"/>.</param>
        void Process(string responseActionScript, Dictionary<string, string> variables);
    }
}
