using System.Collections.Generic;

namespace UNM.Parser
{
    /// <summary>
    /// A set of NameFragments all falling under the same listname.
    /// </summary>
    public class Namelist
    {
        /// <summary>
        /// The name of the list.
        /// </summary>
        /// 
        public string Name { get; private set; }

        private LinkedList<NameFragment> _fragments = new LinkedList<NameFragment>();
        
        /// <summary>
        /// Construct an empty Namelist.
        /// </summary>
        /// <param name="name">The name of the list.</param>
        public Namelist(string name)
        {
            this.Name = name;
        }
        
        /// <summary>
        /// Add a new NameFragment to this list.
        /// </summary>
        /// <param name="fragment">The fragment to add.</param>
        public void AddFragment(NameFragment fragment)
        {
            _fragments.AddLast(fragment);
        }
        
        /// <summary>
        /// Return a list of all fragments matching any of the provided contexts.
        /// </summary>
        /// <param name="contexts"></param>
        /// <returns></returns>
        public List<string> FragmentsForContext(IEnumerable<string> contexts)
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

        /// <summary>
        /// Produce a string representation of this Namelist.
        /// </summary>
        /// <returns>A string representation of this Namelist.</returns>
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