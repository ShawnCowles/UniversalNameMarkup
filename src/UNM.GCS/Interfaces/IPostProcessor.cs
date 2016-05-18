using UNM.GCS.Data;

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
        /// <param name="input">The input set passed into the <see cref="IConversationSystem"/>.</param>
        /// <param name="output">The output set being returned from the <see cref="IConversationSystem"/>.</param>
        void Process(InputSet input, OutputSet output);
    }
}
