using System;
using GraphDataStructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class GraphNodeUnitTest
    {
        [TestMethod]
        public void Constructor_1()
        {
            GraphDataStructure.GraphNode test = new GraphDataStructure.GraphNode();

            Assert.IsNotNull(test);
        }
        [TestMethod]
        public void Constructor_2()
        {
            GraphDataStructure.GraphNode test = new GraphDataStructure.GraphNode("Test");

            Assert.IsTrue(test.Address.Equals("Test"));
        }

        [TestMethod]
        public void Get_Address()
        {
            GraphDataStructure.GraphNode test = new GraphNode("addressTest");

            Assert.IsTrue(test.Address.Equals("addressTest"));
        }

        [TestMethod]
        public void Set_Address()
        {
            GraphDataStructure.GraphNode test = new GraphNode();

            test.Address = "addressTest";
            Assert.IsTrue(test.Address.Equals("addressTest"));
        }
    }
}
