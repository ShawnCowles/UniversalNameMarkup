using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace UNM.Parser.SimpleLexer
{
    [TestFixture]
    public class LexerTest
    {
        [Test]
        public void Tokenize_produces_expected_tokens()
        {
            const string testPattern = "1 * 2 / 3 + 4 - 5";

            var lexer = new Lexer();

            lexer.AddDefinition(new TokenDefinition(
                "(operator)",
                new Regex(@"\*|\/|\+|\-")));

            lexer.AddDefinition(new TokenDefinition(
                "(literal)",
                new Regex(@"\d+")));


            lexer.AddDefinition(new TokenDefinition(
                "(white-space)",
                new Regex(@"\s+"),
                true));


            var expectedTokens = new Token[]
            {
                new Token("(literal)", "1", new TokenPosition(0,1,0)),
                new Token("(operator)", "*", new TokenPosition(2,1,2)),
                new Token("(literal)", "2", new TokenPosition(4,1,4)),
                new Token("(operator)", "/", new TokenPosition(6,1,6)),
                new Token("(literal)", "3", new TokenPosition(8,1,8)),
                new Token("(operator)", "+", new TokenPosition(10,1,10)),
                new Token("(literal)", "4", new TokenPosition(12,1,12)),
                new Token("(operator)", "-", new TokenPosition(14,1,14)),
                new Token("(literal)", "5", new TokenPosition(16,1,16)),
                new Token("(end)", null, new TokenPosition(17,1,17)),
            };

            var tokens = lexer.Tokenize(testPattern);

            for (int i = 0; i < tokens.Count(); i++)
            {
                Assert.That(tokens.ElementAt(i), Is.EqualTo(expectedTokens[i]));
            }

            Assert.That(tokens.SequenceEqual(expectedTokens));
        }
    }
}
