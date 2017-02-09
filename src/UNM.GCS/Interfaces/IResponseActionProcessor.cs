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
        /// <param name="input">The input set passed into the <see cref="IConversationSystem"/>.</param>
        /// <param name="output">The output set being returned from the <see cref="IConversationSystem"/>.</param>
        /// <param name="responseActionScript">The response action script from the chosen <see cref="Response"/>.</param>
        void Process(InputSet input, OutputSet output, string responseActionScript);
    }
}
