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
        private List<Node> _nodeList;

        public Graph()
        {
            this._nodeList = new List<Node>();
        }

        public List<Node> NodeSet
        {
            get {return this._nodeList;}
        }

        public void addNode(string address)
        {
            Node node = new Node(address);
            this._nodeList.Add(node);

        }

        public void addDirectedEdge(string from, string to, decimal value, uint weight)
        {
            this._nodeList.ElementAt(getNode(from)).addNeighbor(this._nodeList.ElementAt(getNode(to)), value,weight);
        }

        public int getNode(string nodeAddress)
        {
            for (int i = 0; i < this._nodeList.Count(); i++)
            {
                if (this._nodeList.ElementAt(i).Address.Equals(nodeAddress))
                {
                    return i;
                }
            }
            return -1;
        }

        public void removeNode(string address)
        {
            int index = getNode(address);
            var nodeToRemove = this._nodeList.ElementAt(index);

            if (nodeToRemove != null)
            {
                this._nodeList.Remove(nodeToRemove);

                foreach (var item in this._nodeList)
                {
                    item.removeEdge(nodeToRemove);
                }
            }
        }

        public void displayList()
        {
            foreach (var gnode in this._nodeList)
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
            foreach (var gnode in this._nodeList)
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
            
            foreach (var node in this._nodeList)
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
                Console.Write(currentAddress.ToString() + " Deg: " + count.ToString() + " Qrying ");
                List<Transaction> sendersList = database.getSentTo(currentAddress);
                List<Transaction> reciverList = database.getRecivedFrom(currentAddress);
                Console.WriteLine(" Proc " + (sendersList.Count+reciverList.Count).ToString()+" trans");
                foreach (Transaction sender in sendersList)
                {
                    if (!graph.addressExits(sender.target))
                    {
                        nextDegree.Enqueue(sender.target);
                        graph.addNode(sender.target);
                        graph.addDirectedEdge(sender.source, sender.target, Convert.ToDecimal(sender.value), sender.weight);
                    }
                    else
                    {
                        graph.addDirectedEdge(sender.source, sender.target, Convert.ToDecimal(sender.value), sender.weight);
                    }
                }

                foreach (Transaction reciver in reciverList)
                {
                    if (!graph.addressExits(reciver.source))
                    {
                        nextDegree.Enqueue(reciver.source);
                        graph.addNode(reciver.source);
                        graph.addDirectedEdge(reciver.source, reciver.target, Convert.ToDecimal(reciver.value), reciver.weight);
                    }
                    else
                    {
                        graph.addDirectedEdge(reciver.source, reciver.target, Convert.ToDecimal(reciver.value), reciver.weight);
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
            foreach(Node node in this._nodeList)
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
            foreach(Node node in this._nodeList)
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

        public bool pathFind(Node node, List<Edge> edges, int depth)
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
                    List<Edge> neighbors = edge.Target.Neighbors;
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

        public bool addressExits(string address)
        {
            foreach(Node node in this._nodeList)
            {
                if(node.Address == address)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
