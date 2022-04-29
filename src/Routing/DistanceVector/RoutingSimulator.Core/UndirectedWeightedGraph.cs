using RoutingSimulator.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoutingSimulator.Core
{
    public class UndirectedWeightedGraph<TNode> where TNode : IComparable<TNode>, IEquatable<TNode>
    {
        private ISet<Node<TNode>> _nodes;

        public IEnumerable<Node<TNode>> Nodes
        {
            get
            {
                return _nodes;
            }
        }

        public UndirectedWeightedGraph()
        {
            _nodes = new HashSet<Node<TNode>>();
        }

        public bool AddNode(Node<TNode> node)
        {
            if (_nodes.Contains(node))
                return false;
            _nodes.Add(node);
            return true;
        }

        public bool RemoveNode(Node<TNode> node)
        {
            if (!_nodes.Contains(node))
                return false;
            _nodes.Remove(node);
            foreach(var neigh in node.Neighbours)
            {
                neigh.RemoveNeighbour(node);
            }
            return true;
        }

        /// <summary>
        /// Adds a bidirectional edge between node1 and node2
        /// </summary>
        public void AddEdge(Node<TNode> node1, Node<TNode> node2, long weight)
        {
            if (!_nodes.Contains(node1) || !_nodes.Contains(node2))
                throw new NodeDoesNotExistException("The specified node does not exist in this graph.");
            node1.AddNeighbour(node2, weight);
            node2.AddNeighbour(node1, weight);
        }

        public void RemoveEdge(Node<TNode> node1, Node<TNode> node2)
        {
            if (!_nodes.Contains(node1) || !_nodes.Contains(node2))
                return;
            node1.RemoveNeighbour(node2);
            node2.RemoveNeighbour(node1);
        }

        public void Tick()
        {
            foreach (var node in _nodes)
                node.Tick();
        }

        public IEnumerable<Node<TNode>> FindShortestPath(Node<TNode> from, Node<TNode> to)
        {
            var path = new List<Node<TNode>>();
            while(from != to)
            {
                path.Add(from);
                from = from.DistanceVector.Entries.Where(x => x.Destination == to).Select(x => x.Next).FirstOrDefault();
                if (from == null)
                    throw new PathDoesNotExistException("The specified path does not exist");
            }
            path.Add(to);
            return path;
        }
    }
}
