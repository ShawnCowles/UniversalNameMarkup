using System.Collections.Generic;

namespace UNM.Parser.ContextExpressions
{
    public interface IContextExpression
    {
        bool Matches(IEnumerable<string> context);
    }
}
