using System;
using System.Collections.Generic;
using System.IO;

namespace UNM.Parser
{
    /// <summary>
    /// An INamelistSource that loads Namelists from a .csv file.
    /// </summary>
    public class FileNamelistSource : INamelistSource
    {
        private Dictionary<string, Namelist> _namelists;

        private string _sourceFile;

        /// <summary>
        /// Construct a new FileNamelistSource
        /// </summary>
        /// <param name="sourceFile">The path to load Namelists from.</param>
        public FileNamelistSource(string sourceFile)
        {
            _sourceFile = sourceFile;
            _namelists = new Dictionary<string, Namelist>();
        }

        /// <summary>
        /// Initialize the FileNamelistSource. Reads and parses the .csv file.
        /// </summary>
        public void Initialize()
        {
			var reader = new StreamReader(_sourceFile);

            reader.ReadLine(); // Ignore first line of headers

			var line = reader.ReadLine();
			
			while(line != null)
			{
		
                var tokens = line.Split(new char[] {','});
				
				var listName = tokens[0];

                var list = new Namelist(listName);
				
				var fragment = tokens[1];

                if (fragment.Length > 1)
                {
                    var contexts = new List<string>();
                    for (int i = 2; i < tokens.Length; i++)
                    {
                        if (tokens[i].Length > 0)
                        {
                            contexts.Add(tokens[i]);
                        }
                    }

                    list.AddFragment(new NameFragment(fragment, contexts));
                }

                _namelists.Add(listName, list);
				
				line = reader.ReadLine();
			}
        }

        /// <summary>
        /// Retrieve a Namelist from this FileNamelistSource
        /// </summary>
        /// <param name="name">The name of the list to retrieve.</param>
        /// <returns>The Namelist matching <paramref name="name"/></returns>
        /// <throws>ArgumentException if the FileNamelistSource does not contain a list matching <paramref name="name"/></throws>
        public Namelist GetNamelist(string name)
        {
            if (_namelists.ContainsKey(name))
            {
                return _namelists[name];
            }

            throw new ArgumentException("No list matching name: " + name);
        }
    }
}
