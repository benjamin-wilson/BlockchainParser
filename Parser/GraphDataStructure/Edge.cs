using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDataStructure
{
    public class Edge
    {
        private GraphNode _neighbor;
        private decimal _weight;

        public Edge(GraphNode target, decimal value)
        {
            this._neighbor = target;
            this._weight = value;
        }

        public GraphNode Target
        {
            get { return this._neighbor; }
        }

        public decimal Weight
        {
            get { return this._weight; }
        }

        public bool findEdge(GraphNode nodeToFind)
        {
            if (nodeToFind.Address.Equals(this._neighbor.Address))
                return true;

            return false;
        }
    }
}
