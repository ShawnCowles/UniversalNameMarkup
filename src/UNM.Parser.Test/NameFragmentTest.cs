using System.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace UNM.Parser
{
    [TestFixture]
    public class NameFragmentTest
    {
        private Fixture _fixture = new Fixture();

        [Test]
        public void MatchesContexts_returns_true_if_fragment_has_no_contexts()
        {
            var fragment = new NameFragment(_fixture.Create<string>(), new string[0]);

            var contexts = _fixture.CreateMany<string>();

            var result = fragment.MatchesContexts(contexts);

            Assert.True(result);
        }

        [Test]
        public void MatchesContexts_returns_true_if_fragment_has_one_of_provided_contexts()
        {
            var fragmentContexts = _fixture.CreateMany<string>();

            var fragment = new NameFragment(_fixture.Create<string>(), fragmentContexts);

            var contexts = _fixture.CreateMany<string>();

            contexts = contexts.Concat(new string[] { fragmentContexts.Last()});

            var result = fragment.MatchesContexts(contexts);

            Assert.True(result);
        }

        [Test]
        public void MatchesContexts_returns_false_if_fragment_has_none_of_provided_contexts()
        {
            var fragmentContexts = _fixture.CreateMany<string>();

            var fragment = new NameFragment(_fixture.Create<string>(), fragmentContexts);

            var contexts = _fixture.CreateMany<string>();

            var result = fragment.MatchesContexts(contexts);

            Assert.False(result);
        }
    }
}
