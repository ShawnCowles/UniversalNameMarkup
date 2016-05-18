using UNM.GCS.Interfaces;

namespace UNM.GCS.Data
{
    /// <summary>
    /// A response that can be delivered by the <see cref="IConversationSystem"/>. Contains the 
    /// body of the response, an expression to determine it's availability, and a script to run if
    /// this response is chosen.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// The text body of the response.
        /// </summary>
        public string Body { get; private set; }

        /// <summary>
        /// An expression that can be evaluated to determine if the response is avilaible. Needs 
        /// and example.
        /// </summary>
        public string AvailabilityExpression { get; private set; }

        /// <summary>
        /// A script to execute through a <see cref="IResponseActionProcessor"/> if this response 
        /// is chosen.
        /// </summary>
        public string ResponseActionScript { get; private set; }

        /// <summary>
        /// Construct a new Response.
        /// </summary>
        /// <param name="body">The text body of the response.</param>
        /// <param name="availabilityExpression">An expression that can be evaluated to determine if the response is avilaible. Needs and example.</param>
        /// <param name="responseActionScript">A script to execute through a <see cref="IResponseActionProcessor"/> if this response is chosen.</param>
        public Response(string body, string availabilityExpression, string responseActionScript)
        {
            Body = body;
            AvailabilityExpression = availabilityExpression;
            ResponseActionScript = responseActionScript;
        }
    }
}