using System;
using GraphDataStructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class GraphNodeUnitTest
    {
        [TestMethod]
        public void Constructor()
        {
            GraphDataStructure.GraphNode test = new GraphDataStructure.GraphNode();

            Assert.IsNotNull(test);
        }
    }
}
