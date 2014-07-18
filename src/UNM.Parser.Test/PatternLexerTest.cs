using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using UNM.Parser.SimpleLexer;

namespace UNM.Parser
{
    [TestFixture]
    public class PatternLexerTest
    {
        private Fixture _fixture = new Fixture();

        [Test]
        public void Initialize_loads_token_definitions_into_internal_lexer()
        {
            var mockLexer = new Mock<ILexer>();

            var patternLexer = new PatternLexer(mockLexer.Object);

            patternLexer.Initialize();

            mockLexer.Verify(x => x.AddDefinition(It.IsAny<TokenDefinition>()));
        }

        [Test]
        public void Process_rethrows_exceptions_generated_by_internal_lexer()
        {
            var mockLexer = new Mock<ILexer>();
            mockLexer
                .Setup(x => x.Tokenize(It.IsAny<string>()))
                .Throws(new Exception());

            var patternLexer = new PatternLexer(mockLexer.Object);

            patternLexer.Initialize();

            Assert.Throws<PatternParseException>(
                () => patternLexer.Process(_fixture.Create<string>()));
        }

        [Test]
        public void Process_returns_tokens_from_internal_lexer_without_end_token()
        {
            var tokenValues = _fixture.CreateMany<string>();

            var rawTokens = new List<Token>();

            foreach (var value in tokenValues)
            {
                rawTokens.Add(new Token(
                    _fixture.Create<TokenType>().ToString(),
                    value,
                    _fixture.Create<TokenPosition>()));
            }

            rawTokens.Add(new Token("(end)", null, _fixture.Create<TokenPosition>()));

            var testPattern = _fixture.Create<string>();

            var mockLexer = new Mock<ILexer>();

            mockLexer
                .Setup(x => x.Tokenize(testPattern))
                .Returns(rawTokens);

            var patternLexer = new PatternLexer(mockLexer.Object);
            patternLexer.Initialize();

            var results = patternLexer.Process(testPattern).ToArray();

            Assert.That(results.Count(), Is.EqualTo(rawTokens.Count() - 1));

            for (int i = 0; i < rawTokens.Count - 1; i++)
            {
                var raw = rawTokens[i];
                var processed = results[i];

                Assert.That(processed.Type.ToString(), Is.EqualTo(raw.Type));
                Assert.That(processed.Value, Is.EqualTo(raw.Value));
                Assert.That(processed.SourceIndex, Is.EqualTo(raw.Position.Index));
            }
        }

        #region Token defnition tests
        [Test]
        public void Basic_definition_test()
        {
            var testPattern = "<sub_fragment><#sub_variable><%12><@branch-context>"
                + "<$branch_Variable>|{this is some$contentwith!$oDDcharacterz}<^sub_pattern>";
            
            var patternLexer = new PatternLexer(new Lexer());
            patternLexer.Initialize();

            var results = patternLexer.Process(testPattern).ToArray();

            Assert.That(results.Count(), Is.EqualTo(10));

            Assert.That(results[0].Type, Is.EqualTo(TokenType.TAG_SUB_FRAGMENT));
            Assert.That(results[0].Value, Is.EqualTo("<sub_fragment>"));
            Assert.That(results[1].Type, Is.EqualTo(TokenType.TAG_SUB_VARIABLE));
            Assert.That(results[1].Value, Is.EqualTo("<#sub_variable>"));
            Assert.That(results[2].Type, Is.EqualTo(TokenType.TAG_BRANCH_CHANCE));
            Assert.That(results[2].Value, Is.EqualTo("<%12>"));
            Assert.That(results[3].Type, Is.EqualTo(TokenType.TAG_BRANCH_CONTEXT));
            Assert.That(results[3].Value, Is.EqualTo("<@branch-context>"));
            Assert.That(results[4].Type, Is.EqualTo(TokenType.TAG_BRANCH_VARIABLE));
            Assert.That(results[4].Value, Is.EqualTo("<$branch_Variable>"));
            Assert.That(results[5].Type, Is.EqualTo(TokenType.TAG_ELSE));
            Assert.That(results[5].Value, Is.EqualTo("|"));
            Assert.That(results[6].Type, Is.EqualTo(TokenType.BRANCH_START));
            Assert.That(results[6].Value, Is.EqualTo("{"));
            Assert.That(results[7].Type, Is.EqualTo(TokenType.CONTENT));
            Assert.That(results[7].Value, Is.EqualTo("this is some$contentwith!$oDDcharacterz"));
            Assert.That(results[8].Type, Is.EqualTo(TokenType.BRANCH_END));
            Assert.That(results[8].Value, Is.EqualTo("}"));
            Assert.That(results[9].Type, Is.EqualTo(TokenType.TAG_SUB_PATTERN));
            Assert.That(results[9].Value, Is.EqualTo("<^sub_pattern>"));
        }

        [Test]
        public void Somewhat_complex_pattern()
        {
            var testPattern = "front<%50>{middle}end";

            var patternLexer = new PatternLexer(new Lexer());
            patternLexer.Initialize();

            var results = patternLexer.Process(testPattern).ToArray();

            Assert.That(results.Count(), Is.EqualTo(6));

            Assert.That(results[0].Type, Is.EqualTo(TokenType.CONTENT));
            Assert.That(results[0].Value, Is.EqualTo("front"));
            Assert.That(results[1].Type, Is.EqualTo(TokenType.TAG_BRANCH_CHANCE));
            Assert.That(results[1].Value, Is.EqualTo("<%50>"));
            Assert.That(results[2].Type, Is.EqualTo(TokenType.BRANCH_START));
            Assert.That(results[2].Value, Is.EqualTo("{"));
            Assert.That(results[3].Type, Is.EqualTo(TokenType.CONTENT));
            Assert.That(results[3].Value, Is.EqualTo("middle"));
            Assert.That(results[4].Type, Is.EqualTo(TokenType.BRANCH_END));
            Assert.That(results[4].Value, Is.EqualTo("}"));
            Assert.That(results[5].Type, Is.EqualTo(TokenType.CONTENT));
            Assert.That(results[5].Value, Is.EqualTo("end"));
        }
        #endregion
    }
}
