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
        private Dictionary<string, int> _index;
        public Graph()
        {
            this._nodeList = new List<Node>();
            this._index = new Dictionary<string, int>();
        }

        private void reIndex()
        {
            this._index = new Dictionary<string, int>();
            for(int i = 0; i < _nodeList.Count; i++)
            {
                _index.Add(_nodeList[i].Address, i);
            }
        }

        public void removeNeighbors()
        {
            reIndex();
            List<Edge> edgesToRemove = new List<Edge>();
            foreach(Node node in this._nodeList)
            {
                foreach(Edge edge in node.Neighbors)
                {
                    if(!_index.ContainsKey(edge.Target.Address))
                    {
                        edgesToRemove.Add(edge);
                    }
                }
                foreach(Edge remove in edgesToRemove)
                {
                    node.Neighbors.Remove(remove);
                }
            }
        }

        public List<Node> NodeSet
        {
            get {return this._nodeList;}
        }

        public void addNode(string address)
        {
            Node node = new Node(address);
            this._nodeList.Add(node);
            this._index.Add(address, this._nodeList.Count-1);
        }

        public void addDirectedEdge(string from, string to, decimal value, uint weight)
        {
            this._nodeList.ElementAt(getNode(from)).addNeighbor(this._nodeList.ElementAt(getNode(to)), value,weight);
        }

        public int getNode(string nodeAddress)
        {
            return this._index[nodeAddress];
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
                Console.WriteLine("Network has " + graph._nodeList.Count.ToString() + " nodes");
                Console.Write(currentAddress.ToString() + " Deg: " + count.ToString() + " Qrying ");
                if (currentAddress.Substring(0, 5) != "1dice")
                {
                    List<Transaction> sendersList = database.getSentTo(currentAddress);
                    List<Transaction> reciverList = database.getRecivedFrom(currentAddress);

                    Console.WriteLine(" Proc " + (sendersList.Count + reciverList.Count).ToString() + " trans");
                    foreach (Transaction sender in sendersList)
                    {
                        if (!graph._index.ContainsKey(sender.target))
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
                        if (!graph._index.ContainsKey(reciver.source))
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
                }
                Console.WriteLine(currentDegree.Count);
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
                this._nodeList.Remove(node);
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
                this._nodeList.Remove(node);
            }
            removeNeighbors();
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
    }
}
