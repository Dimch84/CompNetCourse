using System;
using System.Collections.Generic;
using System.Linq;

namespace RoutingSimulator.Core
{
    public class Node<T> : IEquatable<Node<T>>, IComparable<Node<T>>
                 where T : IComparable<T>, IEquatable<T>
    {
        private ISet<Node<T>> _neighbours;
        private IDictionary<Node<T>, long> _distances;
        private IDictionary<Node<T>, DistanceVector<T>> _neighbourDistanceVectors;

        public IEnumerable<Node<T>> Neighbours
        {
            get
            {
                return _neighbours;
            }
        }


        private DistanceVector<T> _distanceVector;
        public T Value { get; private set; }
        public DistanceVector<T> DistanceVector
        {
            get
            {
                return _distanceVector;
            }
            set
            {
                _distanceVector = value;
                //DistanceVectorUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler DistanceVectorUpdated;

        public Node(T value)
        {
            Value = value;
            _distanceVector = new DistanceVector<T>(this);
            _neighbours = new HashSet<Node<T>>();
            _distances = new Dictionary<Node<T>, long>();
            _neighbourDistanceVectors = new Dictionary<Node<T>, DistanceVector<T>>();
        }

        public void AddNeighbour(Node<T> node, long distance)
        {
            _neighbours.Add(node);
            _distances.Add(node, distance);
            _neighbourDistanceVectors.Add(node, node.DistanceVector);
            node.DistanceVectorUpdated += UpdateNeighbourDistanceVector;
            UpdateDistanceVector();
        }

        private void UpdateNeighbourDistanceVector(object sender, EventArgs e)
        {
            var node = sender as Node<T>;
            _neighbourDistanceVectors[node] = node.DistanceVector;
            UpdateDistanceVector();
        }

        public void RemoveNeighbour(Node<T> node)
        {
            if (!_neighbours.Contains(node))
                return;
            node.DistanceVectorUpdated -= UpdateNeighbourDistanceVector;
            _neighbours.Remove(node);
            _distances.Remove(node);
            _neighbourDistanceVectors.Remove(node);

            UpdateDistanceVector();
        }

        private void UpdateDistanceVector()
        {
            var distanceVector = new DistanceVector<T>(this);
            List<Node<T>> destinations = _neighbours.SelectMany(
                                                    x => x.DistanceVector
                                                          .Entries
                                                          .Select(y => y.Destination)
                                                    ).Where(x => x != this).Distinct().ToList();
            foreach(var destination in destinations)
            {
                long bestDist = long.MaxValue;
                Node<T> next = null;
                foreach(var neighbour in _neighbours)
                {
                    long distToNeigh = _distances[neighbour];
                    var distToDest = neighbour.DistanceVector
                                               .Entries
                                               .Where(x => x.Destination == destination)
                                               .Where(x => x.Next != this)
                                               .Select(x => x.Distance);
                    if (distToDest.Count() == 0)
                        continue;
                    if (distToNeigh + distToDest.First() < bestDist)
                    {
                        bestDist = distToNeigh + distToDest.First();
                        next = neighbour;
                    }
                }
                if(next != null)
                    distanceVector.AddEntry(destination, bestDist, next);
            }
            _distanceVector = distanceVector;
        }

        public void Tick()
        {
            DistanceVectorUpdated?.Invoke(this, EventArgs.Empty);
        }
        
        public int CompareTo(Node<T> other)
        {
            return Value.CompareTo(other.Value);
        }

        public bool Equals(Node<T> other)
        {
            return Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
