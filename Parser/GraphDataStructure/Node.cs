using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDataStructure
{
    public class Node
    {
        private string _address;
        private List<Edge> _neighbors;
         // This is used to create a weighted graph

        public Node()
        {
            this._address = null;
            this._neighbors = new List<Edge>();
            //this._value = new List<decimal>();
        }

        public Node(string data)
        {
            this._address = data;
            this._neighbors = new List<Edge>();
            //this._value = new List<decimal>();
        }

        public Node(string data, List<Edge> neighbors)
        {
            this._address = data;
            this._neighbors = neighbors;
            //this._value = new List<decimal>();
        }

        public string Address
        {
            set { this._address = value; }
            get { return this._address; }
        }

        public List<Edge> Neighbors
        {
            set { this._neighbors = value; }
            get { return this._neighbors; }
        }

        public void addNeighbor(Node neighbor, decimal cost, uint weight)
        {
            Edge newEdge = new Edge(neighbor, cost, weight);
            this._neighbors.Add(newEdge);
        }

        public void removeEdge(Node target)
        {
            var index = getGraphNode(target);
            if(index > -1)
                this._neighbors.ElementAt(index);
        }

        public int getGraphNode(Node node)
        {
            for (int i = 0; i < this._neighbors.Count(); i++)
            {
                if (this._neighbors.ElementAt(i).Target.Address.Equals(node.Address))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
