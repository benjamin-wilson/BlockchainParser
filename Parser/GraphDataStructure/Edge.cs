using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDataStructure
{
    public class Edge
    {
        private Node _neighbor;
        private uint _weight;
        private decimal _value;

        public Edge(Node target, decimal value)
        {
            this._neighbor = target;
            this._value = value;
            _weight = 1;
        }

        public uint Weight
        {
            get { return this._weight; }
        }

        public Node Target
        {
            get { return this._neighbor; }
        }

        public decimal Value 
        {
            get { return this._value; }
        }

        public bool findEdge(Node nodeToFind)
        {
            if (nodeToFind.Address.Equals(this._neighbor.Address))
                return true;

            return false;
        }

        public void addValue(decimal value)
        {
            this._value += value;
        }

        public void addWeight()
        {
            this._weight++;
        }
        public void removeWeight()
        {
            this._weight--;
        }
    }
}
