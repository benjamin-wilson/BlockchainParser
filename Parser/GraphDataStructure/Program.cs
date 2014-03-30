using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDataStructure
{
    class Program
    {
        static void Main(string[] args)
        {
            //Used for testing
            var temp = GraphDataStructure.Graph.populate("1Ka3xYZb6tZQabpkFCnpqods6gC8iNEcZ7");

            temp.removeEndNodes();
            //temp.writeListToFile();
        }
    }
}
