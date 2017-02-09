using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UNM.Parser.Data;
using UNM.Parser.Interfaces;

namespace UNM.Parser.Implementation
{
    /// <summary>
    /// The NameParser, processes patterns to produce names.
    /// </summary>
    public class NameParser : INameParser
    {
        private Random _random;
        private bool _initialized;
        private INamelistSource _namelistSource;
        private IPatternLexer _lexer;

        /// <summary>
        /// Construct a new NameParser.
        /// </summary>
        /// <param name="namelistSource">The source for namelists to use.</param>
        /// <param name="seed">The random seed to use for NameFragment selection.</param>
        /// <param name="lexer">The <see cref="IPatternLexer"/> to use to process patterns.</param>
        public NameParser (INamelistSource namelistSource, IPatternLexer lexer, int seed)
        {
            _namelistSource = namelistSource;

            _lexer = lexer;

            _random = new Random(seed);

            _initialized = false;
        }

        /// <summary>
        /// Construct a new NameParser using the default <see cref="IPatternLexer"/>.
        /// </summary>
        /// <param name="namelistSource">The source for namelists to use.</param>
        /// <param name="seed">The random seed to use for NameFragment selection.</param>
        public NameParser(INamelistSource namelistSource, int seed)
            : this(namelistSource, new PatternLexer(new SimpleLexer.Lexer()), seed)
        {
        }

        /// <summary>
        /// Construct a new NameParser using the default <see cref="IPatternLexer"/> and the 
        /// current milliseconds for the random seed.
        /// </summary>
        /// <param name="namelistSource">The source for namelists to use.</param>
        public NameParser(INamelistSource namelistSource)
            : this(namelistSource, new PatternLexer(new SimpleLexer.Lexer()), DateTime.Now.Millisecond)
        {
        }

        /// <summary>
        /// Initialize the NameParser.
        /// </summary>
        public void Initialize()
        {
            if (!_initialized)
            {
                _initialized = true;
                _namelistSource.Initialize();
                _lexer.Initialize();
            }
        }
        
        /// <summary>
        /// Process a pattern.
        /// </summary>
        /// <param name="parameters">The pattern processing parameters to use.</param>
        /// <throws>A PattternParseException if there is any error processing the pattern.</throws>
        /// <returns>The result of processing the pattern.</returns>
        public string Process(PatternProcessingParameters parameters)
        {
            if (!_initialized)
            {
                throw new PatternParseException("NameParser is uninitialized. Call Initialize before Process");
            }

            try
            {

                for (int i = 0; i < 1000; i++)
                {
                    var result = DoProcess(parameters);

                    if (!parameters.UniqueCheck.Contains(result))
                    {
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new PatternParseException("Error processing pattern", ex);
            }

            throw new PatternParseException("Unable to generate unique result!");
        }

        private string DoProcess(PatternProcessingParameters parameters)
        {
            var tokens = new LinkedList<PatternToken>(_lexer.Process(parameters.Pattern));

            var resultBuilder = new StringBuilder();

            var stateStack = new Stack<UnmState>();
            stateStack.Push(UnmState.READ);

            var location = tokens.First;

            while (location != null)
            {
                var current = location.Value;

                if (stateStack.Peek() == UnmState.READ)
                {
                    switch (current.Type)
                    {
                        case TokenType.BRANCH_END:
                            stateStack.Pop();
                            break;

                        case TokenType.BRANCH_START:
                            stateStack.Push(UnmState.NESTED_IGNORE);
                            break;

                        case TokenType.CONTENT:
                            resultBuilder.Append(current.Value);
                            break;

                        case TokenType.TAG_BRANCH_CHANCE:
                            if (location.Next == null || location.Next.Value.Type != TokenType.BRANCH_START)
                            {
                                throw new PatternParseException(string.Format(
                                    "Syntax error at position: {0}, { expected after conditional tag",
                                    current.SourceIndex));
                            }
                            if (EvalulateCondition(parameters, current))
                            {
                                stateStack.Push(UnmState.READ);
                            }
                            else
                            {
                                stateStack.Push(UnmState.IGNORE);
                            }
                            location = location.Next;
                            break;

                        case TokenType.TAG_BRANCH_CONTEXT:
                            goto case TokenType.TAG_BRANCH_CHANCE;

                        case TokenType.TAG_BRANCH_VARIABLE:
                            goto case TokenType.TAG_BRANCH_CHANCE;

                        case TokenType.TAG_ELSE:
                            if (stateStack.Count < 2)
                            {
                                throw new PatternParseException(
                                    "Cannot use an else outside of a branch, location: "
                                    + current.SourceIndex);
                            }
                            stateStack.Pop();
                            stateStack.Push(UnmState.IGNORE);
                            break;

                        case TokenType.TAG_SUB_FRAGMENT:
                            resultBuilder = PerformSubstitution(resultBuilder, parameters, current);
                            break;

                        case TokenType.TAG_SUB_VARIABLE:
                            goto case TokenType.TAG_SUB_FRAGMENT;

                        case TokenType.TAG_SUB_PATTERN:
                            resultBuilder = ProcessSubPattern(resultBuilder, parameters, current);
                            break;
                    }
                }
                else if (stateStack.Peek() == UnmState.IGNORE)
                {
                    switch (current.Type)
                    {
                        case TokenType.BRANCH_END:
                            stateStack.Pop();
                            break;
                        case TokenType.BRANCH_START:
                            stateStack.Push(UnmState.NESTED_IGNORE);
                            break;
                        case TokenType.TAG_ELSE:
                            stateStack.Pop();
                            stateStack.Push(UnmState.READ);
                            break;
                    }
                }
                else if (stateStack.Peek() == UnmState.NESTED_IGNORE)
                {
                    switch(current.Type)
                    {
                        case TokenType.BRANCH_END:
                            stateStack.Pop();
                            break;
                        case TokenType.BRANCH_START:
                            stateStack.Push(UnmState.NESTED_IGNORE);
                            break;
                    }
                }

                if (stateStack.Count == 0)
                {
                    throw new PatternParseException("Extra closing bracket at position: " + current.SourceIndex);
                }

                location = location.Next;
            }

            if (stateStack.Count > 1)
            {
                throw new PatternParseException("Unclosed branch by end of pattern");
            }

            switch(parameters.CapitalizationScheme)
            {
                case CapitalizationScheme.BY_FRAGMENT: break; //Handled earlier
                case CapitalizationScheme.BY_WORDS: resultBuilder = CaptializeByWords(resultBuilder); break;
                case CapitalizationScheme.FIRST_LETTER: resultBuilder = CaptializeByFirstLetter(resultBuilder); break;
                case CapitalizationScheme.NONE: break;
                case CapitalizationScheme.BY_SENTENCE: resultBuilder = CapitalizeBySentence(resultBuilder); break;
            }

            return resultBuilder.ToString();
        }

        private bool EvalulateCondition(PatternProcessingParameters parameters, PatternToken token)
        {
            if (token.Type == TokenType.TAG_BRANCH_CHANCE)
            {
                var chance = int.Parse(TrimTag(1, token.Value));

                return _random.Next(100) < chance;
            }
            else if (token.Type == TokenType.TAG_BRANCH_CONTEXT)
            {
                return parameters.Context.Contains(TrimTag(1, token.Value));
            }
            else if (token.Type == TokenType.TAG_BRANCH_VARIABLE)
            {
                return parameters.Variables.ContainsKey(TrimTag(1, token.Value));
            }

            throw new PatternParseException("Unhandled condition type: " + token.Type);
        }

        private StringBuilder PerformSubstitution(StringBuilder resultBuilder,
            PatternProcessingParameters parameters, PatternToken token)
        {
            if (token.Type == TokenType.TAG_SUB_FRAGMENT)
            {
                var namelistName = TrimTag(0, token.Value);
                var namelist = _namelistSource.GetNamelist(namelistName);

                if (namelist == null)
                {
                    throw new PatternParseException(string.Format("No namelist matching {0} found.",
                        namelistName));
                }

                var fragments = namelist.FragmentsForContext(parameters.Context);              

                if (fragments.Count < 1)
                {
                    throw new PatternParseException(string.Format(
                        "No fragments found for namelist: {0} and contexts: {1}",
                        namelistName,
                        string.Join(", ", parameters.Context.ToArray())));
                }

                var r = _random.Next(fragments.Count);

                var fragment = fragments[r];

                if (parameters.CapitalizationScheme == CapitalizationScheme.BY_FRAGMENT)
                {
                    fragment = char.ToUpper(fragment[0]) + fragment.Substring(1);
                }

                return resultBuilder.Append(fragment);
            }
            else if (token.Type == TokenType.TAG_SUB_VARIABLE)
            {
                var variableName = TrimTag(1, token.Value);

                if (!parameters.Variables.ContainsKey(variableName))
                {
                    throw new PatternParseException("Variable expected but not provided: " + variableName);
                }

                return resultBuilder.Append(parameters.Variables[variableName]);
            }

            throw new PatternParseException("Unhandled substitution type: " + token.Type);
        }

        private StringBuilder ProcessSubPattern(StringBuilder resultBuilder,
            PatternProcessingParameters parameters, PatternToken token)
        {
            var namelistName = TrimTag(1, token.Value);
            var namelist = _namelistSource.GetNamelist(namelistName);

            if(namelist == null)
            {
                throw new PatternParseException(string.Format("No namelist matching {0} found.",
                    namelistName));
            }

            var fragments = namelist.FragmentsForContext(parameters.Context);

            if(fragments.Count < 1)
            {
                throw new PatternParseException(string.Format(
                    "No fragments found for namelist: {0} and contexts: {1}",
                    namelistName,
                    string.Join(", ", parameters.Context.ToArray())));
            }

            var r = _random.Next(fragments.Count);

            var fragment = fragments[r];

            var subParams = new PatternProcessingParameters(fragment)
            {
                CapitalizationScheme = parameters.CapitalizationScheme,
                Context = parameters.Context,
                UniqueCheck = parameters.UniqueCheck,
                Variables = parameters.Variables
            };

            var result = Process(subParams);

            return resultBuilder.Append(result);
        }

        private string TrimTag(int extraLeadingCharacters, string rawValue)
        {
            return rawValue.Substring(
                1 + extraLeadingCharacters,
                rawValue.Length - 2 - extraLeadingCharacters);
        }

        private StringBuilder CaptializeByWords(StringBuilder resultBuilder)
        {
            var c = resultBuilder[0];

            resultBuilder.Remove(0, 1);

            resultBuilder.Insert(0, char.ToUpper(c));


            for (int i = 1; i < resultBuilder.Length; i++)
            {
                if (char.IsWhiteSpace(resultBuilder[i - 1]) || resultBuilder[i - 1] == '-')
                {
                    c = resultBuilder[i];

                    resultBuilder.Remove(i, 1);

                    resultBuilder.Insert(i, char.ToUpper(c));
                }
            }

            return resultBuilder;
        }

        private StringBuilder CaptializeByFirstLetter(StringBuilder resultBuilder)
        {
            var c = resultBuilder[0];

            resultBuilder.Remove(0, 1);

            resultBuilder.Insert(0, char.ToUpper(c));

            return resultBuilder;
        }

        private StringBuilder CapitalizeBySentence(StringBuilder resultBuilder)
        {
            var passedPunctuation = true;
            var passedWhitespace = true;

            var shouldCap = true;

            for (int i = 0; i < resultBuilder.Length; i++)
            {
                var c = resultBuilder[i];

                if (c == '.'  || char.IsWhiteSpace(c))
                {
                    if (c == '.')
                    {
                        passedPunctuation = true;
                    }
                    if(char.IsWhiteSpace(c))
                    {
                        passedWhitespace = true;
                    }
                }
                else
                {
                    passedWhitespace = false;
                    passedPunctuation = false;
                }

                if (passedPunctuation && passedWhitespace)
                {
                    shouldCap = true;
                }

                if (char.IsLetter(c))
                {
                    if (shouldCap)
                    {
                        resultBuilder.Remove(i, 1);

                        resultBuilder.Insert(i, char.ToUpper(c));

                        shouldCap = false;
                    }
                }
            }

            return resultBuilder;
        }

        private enum UnmState
        {
            READ,
            IGNORE,
            NESTED_IGNORE
        }
    }
}