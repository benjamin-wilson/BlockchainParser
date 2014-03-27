using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDataStructure
{
    public class GraphNode<T>
    {
        private T data;

        private List<int> costs;

        public GraphNode() { }
        public GraphNode(T data) : this(data, null) { }
        public GraphNode(T value, NodeList<T> neighbors) : base(value, neighbors) { }
    }
}
