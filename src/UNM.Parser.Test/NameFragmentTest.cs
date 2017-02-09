using NUnit.Framework;
using Ploeh.AutoFixture;
using Moq;
using UNM.Parser.ContextExpressions;
using UNM.Parser.Data;

namespace UNM.Parser
{
    [TestFixture]
    public class NameFragmentTest
    {
        private Fixture _fixture = new Fixture();

        [Test]
        public void MatchesContexts_delegates_to_the_internal_context_expression()
        {
            var mockExpression = new Mock<IContextExpression>();

            var fragment = new NameFragment(_fixture.Create<string>(), mockExpression.Object);

            var contexts = _fixture.CreateMany<string>();

            var result = fragment.MatchesContexts(contexts);

            mockExpression.Verify(x => x.Matches(contexts));
        }
    }
}
