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
            Console.WriteLine("DONE");
            Console.WriteLine(temp.NodeSet.Count.ToString());
            Console.ReadKey();
            //temp.writeListToFile(@"C:\Windows v2\BeforePruning.txt");

            //temp.pathTrim(degrees+1);
            //temp.quickTrim();
            //temp.writeListToFile(@"C:\Users\wilso_000\Desktop\AfterPruning.txt");
        }
    }
}
