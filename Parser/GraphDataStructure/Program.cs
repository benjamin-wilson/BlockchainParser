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
            Graph network = new Graph();
            GraphNode temp1 = new GraphNode("People.aspx");
            GraphNode temp2 = new GraphNode("Privacy.htm");

            network.addGraphNode("Privacy.htm");
            network.addGraphNode("People.aspx");
            network.addGraphNode("About.htm");
            network.addGraphNode("Index.htm");
            network.addGraphNode("Products.aspx");
            network.addGraphNode("Contact.aspx");

            network.addDirectedEdge("People.aspx", "Privacy.htm");  // People -> Privacy

            network.addDirectedEdge("Privacy.htm", "Index.htm");    // Privacy -> Index
            network.addDirectedEdge("Privacy.htm", "About.htm");    // Privacy -> About

            network.addDirectedEdge("About.htm", "Privacy.htm");    // About -> Privacy
            network.addDirectedEdge("About.htm", "People.aspx");    // About -> People
            network.addDirectedEdge("About.htm", "Contact.aspx");   // About -> Contact

            network.addDirectedEdge("Index.htm", "About.htm");      // Index -> About
            network.addDirectedEdge("Index.htm", "Contact.aspx");   // Index -> Contacts
            network.addDirectedEdge("Index.htm", "Products.aspx");  // Index -> Products

            network.addDirectedEdge("Products.aspx", "Index.htm");  // Products -> Index
            network.addDirectedEdge("Products.aspx", "People.aspx");// Products -> People

            network.displayList();

            network.removeNode(new GraphNode("Contact.aspx"));
            network.removeNode(new GraphNode("Products.aspx"));

            Console.WriteLine();
            network.displayList();
        }
    }
}
