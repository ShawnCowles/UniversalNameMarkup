using System;
using System.Collections.Generic;

namespace UNM.Parser
{
	public class NameList
	{
		public string Name { get; private set; }
		private LinkedList<NameFragment> _fragments = new LinkedList<NameFragment>();
		
		public NameList(string name)
		{
			this.Name = name;
		}
		
		public void AddFragment(NameFragment fragment)
		{
			_fragments.AddLast(fragment);
		}
		
		public List<string> FragmentsForContext(List<string> contexts)
		{
            var outlist = new List<string>();

            foreach (var fragment in _fragments)
            {
                if (fragment.MatchesContexts(contexts))
                {
                    outlist.Add(fragment.Fragment);
                }
            }

            return outlist;
		}

        public override string ToString()
        {
            var ret = "";
            foreach(var fragment in _fragments)
            {
                ret += fragment.Fragment + ", ";
            }
            return ret;
        }
	}
}