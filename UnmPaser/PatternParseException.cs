using System;

namespace UnmParser
{
	public class PatternParseException : Exception
	{
		public PatternParseException (string message)
			:base(message)
		{
		}
	}
}

