using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;

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
            using(var reader = new CsvReader(new StreamReader(_sourceFile)))
            {
                while (reader.Read())
                {
                    var listName = reader.GetField(0);

                    if (!_namelists.ContainsKey(listName))
                    {
                        _namelists.Add(listName, new Namelist(listName));
                    }

                    var list = _namelists[listName];

                    var fragment = reader.GetField(1);

                    if (fragment.Length > 1)
                    {
                        var contexts = new List<string>();
                        for (int i = 2; i < reader.FieldHeaders.Length; i++)
                        {
                            if (reader.GetField(i).Length > 0)
                            {
                                contexts.Add(reader.GetField(i));
                            }
                        }

                        list.AddFragment(new NameFragment(fragment, contexts));
                    }
                }
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
