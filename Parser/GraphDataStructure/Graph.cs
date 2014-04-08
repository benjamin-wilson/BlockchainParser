using Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDataStructure
{
    public class Graph
    {
        private LinkedList<Node> _nodeSet;

        public Graph()
        {
            this._nodeSet = new LinkedList<Node>();
        }
        public Graph(LinkedList<Node> set)
        {
            if (set == null)
            {
                this._nodeSet = new LinkedList<Node>();
            }
            else
            {
                this._nodeSet = set;
            }
        }

        public LinkedList<Node> NodeSet
        {
            get {return this._nodeSet;}
        }

        public void addNode(string address)
        {
            Node node = new Node(address);
            if (!nodeExists(node))
            {
                this._nodeSet.AddLast(node);
            }

        }

        public void addDirectedEdge(string from, string to, decimal cost)
        {
            this._nodeSet.ElementAt(getNode(from)).addNeighbor(this._nodeSet.ElementAt(getNode(to)), cost);
        }

        private bool nodeExists(Node node)
        {
            if (getNode(node.Address) == -1)
            {
                return false;
            }
            else
                return true;
        }

        public int getNode(string nodeAddress)
        {
            for (int i = 0; i < this._nodeSet.Count(); i++)
            {
                if (this._nodeSet.ElementAt(i).Address.Equals(nodeAddress))
                {
                    return i;
                }
            }
            return -1;
        }

        public bool exists(Node node)
        {
            return this._nodeSet.Find(node) != null;
        }

        public void removeNode(string address)
        {
            int index = getNode(address);
            var nodeToRemove = this._nodeSet.ElementAt(index);

            if (nodeToRemove != null)
            {
                this._nodeSet.Remove(nodeToRemove);

                foreach (var item in this._nodeSet)
                {
                    item.removeEdge(nodeToRemove);
                }
            }
        }

        public void displayList()
        {
            foreach (var gnode in this._nodeSet)
            {
                Console.Write(gnode.Address + ":---->");

                foreach (var neighbor in gnode.Neighbors)
                {
                    Console.Write(neighbor.Target.Address + "---->");
                }

                Console.WriteLine();
            }
        }

        public void writeListToFile(string fileName)
        {
            foreach (var gnode in this._nodeSet)
            {
                int count = 0;
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName, true))
                {
                    file.Write(gnode.Address + ":---->");

                    foreach (var neighbor in gnode.Neighbors)
                    {
                        file.Write(neighbor.Target.Address + ":" + neighbor.Weight + ":"+ neighbor.Value + " , ");
                        count++;
                    }

                    file.WriteLine();
                }
            }
        }

        public string buildJsonString()
        {
            StringBuilder jsonString = new StringBuilder("{ \"nodeSet\": [ ");
            bool firstNeighborElement = true;
            bool firstNodeSet = true;
            
            foreach (var node in this._nodeSet)
            {
                if(firstNodeSet)
                {
                    jsonString.Append("{\"address\":\"" + node.Address + "\",");
                    firstNodeSet = false;
                }
                else
                    jsonString.Append(",{\"address\":\"" + node.Address + "\",");

                jsonString.Append("\"neighbors\":[");

                foreach(var neighbor in node.Neighbors)
                {
                    if (firstNeighborElement)
                    {
                        jsonString.Append("\"" + neighbor.Target.Address + "\"");
                        firstNeighborElement = false;
                    }
                    else
                    {
                        jsonString.Append(",\"" + neighbor.Target.Address + "\"");
                    }
                }

                jsonString.Append("]");
                jsonString.Append("}");
            }
            jsonString.Append("] }");
            return jsonString.ToString();
        }

        public static Graph populate(string publicAddress, int degree)
        {
            DBConnect database = new DBConnect();
            Graph graph = new Graph();
            int count = 0;
            
            Queue<string> currentDegree = new Queue<string>();
            Queue<string> nextDegree = new Queue<string>();


            graph.addNode(publicAddress);
            currentDegree.Enqueue(publicAddress);

            while (count < degree) 
            {
                string currentAddress = currentDegree.Dequeue();
                Console.WriteLine(currentAddress.ToString() + " " + count.ToString());

                var sendersList = database.getSentTo(currentAddress);
                var reciverList = database.getRecivedFrom(currentAddress);

                if (sendersList.Count <= 1000 && reciverList.Count <= 1000)
                {
                    foreach (var sender in sendersList)
                    {
                        if (graph.addressDoesNotExist(sender.target))
                        {
                            nextDegree.Enqueue(sender.target);
                            graph.addNode(sender.target);
                            graph.addDirectedEdge(sender.source, sender.target, Convert.ToDecimal(sender.value));
                        }
                        else
                        {
                            graph.addDirectedEdge(sender.source, sender.target, Convert.ToDecimal(sender.value));
                        }
                    }

                    foreach (var reciver in reciverList)
                    {
                        if (graph.addressDoesNotExist(reciver.source))
                        {
                            nextDegree.Enqueue(reciver.source);
                            graph.addNode(reciver.source);
                            graph.addDirectedEdge(reciver.source, reciver.target, Convert.ToDecimal(reciver.value));
                        }
                        else
                        {
                            graph.addDirectedEdge(reciver.source, reciver.target, Convert.ToDecimal(reciver.value));
                        }
                    }
                }
                if (currentDegree.Count <= 0)
                {
                    count++;
                    currentDegree = nextDegree;
                    nextDegree = new Queue<string>();
                }
            }

            return graph;
        }

        public void pathTrim(int depth)
        {
            List<Node> nodesToRemove = new List<Node>();
            foreach(Node node in this._nodeSet)
            {
                if(!pathFind(node, node.Neighbors, depth))
                {
                    nodesToRemove.Add(node);
                }
            }
            foreach (Node node in nodesToRemove)
            {
                removeNode(node.Address);
            }
        }

        public void quickTrim()
        {
            List<Node> nodesToRemove = new List<Node>();
            foreach(Node node in this._nodeSet)
            {
                if(node.Neighbors.Count <= 1)
                {
                    nodesToRemove.Add(node);
                }
            }
            foreach(Node node in nodesToRemove)
            {
                removeNode(node.Address);
            }
        }

        public bool pathFind(Node node, LinkedList<Edge> edges, int depth)
        {
            int currentDepth = depth - 1;
            if(edges.Count <= 0 || depth <= 0)
            {
                return false;
            }
            else
            {
                foreach(Edge edge in edges)
                {
                    LinkedList<Edge> neighbors = edge.Target.Neighbors;
                    foreach(Edge neighbor in neighbors)
                    {
                        if (neighbor.Target.Address == node.Address) 
                        {
                            return true;
                        }
                        else
                        {
                            return pathFind(node, neighbors, currentDepth);
                        }
                    }
                }
            }
            return false;
        }

        public bool addressDoesNotExist(string address)
        {
            foreach(var item in this._nodeSet)
            {
                if(item.Address == address)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
