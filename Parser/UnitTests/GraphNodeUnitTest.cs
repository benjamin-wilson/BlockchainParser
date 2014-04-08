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
            GraphDataStructure.Node test = new GraphDataStructure.Node();

            Assert.IsNotNull(test);
        }
        [TestMethod]
        public void Constructor_2()
        {
            GraphDataStructure.Node test = new GraphDataStructure.Node("Test");

            Assert.IsTrue(test.Address.Equals("Test"));
        }

        [TestMethod]
        public void Get_Address()
        {
            GraphDataStructure.Node test = new Node("addressTest");

            Assert.IsTrue(test.Address.Equals("addressTest"));
        }

        [TestMethod]
        public void Set_Address()
        {
            GraphDataStructure.Node test = new Node();

            test.Address = "addressTest";
            Assert.IsTrue(test.Address.Equals("addressTest"));
        }
    }
}
