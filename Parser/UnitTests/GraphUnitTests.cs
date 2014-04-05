using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class GraphUnitTests
    {
        [TestMethod]
        public void Populate_Graph_Duplicate_Check()
        {
            var temp = GraphDataStructure.Graph.populate("1Ka3xYZb6tZQabpkFCnpqods6gC8iNEcZ7", 1);

            foreach(var item in temp.NodeSet)
            {
                
            }
        }
    }
}
