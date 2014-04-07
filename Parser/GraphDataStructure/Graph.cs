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
        private static List<string> _previousAddresses; //Addresses that have already been search, will be used to skip them in the future

        public Graph()
        {
            this._nodeSet = new LinkedList<GraphNode>();
            _previousAddresses = new List<string>();
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
        public void addGraphNode(GraphNode node)
        {
            this._nodeSet.AddLast(node);
        }

        public void addGraphNode(string node)
        {
            this._nodeSet.AddLast(new GraphNode(node));
        }

        public void addDirectedEdge(GraphNode from, GraphNode to, decimal cost)
        {
            from.addNeighbor(to, cost);
        }

        public void addDirectedEdge(string from, string to, decimal cost)
        {
            GraphNode fromNode = new GraphNode(from);
            GraphNode toNode = new GraphNode(to);

            this._nodeSet.ElementAt(getGraphNode(fromNode)).addNeighbor(this._nodeSet.ElementAt(getGraphNode(toNode)), cost);
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

        public void removeEndNodes()
        {
            var endNodes = findEndNodes();

            foreach(var node in endNodes)
            {
                removeNode(node);
            }
        }

        public List<GraphNode> findEndNodes()
        {
            var nodesToRemoveList = new List<GraphNode>();

            for (int i = 0; i < this._nodeSet.Count; i++ )
            {
                var node = this._nodeSet.ElementAt(i);

                if (node.Neighbors.Count < 2 || node == null)
                {
                    nodesToRemoveList.Add(node);
                }
            }

            return nodesToRemoveList;
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
                        file.Write(neighbor.Target.Address + ":" + neighbor.Weight +" , ");
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
            int count = 0; //Used For testing
            
            Queue<string> nextAddresses = new Queue<string>();
            GraphDataStructure.Graph graphList = new GraphDataStructure.Graph();

            graphList.addGraphNode(publicAddress);
            nextAddresses.Enqueue(publicAddress);
            _previousAddresses.Add(publicAddress);

            //while (nextAddresses.Count > 0)
            while (count < degree && nextAddresses.Count != 0) //Used for testing
            {
                Database.DBConnect getLists = new Database.DBConnect();
                string current = nextAddresses.Dequeue();

                var sendersList = getLists.getSentTo(current);
                var reciverList = getLists.getRecivedFrom(current);

                foreach (var sender in sendersList)
                {
                    if(!_previousAddresses.Contains(sender.target))
                    {
                        nextAddresses.Enqueue(sender.target);
                        _previousAddresses.Add(sender.target);
                        graphList.addGraphNode(sender.target);
                    }
                    
                    graphList.addDirectedEdge(sender.source, sender.target, Convert.ToDecimal(sender.value));
                    // Console.WriteLine(sender.source + "     " + sender.target);
                }

                foreach (var reciver in reciverList)
                {
                    if(!_previousAddresses.Contains(reciver.source))
                    { 
                        nextAddresses.Enqueue(reciver.source);
                        _previousAddresses.Add(reciver.source);
                        graphList.addGraphNode(reciver.source);
                        graphList.addDirectedEdge(reciver.source, reciver.target, Convert.ToDecimal(reciver.value));
                    }
                    //  Console.WriteLine(reciver.source + "    " + reciver.target);
                }

                //if(count%100 == 0)
                {
                    Console.WriteLine("Still Alive: " + count);
                }
                count++;
            }

            return graphList;
        }
    }
}
