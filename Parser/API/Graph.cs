using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database;

namespace API
{
    public class Graph
    {
        public Graph()
        {
            nodes = new List<Node>();
            links = new List<Link>();
        }
        public List<Node> nodes;
        public List<Link> links;

        public static Graph getNodes(string address)
        {
            Graph graph = new Graph();
            List<Node> nodes = new List<Node>();
            Node firstNode = new Node();
            firstNode.receivingPublicAddresses = address;
            firstNode.group = 0;
            nodes.Add(firstNode);
            List<MySQLOutput> recived = Querrys.getRecivedFromOutputs(address);
            foreach (MySQLOutput output in recived)
            {
                Node node = new Node();
                node.receivingPublicAddresses = output.publicAddress;
                node.group = 1;
                graph.nodes.Add(node);
            }
            List<MySQLOutput> sent = Querrys.getSentToOutputs(address);
            foreach (MySQLOutput output in sent)
            {
                Node node = new Node();
                node.receivingPublicAddresses = output.publicAddress;
                node.group = 2;
                graph.nodes.Add(node);
            }


            List<Link> links = new List<Link>();

            foreach (MySQLOutput output in recived)
            {
                Link link = new Link();
                link.source = output.publicAddress;
                link.target = address;
                link.value = 1;
                graph.links.Add(link);
            }
            foreach (MySQLOutput output in sent)
            {
                Link link = new Link();
                link.source = address;
                link.target = output.publicAddress;
                link.value = 2;
                graph.links.Add(link);
            }
            return graph;
        }
    }
    public class Link
    {
        public string source;
        public string target;
        public uint value;
    }
    public class Node
    {
        public string receivingPublicAddresses;
        public uint group;
    }

    
}
