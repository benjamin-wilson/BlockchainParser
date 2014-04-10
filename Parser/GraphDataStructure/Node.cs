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
        private int _degree;
        private uint _weight;
         // This is used to create a weighted graph

        public Node()
        {
            this._address = null;
            this._neighbors = new List<Edge>();
            this._weight = 1;
            //this._value = new List<decimal>();
        }

        public Node(string data)
        {
            this._address = data;
            this._neighbors = new List<Edge>();
            this._weight = 1;
            //this._value = new List<decimal>();
        }

        public Node(string data, List<Edge> neighbors)
        {
            this._address = data;
            this._neighbors = neighbors;
            this._weight = 1;
            //this._value = new List<decimal>();
        }

        public string Address
        {
            set { this._address = value; }
            get { return this._address; }
        }

        public int Degree
        {
            get { return this._degree; }
            set { this._degree = value; }
        }
        public List<Edge> Neighbors
        {
            set { this._neighbors = value; }
            get { return this._neighbors; }
        }

        public uint Weight
        {
            get { return this._weight; }
            set { this._weight = value; }
        }
        public void addNeighbor(Node neighbor, decimal cost, uint weight, int degree)
        {
            Edge newEdge = new Edge(neighbor, cost, weight, degree);
            if (neighborExists(newEdge))
            {
                updateEdge(newEdge);
            }
            else
            {
                this._neighbors.Add(newEdge);
            }
        }

        public bool neighborExists(Edge edgeCheck)
        {
            foreach(var edge in this._neighbors)
            {
                if (edge.Target.Address.Equals(edgeCheck.Target.Address))
                    return true;
            }
            return false;
        }

        public void updateEdge(Edge edge)
        {
            int index = getEdge(edge);

            this._neighbors.ElementAt(index).addValue(edge.Value);
            this._neighbors.ElementAt(index).updateWeight();
        }

        public void removeEdge(Node target)
        {
            var index = getGraphNode(target);
            if(index > -1)
                this._neighbors.ElementAt(index);
        }

        public int getEdge(Edge edge)
        {
            for (int i = 0; i < this._neighbors.Count;i++ )
            {
                if (edge.Target.Address.Equals(this._neighbors.ElementAt(i).Target.Address))
                    return i;
            }
            return -1;
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
