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

        public void addNode(string address, int degree)
        {
            Node node = new Node(address);
            this._nodeList.Add(node);
            node.Degree = degree;
            this._index.Add(address, this._nodeList.Count-1);
        }

        public void addDirectedEdge(string from, string to, decimal value, uint weight, int degree)
        {
            this._nodeList.ElementAt(getNode(from)).addNeighbor(this._nodeList.ElementAt(getNode(to)), value, weight, degree);
        }

        public int getNode(string nodeAddress)
        {
            return this._index[nodeAddress];
        }

        public void writeJSONToFile(string fileName, string data)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName, true))
            {
                file.Write(data);
            }
        }
        public string buildNodeJsonString()
        {
            StringBuilder jsonString = new StringBuilder("\"nodes\": [ ");
            bool firstNodeSet = true;
            foreach (var node in this._nodeList)
            {
                if (firstNodeSet)
                {
                    jsonString.Append("{\"name\":\"" + node.Address + "\",\"group\":" + node.Degree + "}");
                    firstNodeSet = false;
                }
                else
                {
                    jsonString.Append(",{\"name\":\"" + node.Address + "\",\"group\":" + node.Degree + "}");
                }
            }
            jsonString.Append("],");


            return jsonString.ToString();
        }
        public string buildLinkJsonString()
        {
            StringBuilder jsonString = new StringBuilder("\"links\": [");
            bool firstNode = true;

            for (int i = 0; i < this._nodeList.Count;i++ )
            {
                for(int j = 0; j < this._nodeList.ElementAt(i).Neighbors.Count; j++)
                {
                    if (firstNode)
                    {
                        firstNode = false;
                        jsonString.Append("{\"source\":" + i + ",\"target\":" + getNode(this._nodeList.ElementAt(i).Neighbors.ElementAt(j).Target.Address) + ",\"value\":" + 1 + "}");
                    }
                    else
                    {
                        jsonString.Append(",{\"source\":" + i + ",\"target\":" + getNode(this._nodeList.ElementAt(i).Neighbors.ElementAt(j).Target.Address) + ",\"value\":" + 1 + "}");
                    }
                }
            }
            jsonString.Append("]");
            return jsonString.ToString();
        }
        public string buildJsonString()
        {
            StringBuilder jsonString = new StringBuilder("{");

            jsonString.Append(buildNodeJsonString());
            jsonString.Append(buildLinkJsonString());
            jsonString.Append("}");
            
            return jsonString.ToString();
        }

        public static Graph populate(string publicAddress, int degree, int tooBigToAddToNetwork)
        {
            DBConnect database = new DBConnect();
            database.OpenConnection();
            database.SetMaxAddressQuerryTime(500);
            Graph graph = new Graph();
            int count = 0;

            
            Queue<string> currentDegree = new Queue<string>();
            Queue<string> nextDegree = new Queue<string>();
            

            graph.addNode(publicAddress, 0);
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
                    if (sendersList != null && reciverList != null)
                    {
                        Console.WriteLine(" Proc " + (sendersList.Count + reciverList.Count).ToString() + " trans");
                        if (sendersList.Count < tooBigToAddToNetwork)
                        {
                            foreach (Transaction sender in sendersList)
                            {
                                if (!graph._index.ContainsKey(sender.target))
                                {
                                    nextDegree.Enqueue(sender.target);
                                    graph.addNode(sender.target, count + 1);
                                    //graph.addDirectedEdge(sender.source, sender.target, Convert.ToDecimal(sender.value), sender.weight, count+1);
                                }
                                //else
                                {
                                    graph.addDirectedEdge(sender.source, sender.target, Convert.ToDecimal(sender.value), sender.weight, count + 1);
                                }
                            }

                            foreach (Transaction reciver in reciverList)
                            {
                                if (!graph._index.ContainsKey(reciver.source))
                                {
                                    nextDegree.Enqueue(reciver.source);
                                    graph.addNode(reciver.source, count + 1);
                                    //graph.addDirectedEdge(reciver.source, reciver.target, Convert.ToDecimal(reciver.value), reciver.weight, count+1);
                                }
                                //else
                                {
                                    graph.addDirectedEdge(reciver.source, reciver.target, Convert.ToDecimal(reciver.value), reciver.weight, count + 1);
                                }
                            }
                        }
                    }
                }
                Console.WriteLine(currentDegree.Count+" queries to go");
                if (currentDegree.Count <= 0)
                {
                    count++;
                    currentDegree = nextDegree;
                    nextDegree = new Queue<string>();
                }
            }
            database.CloseConnection();
            return graph;
        }

        public void trim(int maxNumberOfNodes)
        {
            int weightMinimum = 1;
            while (this._nodeList.Count > maxNumberOfNodes)
            {
                List<Node> nodesToDelete = new List<Node>();
                foreach (Node node in this._nodeList)
                {
                    if ((node.Weight < weightMinimum) || node.Neighbors.Count <= 0)
                    {
                        nodesToDelete.Add(node);
                    }
                }
                foreach(Node nodeToDelete in nodesToDelete)
                {
                    this._nodeList.Remove(nodeToDelete);
                }
                reIndex();
                removeNeighbors();
                weigh();
                weightMinimum++;
            }
        }

        public void weigh()
        {
            foreach(var node in this._nodeList)
            {
                foreach(var neighbor in node.Neighbors)
                {
                    
                    this._nodeList.ElementAt(getNode(neighbor.Target.Address)).Weight = neighbor.Weight;
                }
            }
        }
    }
}
