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
    public class ContextExpressionTests
    {
        private readonly Fixture _fixture = new Fixture();

        private ContextExpressionParser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new ContextExpressionParser(new Lexer());
            _parser.Initialize();
        }
        [Test]
        public void Simple_logic_test_match()
        {
            var match = _fixture.Create<string>();

            var input = match;

            var expression = _parser.ParseExpression(input);

            Assert.True(expression.Matches(new[] { match }));

            Assert.False(expression.Matches(_fixture.CreateMany<string>()));
        }

        [Test]
        public void Simple_logic_test_not()
        {
            var match = _fixture.Create<string>();

            var input = "!" + match;

            var expression = _parser.ParseExpression(input);

            Assert.False(expression.Matches(new[] { match }));

            Assert.True(expression.Matches(_fixture.CreateMany<string>()));
        }

        [Test]
        public void Simple_logic_test_and()
        {
            var firstMatch = _fixture.Create<string>();
            var secondMatch = _fixture.Create<string>();

            var input = firstMatch + " && " + secondMatch;

            var expression = _parser.ParseExpression(input);

            Assert.False(expression.Matches(new[] { firstMatch }));
            Assert.False(expression.Matches(new[] { secondMatch }));
            Assert.False(expression.Matches(new[] { firstMatch, _fixture.Create<string>() }));
            Assert.False(expression.Matches(new[] { secondMatch, _fixture.Create<string>() }));
            Assert.False(expression.Matches(_fixture.CreateMany<string>()));
            Assert.True(expression.Matches(new[] { firstMatch, secondMatch }));
            Assert.True(expression.Matches(new[] { secondMatch, firstMatch }));
            Assert.True(expression.Matches(new[] { secondMatch, _fixture.Create<string>(), firstMatch }));
        }

        [Test]
        public void Simple_logic_test_or()
        {
            var firstMatch = _fixture.Create<string>();
            var secondMatch = _fixture.Create<string>();

            var input = firstMatch + " || " + secondMatch;

            var expression = _parser.ParseExpression(input);

            Assert.False(expression.Matches(_fixture.CreateMany<string>()));
            Assert.True(expression.Matches(new[] { firstMatch }));
            Assert.True(expression.Matches(new[] { secondMatch }));
            Assert.True(expression.Matches(new[] { firstMatch, _fixture.Create<string>() }));
            Assert.True(expression.Matches(new[] { secondMatch, _fixture.Create<string>() }));
            Assert.True(expression.Matches(new[] { firstMatch, secondMatch }));
            Assert.True(expression.Matches(new[] { secondMatch, firstMatch }));
            Assert.True(expression.Matches(new[] { secondMatch, _fixture.Create<string>(), firstMatch }));
        }

        [Test]
        public void Complex_logic_test()
        {
            var firstMatch = _fixture.Create<string>();
            var secondMatch = _fixture.Create<string>();
            var thirdMatch = _fixture.Create<string>();

            var input = string.Format("{0} && {1} || ! {2}",
                firstMatch, secondMatch, thirdMatch);

            var expression = _parser.ParseExpression(input);

            Assert.True(expression.Matches(_fixture.CreateMany<string>()));
            Assert.False(expression.Matches(new[] { thirdMatch }));
            Assert.False(expression.Matches(new[] { secondMatch, thirdMatch }));
            Assert.False(expression.Matches(new[] { firstMatch, thirdMatch }));
            Assert.True(expression.Matches(new[] { firstMatch, thirdMatch, secondMatch }));
        }
    }
}
