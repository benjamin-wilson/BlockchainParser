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
        private LinkedList<GraphNode> _nodeSet;

        public Graph()
        {
            this._nodeSet = new LinkedList<GraphNode>();
        }
        public Graph(LinkedList<GraphNode> set)
        {
            if (set == null)
            {
                this._nodeSet = new LinkedList<GraphNode>();
            }
            else
            {
                this._nodeSet = set;
            }
        }

        public LinkedList<GraphNode> NodeSet
        {
            get {return this._nodeSet;}
        }

        public void addNode(string address)
        {
            GraphNode node = new GraphNode(address);
            if (!nodeExists(node))
            {
                this._nodeSet.AddLast(node);
            }

        }

        public void addDirectedEdge(string from, string to, decimal cost)
        {
            GraphNode fromNode = new GraphNode(from);
            GraphNode toNode = new GraphNode(to);

            this._nodeSet.ElementAt(getGraphNode(fromNode)).addNeighbor(this._nodeSet.ElementAt(getGraphNode(toNode)), cost);
        }

        private bool nodeExists(GraphNode node)
        {
            if (getGraphNode(node) == -1)
            {
                return false;
            }
            else
                return true;
        }

        public int getGraphNode(GraphNode node)
        {
            for (int i = 0; i < this._nodeSet.Count(); i++)
            {
                if (this._nodeSet.ElementAt(i).Address.Equals(node.Address))
                {
                    return i;
                }
            }
            return -1;
        }

        public bool exists(GraphNode node)
        {
            return this._nodeSet.Find(node) != null;
        }

        public void removeNode(GraphNode node)
        {
            int index = getGraphNode(node);
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

                var sendersList = database.getSentTo(currentAddress);
                var reciverList = database.getRecivedFrom(currentAddress);

                foreach (var sender in sendersList)
                {
                    nextDegree.Enqueue(sender.target);
                    graph.addNode(sender.target);
                    graph.addDirectedEdge(sender.source, sender.target, -Convert.ToDecimal(sender.value));
                }

                foreach (var reciver in reciverList)
                {
                    nextDegree.Enqueue(reciver.source);
                    graph.addNode(reciver.source);
                    graph.addDirectedEdge(reciver.source, reciver.target, Convert.ToDecimal(reciver.value));
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
    }
}
