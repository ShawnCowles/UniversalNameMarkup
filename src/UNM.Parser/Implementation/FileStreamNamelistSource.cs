using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using UNM.Parser.Data;
using UNM.Parser.Interfaces;

namespace UNM.Parser.Implementation
{
    /// <summary>
    /// An INamelistSource that loads Namelists from a .csv file via a stream.
    /// </summary>
    public class FileStreamNamelistSource : INamelistSource
    {
        private Dictionary<string, Namelist> _namelists;

        private Stream _sourceStream;

        private IContextExpressionParser _expressionParser;

        /// <summary>
        /// Construct a new FileStreamNamelistSource
        /// </summary>
        /// <param name="sourceStream">The stream to load Namelists from. Should provide a .csv format.</param>
        /// <param name="contextParser">The <see cref="IContextExpressionParser"/> to use to parse context expressions.</param>
        public FileStreamNamelistSource(Stream sourceStream, IContextExpressionParser contextParser)
        {
            _sourceStream = sourceStream;
            _namelists = new Dictionary<string, Namelist>();
            _expressionParser = contextParser;
        }

        /// <summary>
        /// Initialize the FileStreamNamelistSource. Reads and parses the .csv stream.
        /// </summary>
        public void Initialize()
        {
            _expressionParser.Initialize();
            
            using(var reader = new CsvReader(new StreamReader(_sourceStream)))
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
                        var contextExpression = _expressionParser.ParseExpression(reader.GetField(2));
                        
                        list.AddFragment(new NameFragment(fragment, contextExpression));
                    }
                }
            }
        }

        /// <summary>
        /// Retrieve a Namelist from this FileStreamNamelistSource
        /// </summary>
        /// <param name="name">The name of the list to retrieve.</param>
        /// <returns>The Namelist matching <paramref name="name"/></returns>
        /// <throws>ArgumentException if the FileStreamNamelistSource does not contain a list matching <paramref name="name"/></throws>
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
