using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using UNM.Parser.ContextExpressions;
using UNM.Parser.SimpleLexer;

namespace UNM.Parser
{
    [TestFixture]
    public class ContextExpressionParserTest
    {
        private readonly Fixture _fixture = new Fixture();

        [Test]
        public void Initialize_loads_token_definitions_into_internal_lexer()
        {
            var mockLexer = new Mock<ILexer>();

            var parser = new ContextExpressionParser(mockLexer.Object);

            parser.Initialize();

            mockLexer.Verify(x => x.AddDefinition(It.IsAny<TokenDefinition>()));
        }

        [Test]
        public void ParseExpression_rethrows_exceptions_generated_by_internal_lexer()
        {
            var mockLexer = new Mock<ILexer>();
            mockLexer
                .Setup(x => x.Tokenize(It.IsAny<string>()))
                .Throws(new Exception());

            var parser = new ContextExpressionParser(mockLexer.Object);

            parser.Initialize();

            Assert.Throws<ExpressionParseException>(
                () => parser.ParseExpression(_fixture.Create<string>()));
        }

        [Test]
        public void ParseExpression_returns_EmptyExpression_for_zero_length_string()
        {
            var parser = new ContextExpressionParser(new Lexer());

            var result = parser.ParseExpression("");

            Assert.That(result, Is.TypeOf<EmptyExpression>());
        }

        [Test]
        public void ParseExpression_returns_MatchExpression_for_match_expression()
        {
            var match = _fixture.Create<string>();

            var parser = new ContextExpressionParser(new Lexer());

            var result = parser.ParseExpression(match);

            Assert.That(result, Is.TypeOf<MatchExpression>());

            Assert.True(result.Matches(new [] {match}));

            Assert.False(result.Matches(new [] {_fixture.Create<string>()}));
        }

        [Test]
        public void ParseExpression_returns_proper_result_for_simple_expression()
        {
            var firstMatch = _fixture.Create<string>();
            var secondMatch = _fixture.Create<string>();
            var thirdMatch = _fixture.Create<string>();

            var inputExpression = string.Format("{0} && {1} || ! {2}",
                firstMatch, secondMatch, thirdMatch);

            var parser = new ContextExpressionParser(new Lexer());

            var result = parser.ParseExpression(inputExpression);


            Assert.True(result.Matches(new[] { firstMatch }));

            Assert.False(result.Matches(new[] { firstMatch, thirdMatch }));

            Assert.False(result.Matches(new[] { thirdMatch }));

            Assert.False(result.Matches(new[] { secondMatch }));

            Assert.False(result.Matches(new[] { secondMatch, thirdMatch }));
        }
    }
}
