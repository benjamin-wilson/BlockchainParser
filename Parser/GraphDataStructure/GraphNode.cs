using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDataStructure
{
    public class GraphNode
    {
        private string _address;
        private LinkedList<GraphNode> _neighbors;
        private List<decimal> _value; // This is used to create a weighted graph

        public GraphNode()
        {
            this._address = null;
            this._neighbors = new LinkedList<GraphNode>();
        }

        public GraphNode(string data)
        {
            this._address = data;
            this._neighbors = new LinkedList<GraphNode>();
        }

        public GraphNode(string data, LinkedList<GraphNode> neighbors)
        {
            this._address = data;
            this._neighbors = neighbors;
        }

        public string Address
        {
            set { this._address = value; }
            get { return this._address; }
        }

        public LinkedList<GraphNode> Neighbors
        {
            set { this._neighbors = value; }
            get { return this._neighbors; }
        }
    }
}
