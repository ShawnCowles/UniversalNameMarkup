using System;
using System.Collections.Generic;
using System.IO;

namespace UnmParser
{
    public class FileNamelistSource : INamelistSource
    {
        private string _sourceFile;

        public FileNamelistSource(string sourceFile)
        {
            _sourceFile = sourceFile;
        }

        public UnmData LoadData()
        {
			var reader = new StreamReader(_sourceFile);
			
			var data = new UnmData();

            reader.ReadLine(); // Ignore first line of headers

			var line = reader.ReadLine();
			
			while(line != null)
			{
		
                var tokens = line.Split(new char[] {','});
				
				var listName = tokens[0];
				
				var list = data.GetList(listName);
				
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
				
				line = reader.ReadLine();
			}
            
			return data;
        }
    }
}
