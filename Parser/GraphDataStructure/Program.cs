using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;

namespace GraphDataStructure
{
    class Program
    {
        static void Main(string[] args)
        {
            int degrees = 3;
            //Used for testing
            Graph temp = Graph.populate("16MEiyzg9qaB1RWBhmcYd8bicVcEiTQJrE", degrees);
            //temp.writeListToFile(@"C:\Windows v2\BeforePruning.txt");

            //temp.pathTrim(degrees+1);
            temp.quickTrim();
            temp.writeListToFile(@"C:\Users\wilso_000\Desktop\AfterPruning.txt");
            temp.writeJSONToFile(@"C:\Users\wilso_000\Desktop\miserables.html");
        }
    }
}
