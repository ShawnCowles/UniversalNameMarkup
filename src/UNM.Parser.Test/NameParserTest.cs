using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace UNM.Parser
{
    [TestFixture]
    public class NameParserTest
    {
        private Fixture _fixture = new Fixture();

        [Test]
        public void Initialize_initializes_the_namelist_source_and_pattern_lexer()
        {
            var mockNamelistSource = new Mock<INamelistSource>();

            var mockLexer = new Mock<IPatternLexer>();
            
            var parser = new NameParser(mockNamelistSource.Object, mockLexer.Object, _fixture.Create<int>());

            parser.Initialize();

            mockNamelistSource.Verify(x => x.Initialize());
        }

        [Test]
        public void Initialize_only_initializes_the_namelist_source_and_pattern_lexer_once()
        {
            var mockNamelistSource = new Mock<INamelistSource>();

            var mockLexer = new Mock<IPatternLexer>();

            var parser = new NameParser(mockNamelistSource.Object, mockLexer.Object, _fixture.Create<int>());

            parser.Initialize();

            mockNamelistSource.Verify(x => x.Initialize(), Times.Once);
        }

        [Test]
        public void Process_throws_exception_if_initialize_hasnt_been_called()
        {
            var mockNamelistSource = new Mock<INamelistSource>();

            var mockLexer = new Mock<IPatternLexer>();

            var parser = new NameParser(mockNamelistSource.Object, mockLexer.Object, _fixture.Create<int>());

            Assert.Throws<PatternParseException>(
                () => parser.Process(_fixture.Create<PatternProcessingParameters>()));
        }

        [Test]
        public void Process_rethrows_exceptions_thrown_by_provided_lexer()
        {
            var mockLexer = new Mock<IPatternLexer>(); ;

            var mockNamelistSource = new Mock<INamelistSource>();

            mockLexer
                .Setup(x => x.Process(It.IsAny<string>()))
                .Throws(new PatternParseException(""));
            
            var parser = new NameParser(mockNamelistSource.Object, mockLexer.Object, _fixture.Create<int>());

            parser.Initialize();

            Assert.Throws<PatternParseException>(
                () => parser.Process(_fixture.Create<PatternProcessingParameters>()));
        }

        [Test]
        public void Process_correctly_subsitutes_fragments_in_simple_pattern()
        {
            var testPattern = "<first_part><second_part>";

            var firstNamelist = new Namelist("first_part");
            var firstReplacement = _fixture.Create<string>();
            firstNamelist.AddFragment(new NameFragment(firstReplacement, Enumerable.Empty<string>()));

            var secondNamelist = new Namelist("second_part");
            var secondReplacement = _fixture.Create<string>();
            secondNamelist.AddFragment(new NameFragment(secondReplacement, Enumerable.Empty<string>()));

            var expectedResult = firstReplacement + secondReplacement;

            var lexer = new PatternLexer(new SimpleLexer.Lexer());

            var mockNamelistSource = new Mock<INamelistSource>();
            mockNamelistSource
                .Setup(x => x.GetNamelist("first_part"))
                .Returns(firstNamelist);
            mockNamelistSource
                .Setup(x => x.GetNamelist("second_part"))
                .Returns(secondNamelist);

            var parser = new NameParser(mockNamelistSource.Object, lexer, _fixture.Create<int>());

            parser.Initialize();

            var parameters = new PatternProcessingParameters(testPattern);

            var result = parser.Process(parameters);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Process_correctly_handles_branching_by_context(bool takeBranch)
        {
            var testPattern = "front<@branch>{middle}end";
            var expectedTakeBranch = "frontmiddleend";
            var expectedIgnoreBranch = "frontend";

            var mockNamelistSource = new Mock<INamelistSource>();

            var context = new List<string>();
            
            if (takeBranch)
            {
                context.Add("branch");
            }

            var lexer = new PatternLexer(new SimpleLexer.Lexer());

            var parser = new NameParser(mockNamelistSource.Object, lexer, _fixture.Create<int>());

            parser.Initialize();

            var parameters = new PatternProcessingParameters(testPattern)
            {
                Context = context
            };

            var result = parser.Process(parameters);

            if (takeBranch)
            {
                Assert.That(result, Is.EqualTo(expectedTakeBranch));
            }
            else
            {
                Assert.That(result, Is.EqualTo(expectedIgnoreBranch));
            }
        }

        [Test]
        public void Process_correctly_handles_branching_by_chance()
        {
            var testPattern = "front<%50>{middle}end";
            var expectedTakeBranch = "frontmiddleend";
            var expectedIgnoreBranch = "frontend";

            var mockNamelistSource = new Mock<INamelistSource>();

            var lexer = new PatternLexer(new SimpleLexer.Lexer());

            var parser = new NameParser(mockNamelistSource.Object, lexer, _fixture.Create<int>());

            parser.Initialize();

            var parameters = new PatternProcessingParameters(testPattern);

            var result = parser.Process(parameters);

            //TODO try asserting that each is taken roughly half the time out of many tries

            Assert.That(result, Is.EqualTo(expectedTakeBranch).Or.EqualTo(expectedIgnoreBranch));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Process_correctly_handles_branching_by_variable(bool takeBranch)
        {
            var testPattern = "front<$var1>{middle}end";
            var expectedTakeBranch = "frontmiddleend";
            var expectedIgnoreBranch = "frontend";

            var mockNamelistSource = new Mock<INamelistSource>();

            var variables = new Dictionary<string, string>();

            if (takeBranch)
            {
                variables.Add("var1", _fixture.Create<string>());
            }

            var lexer = new PatternLexer(new SimpleLexer.Lexer());

            var parser = new NameParser(mockNamelistSource.Object, lexer, _fixture.Create<int>());

            parser.Initialize();

            var parameters = new PatternProcessingParameters(testPattern)
            {
                Variables = variables
            };

            var result = parser.Process(parameters);

            if (takeBranch)
            {
                Assert.That(result, Is.EqualTo(expectedTakeBranch));
            }
            else
            {
                Assert.That(result, Is.EqualTo(expectedIgnoreBranch));
            }
        }

        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(true, true)]
        public void Process_correctly_handles_nested_branching(bool takeOuterBranch, bool takeInnerBranch)
        {
            var testPattern = "front<@outer>{middle<@inner>{deep}}end";
            var expectedTakeJustOutsideBranch = "frontmiddleend";
            var expectedTakeAllBranch = "frontmiddledeepend";
            var expectedIgnoreBranch = "frontend";

            var mockNamelistSource = new Mock<INamelistSource>();

            var context = new List<string>();

            if (takeOuterBranch)
            {
                context.Add("outer");
            }

            if (takeInnerBranch)
            {
                context.Add("inner");
            }

            var lexer = new PatternLexer(new SimpleLexer.Lexer());

            var parser = new NameParser(mockNamelistSource.Object, lexer, _fixture.Create<int>());

            parser.Initialize();

            var parameters = new PatternProcessingParameters(testPattern)
            {
                Context = context
            };

            var result = parser.Process(parameters);

            if (takeOuterBranch)
            {
                if (takeInnerBranch)
                {
                    Assert.That(result, Is.EqualTo(expectedTakeAllBranch));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(expectedTakeJustOutsideBranch));
                }
            }
            else
            {
                Assert.That(result, Is.EqualTo(expectedIgnoreBranch));
            }
        }

        [Test]
        public void Process_replaces_variable_tags()
        {
            var testPattern = "pre<#var1>mid<#var2>post";

            var variable1Value = _fixture.Create<string>();
            var variable2Value = _fixture.Create<string>();

            var expectedResult = "pre" + variable1Value + "mid" + variable2Value + "post";

            var mockNamelistSource = new Mock<INamelistSource>();

            var lexer = new PatternLexer(new SimpleLexer.Lexer());

            var parser = new NameParser(mockNamelistSource.Object, lexer, _fixture.Create<int>());

            parser.Initialize();

            var variables = new Dictionary<string, string>();
            variables.Add("var1", variable1Value);
            variables.Add("var2", variable2Value);

            var parameters = new PatternProcessingParameters(testPattern)
            {
                Variables = variables
            };

            var result = parser.Process(parameters);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void Process_throws_exception_if_variable_subsitution_is_missing()
        {
            var testPattern = "pre<#var1>post";

            var mockNamelistSource = new Mock<INamelistSource>();

            var lexer = new PatternLexer(new SimpleLexer.Lexer());

            var parser = new NameParser(mockNamelistSource.Object, lexer, _fixture.Create<int>());

            parser.Initialize();

            var parameters = new PatternProcessingParameters(testPattern)
            {
                Variables = new Dictionary<string, string>()
            };

            Assert.Throws<PatternParseException>(() => parser.Process(parameters));
        }


        [TestCase(true)]
        [TestCase(false)]
        public void Process_replaces_by_context_properly(bool useProperContext)
        {
            var testPattern = "pre<replaceme>post";

            var properContext = _fixture.Create<string>();
            var improperContext = _fixture.Create<string>();

            var properReplacement = _fixture.Create<string>();
            var improperReplacement = _fixture.Create<string>();

            var namelist = new Namelist("replaceme");
            namelist.AddFragment(new NameFragment(properReplacement, new string[] { properContext }));
            namelist.AddFragment(new NameFragment(improperReplacement, new string[] { improperContext }));

            var expectedResult = "";

            var context = new List<string>();

            if (useProperContext)
            {
                context.Add(properContext);
                expectedResult = "pre" + properReplacement + "post";
            }
            else
            {
                context.Add(improperContext);
                expectedResult = "pre" + improperReplacement + "post";
            }
            
            var mockNamelistSource = new Mock<INamelistSource>();
            mockNamelistSource
                .Setup(x => x.GetNamelist("replaceme"))
                .Returns(namelist);

            var lexer = new PatternLexer(new SimpleLexer.Lexer());

            var parameters = new PatternProcessingParameters(testPattern)
            {
                Context = context
            };

            var parser = new NameParser(mockNamelistSource.Object, lexer, _fixture.Create<int>());

            parser.Initialize();

            var result = parser.Process(parameters);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Process_correctly_handles_branching_with_or(bool takeBranch)
        {
            var testPattern = "front<@branch>{middle|else}end";
            var expectedTakeBranch = "frontmiddleend";
            var expectedIgnoreBranch = "frontelseend";

            var mockNamelistSource = new Mock<INamelistSource>();

            var context = new List<string>();

            if (takeBranch)
            {
                context.Add("branch");
            }

            var lexer = new PatternLexer(new SimpleLexer.Lexer());

            var parser = new NameParser(mockNamelistSource.Object, lexer, _fixture.Create<int>());

            parser.Initialize();

            var parameters = new PatternProcessingParameters(testPattern)
            {
                Context = context
            };

            var result = parser.Process(parameters);

            if (takeBranch)
            {
                Assert.That(result, Is.EqualTo(expectedTakeBranch));
            }
            else
            {
                Assert.That(result, Is.EqualTo(expectedIgnoreBranch));
            }
        }

        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(true, true)]
        public void Process_correctly_handles_branching_with_nested_or(bool takeInner, bool takeOuter)
        {
            var testPattern = "<@outer>{<@inner>{a|b}|<@inner>{c|d}}";
            var expectedNoNo = "d";
            var expectedNoYes = "c";
            var expectedYesNo = "b";
            var expectedYesYes = "a";

            var mockNamelistSource = new Mock<INamelistSource>();

            var context = new List<string>();

            if (takeInner)
            {
                context.Add("inner");
            }
            if (takeOuter)
            {
                context.Add("outer");
            }

            var lexer = new PatternLexer(new SimpleLexer.Lexer());

            var parser = new NameParser(mockNamelistSource.Object, lexer, _fixture.Create<int>());

            parser.Initialize();

            var parameters = new PatternProcessingParameters(testPattern)
            {
                Context = context
            };

            var result = parser.Process(parameters);

            if (takeOuter)
            {
                if (takeInner)
                {
                    Assert.That(result, Is.EqualTo(expectedYesYes));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(expectedYesNo));
                }
            }
            else
            {
                if (takeInner)
                {
                    Assert.That(result, Is.EqualTo(expectedNoYes));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(expectedNoNo));
                }
            }
        }

        //test handles uniqueness checks properly
        [Test]
        public void Process_handles_uniqueness_check_properly()
        {
            var testPattern = "<replaceme>";

            var replacements = _fixture.CreateMany<string>();

            var namelist = new Namelist("replaceme");

            foreach (var replacement in replacements)
            {
                namelist.AddFragment(new NameFragment(replacement, Enumerable.Empty<string>()));
            }

            var mockNamelistSource = new Mock<INamelistSource>();
            mockNamelistSource
                .Setup(x => x.GetNamelist("replaceme"))
                .Returns(namelist);

            var lexer = new PatternLexer(new SimpleLexer.Lexer());

            var parser = new NameParser(mockNamelistSource.Object, lexer, _fixture.Create<int>());

            parser.Initialize();

            var results = new List<string>();

            PatternProcessingParameters parameters;

            for (int i = 0; i < replacements.Count(); i++)
            {
                parameters = new PatternProcessingParameters(testPattern)
                {
                    UniqueCheck = results
                };

                results.Add(parser.Process(parameters));
            }

            parameters = new PatternProcessingParameters(testPattern)
            {
                UniqueCheck = results
            };

            Assert.Throws<PatternParseException>(
                () => parser.Process(parameters));

            foreach (var replacement in replacements)
            {
                Assert.That(results.Where(x => x == replacement).Count() == 1);
            }
        }

        [Test]
        public void Process_honors_capitalization_by_fragment()
        {
            var firstNamelist = new Namelist("first_part");
            var firstReplacement = "first";
            firstNamelist.AddFragment(new NameFragment(firstReplacement, Enumerable.Empty<string>()));

            var secondNamelist = new Namelist("second_part");
            var secondReplacement = "second";
            secondNamelist.AddFragment(new NameFragment(secondReplacement, Enumerable.Empty<string>()));

            var testPattern = "some begining <first_part> and. then SOME more <second_part> mIxeD CASe";

            var expectedResult = "some begining First and. then SOME more Second mIxeD CASe";


            var mockNamelistSource = new Mock<INamelistSource>();
            mockNamelistSource
                .Setup(x => x.GetNamelist("first_part"))
                .Returns(firstNamelist);
            mockNamelistSource
                .Setup(x => x.GetNamelist("second_part"))
                .Returns(secondNamelist);

            var lexer = new PatternLexer(new SimpleLexer.Lexer());

            var parser = new NameParser(mockNamelistSource.Object, lexer, _fixture.Create<int>());

            parser.Initialize();

            var parameters = new PatternProcessingParameters(testPattern)
            {
                CapitalizationScheme = CapitalizationScheme.BY_FRAGMENT
            };

            var result = parser.Process(parameters);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void Process_honors_capitalization_by_word()
        {
            var firstNamelist = new Namelist("first_part");
            var firstReplacement = "first";
            firstNamelist.AddFragment(new NameFragment(firstReplacement, Enumerable.Empty<string>()));

            var secondNamelist = new Namelist("second_part");
            var secondReplacement = "second";
            secondNamelist.AddFragment(new NameFragment(secondReplacement, Enumerable.Empty<string>()));

            var testPattern = "some begining <first_part> and then. SOME more <second_part> mIxeD CASe";

            var expectedResult = "Some Begining First And Then. SOME More Second MIxeD CASe";


            var mockNamelistSource = new Mock<INamelistSource>();
            mockNamelistSource
                .Setup(x => x.GetNamelist("first_part"))
                .Returns(firstNamelist);
            mockNamelistSource
                .Setup(x => x.GetNamelist("second_part"))
                .Returns(secondNamelist);

            var lexer = new PatternLexer(new SimpleLexer.Lexer());

            var parser = new NameParser(mockNamelistSource.Object, lexer, _fixture.Create<int>());

            parser.Initialize();

            var parameters = new PatternProcessingParameters(testPattern)
            {
                CapitalizationScheme = CapitalizationScheme.BY_WORDS
            };

            var result = parser.Process(parameters);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void Process_honors_capitalization_first_letter()
        {
            var firstNamelist = new Namelist("first_part");
            var firstReplacement = "first";
            firstNamelist.AddFragment(new NameFragment(firstReplacement, Enumerable.Empty<string>()));

            var secondNamelist = new Namelist("second_part");
            var secondReplacement = "second";
            secondNamelist.AddFragment(new NameFragment(secondReplacement, Enumerable.Empty<string>()));

            var testPattern = "some begining <first_part> and then. SOME more <second_part> mIxeD CASe";

            var expectedResult = "Some begining first and then. SOME more second mIxeD CASe";


            var mockNamelistSource = new Mock<INamelistSource>();
            mockNamelistSource
                .Setup(x => x.GetNamelist("first_part"))
                .Returns(firstNamelist);
            mockNamelistSource
                .Setup(x => x.GetNamelist("second_part"))
                .Returns(secondNamelist);

            var lexer = new PatternLexer(new SimpleLexer.Lexer());

            var parser = new NameParser(mockNamelistSource.Object, lexer, _fixture.Create<int>());

            parser.Initialize();

            var parameters = new PatternProcessingParameters(testPattern)
            {
                CapitalizationScheme = CapitalizationScheme.FIRST_LETTER
            };

            var result = parser.Process(parameters);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void Process_honors_capitalization_none()
        {
            var firstNamelist = new Namelist("first_part");
            var firstReplacement = "first";
            firstNamelist.AddFragment(new NameFragment(firstReplacement, Enumerable.Empty<string>()));

            var secondNamelist = new Namelist("second_part");
            var secondReplacement = "second";
            secondNamelist.AddFragment(new NameFragment(secondReplacement, Enumerable.Empty<string>()));

            var testPattern = "some begining <first_part> and then. SOME more <second_part> mIxeD CASe";

            var expectedResult = "some begining first and then. SOME more second mIxeD CASe";


            var mockNamelistSource = new Mock<INamelistSource>();
            mockNamelistSource
                .Setup(x => x.GetNamelist("first_part"))
                .Returns(firstNamelist);
            mockNamelistSource
                .Setup(x => x.GetNamelist("second_part"))
                .Returns(secondNamelist);

            var lexer = new PatternLexer(new SimpleLexer.Lexer());

            var parser = new NameParser(mockNamelistSource.Object, lexer, _fixture.Create<int>());

            parser.Initialize();

            var parameters = new PatternProcessingParameters(testPattern)
            {
                CapitalizationScheme = CapitalizationScheme.NONE
            };

            var result = parser.Process(parameters);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void Process_honors_capitalization_by_sentence()
        {
            var firstNamelist = new Namelist("first_part");
            var firstReplacement = "first";
            firstNamelist.AddFragment(new NameFragment(firstReplacement, Enumerable.Empty<string>()));

            var secondNamelist = new Namelist("second_part");
            var secondReplacement = "second";
            secondNamelist.AddFragment(new NameFragment(secondReplacement, Enumerable.Empty<string>()));

            var testPattern = "some begining <first_part> and then. sOME more <second_part> mIxeD CASe";

            var expectedResult = "Some begining first and then. SOME more second mIxeD CASe";


            var mockNamelistSource = new Mock<INamelistSource>();
            mockNamelistSource
                .Setup(x => x.GetNamelist("first_part"))
                .Returns(firstNamelist);
            mockNamelistSource
                .Setup(x => x.GetNamelist("second_part"))
                .Returns(secondNamelist);

            var lexer = new PatternLexer(new SimpleLexer.Lexer());

            var parser = new NameParser(mockNamelistSource.Object, lexer, _fixture.Create<int>());

            parser.Initialize();

            var parameters = new PatternProcessingParameters(testPattern)
            {
                CapitalizationScheme = CapitalizationScheme.BY_SENTENCE
            };

            var result = parser.Process(parameters);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void Subpatterns_are_processed_before_being_substituted()
        {
            var subPattern = "<first> and then the <second>";

            var pattern = "<^sub_pattern> and then some more <first>";

            var expected = "first_sub and then the second_sub and then some more first_sub";

            var subPatternNamelist = new Namelist("sub_pattern");
            subPatternNamelist.AddFragment(new NameFragment(subPattern, Enumerable.Empty<string>()));

            var firstNamelist = new Namelist("first");
            firstNamelist.AddFragment(new NameFragment("first_sub", Enumerable.Empty<string>()));

            var secondNamelist = new Namelist("second");
            secondNamelist.AddFragment(new NameFragment("second_sub", Enumerable.Empty<string>()));

            var mockNamelistSource = new Mock<INamelistSource>();
            mockNamelistSource
                .Setup(x => x.GetNamelist("sub_pattern"))
                .Returns(subPatternNamelist);

            mockNamelistSource
                .Setup(x => x.GetNamelist("first"))
                .Returns(firstNamelist);

            mockNamelistSource
                .Setup(x => x.GetNamelist("second"))
                .Returns(secondNamelist);

            var parser = new NameParser(mockNamelistSource.Object, 0);

            parser.Initialize();

            var parameters = new PatternProcessingParameters(pattern);

            var result = parser.Process(parameters);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
