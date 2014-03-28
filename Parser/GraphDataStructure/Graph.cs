using System;
using System.Collections.Generic;
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

        public void addGraphNode(GraphNode node)
        {
            this._nodeSet.AddLast(node);
        }

        public void addGraphNode(string node)
        {
            this._nodeSet.AddLast(new GraphNode(node));
        }

        public void addDirectedEdge(GraphNode from, GraphNode to)
        {
            from.Neighbors.AddFirst(to);
        }

        public void addDirectedEdge(string from, string to)
        {
            GraphNode fromNode = new GraphNode(from);
            GraphNode toNode = new GraphNode(to);

            this._nodeSet.ElementAt(getGraphNode(fromNode)).Neighbors.AddFirst(this._nodeSet.ElementAt(getGraphNode(toNode)));
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
            return 0;
        }

        public bool exists(GraphNode node)
        {
            return this._nodeSet.Find(node) != null;
        }

        public void removeNode(GraphNode node)
        {
            var nodeToRemove = this._nodeSet.ElementAt(getGraphNode(node));

            if (nodeToRemove != null)
            {
                this._nodeSet.Remove(nodeToRemove);

                foreach (var item in this._nodeSet)
                {
                    item.Neighbors.Remove(nodeToRemove);
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
                    Console.Write(neighbor.Address + "---->");
                }

                Console.WriteLine();
            }
        }
    }
}
