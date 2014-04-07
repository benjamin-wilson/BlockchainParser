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
            //Used for testing
            var temp = GraphDataStructure.Graph.populate("16MEiyzg9qaB1RWBhmcYd8bicVcEiTQJrE", 1);

            temp.writeListToFile(@"C:\Windows v2\BeforePruning.txt");

            temp.removeEndNodes();
            temp.writeListToFile(@"C:\Windows v2\AfterPruning.txt");
        }
    }
}
