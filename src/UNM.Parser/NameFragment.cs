using System;
using System.Collections.Generic;

namespace UNM.Parser
{
	public class NameFragment
	{
		public string Fragment { get; private set; }
		private List<string> _contexts;
		
		public NameFragment (string fragment, List<string> contexts)
		{
			Fragment = fragment;
			_contexts = contexts;
		}
		
		public bool MatchesContexts(List<string> toMatch)
		{
			if((toMatch.Count == 0 && _contexts.Count == 0) || _contexts.Count == 0)
				return true;
			
			foreach(var match in toMatch)
			{
				foreach(var context in _contexts)
				{
					if(match == context)
						return true;
				}
			}
			
			return false;
		}
	}
}