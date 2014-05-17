using System;
using System.Collections.Generic;
using System.Linq;

namespace UNM.Parser
{
    /// <summary>
    /// The NameParser, processes patterns to produce names.
    /// </summary>
	public class NameParser
	{
		private Random _random;
		private string _tag;
        private IEnumerable<string> _context;
		private LinkedList<UnmState> _stateStack = new LinkedList<UnmState>();
        private CapitalizationScheme _capScheme;
        private INamelistSource _namelistSource;

        private bool _initialized;

        /// <summary>
        /// Construct a new NameParser.
        /// </summary>
        /// <param name="namelistSource">The source for namelists to use.</param>
        /// <param name="seed">The random seed to use for NameFragment selection.</param>
		public NameParser (INamelistSource namelistSource, int seed)
		{
            _namelistSource = namelistSource;

			_random = new Random(seed);

            _initialized = false;
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
            }
        }
        		
        /// <summary>
        /// Process a pattern.
        /// </summary>
        /// <param name="pattern">The pattern to process.</param>
        /// <param name="capScheme">The CapitalizationScheme to use.</param>
        /// <throws>A PattternParseException if there is any error processing the pattern.</throws>
        /// <returns>The result of processing the pattern.</returns>
		public string Process(string pattern, CapitalizationScheme capScheme)
		{
			return Process (pattern, new List<string>(), capScheme);
		}

        /// <summary>
        /// Process a pattern.
        /// </summary>
        /// <param name="pattern">The pattern to process.</param>
        /// <param name="context">The contexts to use to filter NameFragments.</param>
        /// <param name="capScheme">The CaptializationScheme to use.</param>
        /// <throws>A PattternParseException if there is any error processing the pattern.</throws>
        /// <returns>The result of processing the pattern.</returns>
        public string Process(string pattern, IEnumerable<string> context, CapitalizationScheme capScheme)
		{
            return Process(pattern, context, new Dictionary<string, string>(), capScheme);
		}

        /// <summary>
        /// Process a pattern.
        /// </summary>
        /// <param name="pattern">The pattern to process.</param>
        /// <param name="context">The contexts to use to filter NameFragments.</param>
        /// <param name="variables">The variable values to use for external variables.</param>
        /// <param name="capScheme">The capitalization scheme to use.</param>
        /// <throws>A PattternParseException if there is any error processing the pattern.</throws>
        /// <returns>The result of processing the pattern.</returns>
        public string Process(string pattern, IEnumerable<string> context, Dictionary<string, string> variables, CapitalizationScheme capScheme)
        {
            return Process(pattern, context, variables, new List<string>(), capScheme);
        }

        /// <summary>
        /// Process a pattern.
        /// </summary>
        /// <param name="pattern">The pattern to process.</param>
        /// <param name="context">The contexts to use to filter NameFragments.</param>
        /// <param name="variables">The variable values to use for external variables.</param>
        /// <param name="uniqueCheck">The list of names to check against for uniqueness.</param>
        /// <param name="capScheme">The capitalization scheme to use.</param>
        /// <throws>A PattternParseException if there is any error processing the pattern.</throws>
        /// <returns>The result of processing the pattern.</returns>
        public string Process(string pattern, IEnumerable<string> context, Dictionary<string, string> variables, List<string> uniqueCheck, CapitalizationScheme capScheme)
        {
            if (!_initialized)
            {
                throw new PatternParseException("NameParser is uninitialized. Call Initialize before Process");
            }

            for (int i = 0; i < 1000; i++)
            {
                var result = DoProcess(pattern, context, variables, capScheme);

                if (!uniqueCheck.Contains(result))
                {
                    return result;
                }
            }

            throw new PatternParseException("Unable to generate unique result!");
        }

        private string DoProcess(string pattern, IEnumerable<string> context, Dictionary<string, string> variables, CapitalizationScheme capScheme)
        {
            _capScheme = capScheme;
            _context = context;
            _tag = "";
            var outString = "";
            _stateStack.AddFirst(UnmState.NO_STATE);
            for(int i = 0; i < pattern.Length; i++)
            {
                switch(_stateStack.First.Value)
                {
                    case UnmState.NO_STATE: outString += HandleNoState(pattern[i]); break;
                    case UnmState.IN_TAG: outString += HandleInTag(pattern[i]); break;
                    case UnmState.BRANCH_TAG: outString += HandleBranchTag(pattern[i]); break;
                    case UnmState.CONTEXT_BRANCH_TAG: outString += HandleContextBranchTag(pattern[i]); break;
                    case UnmState.TAKE_BRANCH: outString += HandleTakeBranch(pattern[i]); break;
                    case UnmState.IGNORE_BRANCH: outString += HandleIgnoreBranch(pattern[i]); break;
                    case UnmState.NESTED_IGNORE: outString += HandleNestedIgnore(pattern[i]); break;
                    case UnmState.VARIABLE_TAG: outString += HandleVariableTag(pattern[i], variables); break;
                    case UnmState.VARIABLE_BRANCH_TAG: outString += HandleVariableBranchTag(pattern[i], variables); break;
                    default: throw new Exception("Unknown state: " + _stateStack.First.Value);
                }
            }

            switch(_capScheme)
            {
                case CapitalizationScheme.BY_FRAGMENT: break;
                case CapitalizationScheme.BY_WORDS: outString = CaptializeByWords(outString); break;
                case CapitalizationScheme.FIRST_LETTER: outString = CaptializeByFirstLetter(outString); break;
                case CapitalizationScheme.NONE: break;
                case CapitalizationScheme.BY_SENTENCE: outString = CapitalizeBySentence(outString); break;
            }

            return outString;
        }

        private string CaptializeByFirstLetter(string text)
        {
            var result = text[0].ToString().ToUpper();

            for(int i = 1; i < text.Length; i++)
            {
                result += text[i];
            }

            return result;
        }

        private string CapitalizeBySentence(string text)
        {
            var result = "";

            var punctuation = new List<char> { ',', '.', '!', '?' };

            var letters = new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p',
                'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

            var shouldCap = true;

            for(int i = 0; i < text.Length; i++)
            {
                var c = text[i];

                if(String.IsNullOrEmpty(c.ToString()))
                {
                    result += c;
                }
                else if(punctuation.Contains(c))
                {
                    result += c;
                    shouldCap = true;
                }
                else
                {
                    if(letters.Contains(c) && shouldCap)
                    {
                        result += c.ToString().ToUpper();
                        shouldCap = false;
                    }
                    else
                    {
                        result += c;
                    }
                }
            }

            return result;
        }

        private string CaptializeByWords(string text)
        {
            var result = "";

            result += text[0].ToString().ToUpper();

            for(int i = 1; i < text.Length; i++)
            {
                if(String.IsNullOrEmpty(text[i - 1].ToString()) || text[i - 1] == ' ')
                {
                    result += text[i].ToString().ToUpper();
                }
                else
                {
                    result += text[i];
                }
            }

            return result;
        }
		
		private string HandleNoState(char c)
		{
			switch(c)
			{
				case '<': _stateStack.AddFirst(UnmState.IN_TAG); _tag = ""; break;
				case '>': goto case '|';
				case '{': goto case '|';
				case '}': goto case '|';
				case '|': throw new PatternParseException("Unexpected character in pattern: " + c);
				case '@': goto default;
				case '$': goto default;
                case '#': goto default;
				default: return c + "";
			}
			return "";
		}
		
		private string HandleInTag(char c)
		{
			switch(c)
			{
				case '<': goto case '|';
				case '>': _stateStack.RemoveFirst(); return ProcessTag();
				case '{': goto case '|';
				case '}': goto case '|';
				case '|': throw new PatternParseException("Unexpected character in pattern: " + c);
                case '@': _stateStack.RemoveFirst(); _stateStack.AddFirst(UnmState.BRANCH_TAG); break;
                case '$': _stateStack.RemoveFirst(); _stateStack.AddFirst(UnmState.CONTEXT_BRANCH_TAG); break;
                case '#': _stateStack.RemoveFirst(); _stateStack.AddFirst(UnmState.VARIABLE_TAG); break;
				default: _tag += c; break;
			}
			return "";
		}
		
		private string HandleBranchTag(char c)
		{
			switch(c)
			{
				case '<': goto case '|';
				case '>': goto case '|';
				case '{': _stateStack.RemoveFirst(); ProcessBranchTag(); break;
				case '}': goto case '|';
				case '|': throw new PatternParseException("Unexpected character in pattern: " + c);
				case '@': goto case '|';
				case '$': goto case '|';
                case '#': if(_tag.Length == 0)
                    {
                        _stateStack.RemoveFirst();
                        _stateStack.AddFirst(UnmState.VARIABLE_BRANCH_TAG)
                        ; break;
                    }
                    else
                    {
                        goto case '|';
                    }
				default: _tag += c; break;
			}
			return "";
		}
		
		private string HandleContextBranchTag(char c)
		{
			switch(c)
			{
				case '<': goto case '|';
				case '>': goto case '|';
				case '{': _stateStack.RemoveFirst(); ProcessContextBranchTag(); break;
				case '}': goto case '|';
				case '|': throw new PatternParseException("Unexpected character in pattern: " + c);
				case '@': goto case '|';
				case '$': goto case '|';
                case '#': goto case '|';
				default: _tag += c; break;
			}
			return "";
		}
		
		private string HandleTakeBranch(char c)
		{
			switch(c)
			{
				case '<': _stateStack.AddFirst(UnmState.IN_TAG); _tag = ""; break;
				case '>': goto case '{';
				case '{': throw new PatternParseException("Unexpected character in pattern: " + c);
				case '}': _stateStack.RemoveFirst(); break;
				case '|': _stateStack.RemoveFirst(); _stateStack.AddFirst(UnmState.IGNORE_BRANCH); break;
				case '@': goto default;
				case '$': goto default;
                case '#': goto default;
				default: return c + "";
			}
			return "";
		}
		
		private string HandleIgnoreBranch(char c)
		{
			switch(c)
			{
				case '<': goto default;
				case '>': goto default;
				case '{': _stateStack.AddFirst(UnmState.NESTED_IGNORE); break;
				case '}': _stateStack.RemoveFirst(); break;
				case '|': _stateStack.RemoveFirst(); _stateStack.AddFirst(UnmState.TAKE_BRANCH); break;
				case '@': goto default;
				case '$': goto default;
                case '#': goto default;
				default: break;
			}
			return "";
		}
		
		private string HandleNestedIgnore(char c)
		{
			switch(c)
			{
				case '<': goto default;
				case '>': goto default;
				case '{': _stateStack.AddFirst(UnmState.NESTED_IGNORE); break;
				case '}': _stateStack.RemoveFirst(); break;
				case '|': goto default;
				case '@': goto default;
				case '$': goto default;
                case '#': goto default;
				default: break;
			}
			return "";
		}

        private string HandleVariableTag(char c, Dictionary<string, string> variables)
        {
            switch(c)
            {
                case '<': goto case '|';
                case '>': _stateStack.RemoveFirst(); return ProcessVariableTag(variables);
                case '{': goto case '|';
                case '}': goto case '|';
                case '|': throw new PatternParseException("Unexpected character in pattern: " + c);
                case '@': goto case '|';
                case '$': goto case '|';
                case '#': goto case '|';
                default: _tag += c; break;
            }
            return "";
        }

        private string HandleVariableBranchTag(char c, Dictionary<string, string> variables)
        {
            switch(c)
            {
                case '<': goto case '|';
                case '>': goto case '|';
                case '{': _stateStack.RemoveFirst(); ProcessVariableBranchTag(variables); break;
                case '}': goto case '|';
                case '|': throw new PatternParseException("Unexpected character in pattern: " + c);
                case '@': goto case '|';
                case '$': goto case '|';
                case '#': goto case '|';
                default: _tag += c; break;
            }
            return "";
        }

		private string ProcessTag()
		{
			var list = _namelistSource.GetNamelist(_tag);
			
			var fragments = list.FragmentsForContext(_context);
			
			if(fragments.Count == 0)
			{
                var cstr = "";

                foreach(var c in _context)
                {
                    cstr += c + ", ";
                }

				throw new PatternParseException("No fragments found in list: " + _tag
					+ " for context: "  + cstr);
			}
			
			var fragment = fragments[_random.Next(fragments.Count)];

            if(_capScheme == CapitalizationScheme.BY_FRAGMENT)
            {
                var outFragment = "";

                for(int i = 0; i < fragment.Length; i++)
                {
                    if(i == 0)
                    {
                        outFragment += fragment[i].ToString().ToUpper();
                    }
                    else
                    {
                        outFragment += fragment[i];
                    }
                }

                fragment = outFragment;
            }

            return fragment;
		}

        private void ProcessBranchTag()
		{
			int chance = Int32.Parse(_tag);
			
			if(chance >= _random.Next(100))
			{
				_stateStack.AddFirst(UnmState.TAKE_BRANCH);
			}
			else
			{
				_stateStack.AddFirst(UnmState.IGNORE_BRANCH);
			}
		}
		
		private void ProcessContextBranchTag()
		{
			if(_context.Contains(_tag))
			{
				_stateStack.AddFirst(UnmState.TAKE_BRANCH);
			}
			else
			{
				_stateStack.AddFirst(UnmState.IGNORE_BRANCH);
			}
		}

        private string ProcessVariableTag(Dictionary<string, string> variables)
        {
            if(variables.ContainsKey(_tag))
            {
                return variables[_tag];
            }
            else
            {
                throw new PatternParseException("No value specified for variable: " + _tag);
            }
        }

        private void ProcessVariableBranchTag(Dictionary<string, string> variables)
        {
            if(variables.ContainsKey(_tag))
            {
                _stateStack.AddFirst(UnmState.TAKE_BRANCH);
            }
            else
            {
                _stateStack.AddFirst(UnmState.IGNORE_BRANCH);
            }
        }
		
		private enum UnmState
		{
			NO_STATE, IN_TAG, BRANCH_TAG, CONTEXT_BRANCH_TAG, TAKE_BRANCH, IGNORE_BRANCH, NESTED_IGNORE, VARIABLE_TAG, VARIABLE_BRANCH_TAG
		}
	}
}