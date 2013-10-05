using System;
using System.Collections.Generic;
using UnmParser;

namespace UnmPaser_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var source = new FileNamelistSource("namelists.csv");
            var parser = new NameParser(source, DateTime.Now.Millisecond);

            string pattern = @"<god_adjective> <god_noun> <#test_variable> <@#test_branch{taken|ignored}";

            var contexts = new List<string>() { "morality_evil", "structure_order", "domain_magic", "domain_war", "gender_female" };

            var variables = new Dictionary<string, string>();
            variables.Add("test_variable", "good");
            //variables.Add("test_branch", "bad");

            var previous = new List<string>();

            for (int i = 0; i < 30; i++)
            {
                var name = parser.Process(pattern, contexts, variables, previous, NameParser.CapitalizationScheme.BY_FRAGMENT);
                
                Console.Write(name);

                if (previous.Contains(name))
                {
                    Console.WriteLine(" DUPE!");
                }
                else
                {
                    Console.WriteLine();
                    previous.Add(name);
                }
            }

            Console.ReadKey();
        }
    }
}
