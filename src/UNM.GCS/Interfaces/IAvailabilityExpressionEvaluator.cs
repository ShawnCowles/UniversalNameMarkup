using System.Collections.Generic;
using UNM.GCS.Data;

namespace UNM.GCS.Interfaces
{
    /// <summary>
    /// Processes availability expressions of <see cref="Response"/>s to determine which are available.
    /// </summary>
    public interface IAvailabilityExpressionEvaluator
    {
        /// <summary>
        /// Evaluate and availability expression.
        /// </summary>
        /// <param name="availabilityExpression">The expression to evaluate.</param>
        /// <param name="variables">The variables passed into the <see cref="IConversationSystem"/>.</param>
        /// <returns>True if the expression evaluates positively, false otherwise.</returns>
        bool Evaluate(string availabilityExpression, Dictionary<string, string> variables);
    }
}
