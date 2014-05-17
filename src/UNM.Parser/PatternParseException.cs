using System;

namespace UNM.Parser
{
	public class PatternParseException : Exception
	{
		public PatternParseException (string message)
			:base(message)
		{
		}
	}
}

