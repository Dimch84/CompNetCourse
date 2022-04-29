using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace NetworkRouting
{
    public class PriorityQueue
    {
        private List<Node> heap = new List<Node>();
        private List<Node> pointers = new List<Node>();
        private int degree = 2;

        public PriorityQueue(int numPoints, int? degree = null, bool onePath = false)
        {
            //Create a populated pointer array.
            pointers = new List<Node>( new Node[numPoints] );

            if (onePath == false)
            {
                //If not performing the one-path algorithm, initialize a list of all points.
                for (int i = 0; i < numPoints; i++)
                {
                    insert(i, double.PositiveInfinity);
                }
            }

            if (degree != null)
            {
                this.degree = (int)degree;
            }
        }

        private int getParentIndex(int index)
        {
            //Returns the parent node of a given child node.
            return (int)Math.Floor((double)((index - 1) / degree)); 
        }

        private int getChildSmallest(int index)
        {
            //Returns the smallest child node of a given parent node.
            int childIndex = (degree * index) + 1;
            if (childIndex > heap.Count - 1)
                return index;

            //Iterate over the two child nodes.
            for(int i = 0; i < degree; i++)
            {
                //If the potential child node falls outside the heap, break.
                if ((degree * index) + 1 + i > heap.Count - 1)
                    break;

                double childDistance = heap[(degree * index) + 1 + i].value;
                //Return the child node with the smallest distance value.
                if (childDistance < heap[childIndex].value)
                    childIndex = (degree * index) + 1 + i;
            }
            return childIndex;
        }

        private void swap(int aIndex, int bIndex)
        {
            //Switches the indexes of two nodes in the queue.
            heap[aIndex].index = bIndex;
            heap[bIndex].index = aIndex;

            Node temp = heap[aIndex];
            heap[aIndex] = heap[bIndex];
            heap[bIndex] = temp;
        }

        private void swapUp(int childIndex)
        {
            //Swap node with its parent if the parent's value is larger.
            int parentIndex = getParentIndex(childIndex);

            while (heap[parentIndex].value > heap[childIndex].value && childIndex != 0)
            {
                swap(childIndex, parentIndex);
                childIndex = parentIndex;
                parentIndex = getParentIndex(childIndex);
            }
        }

        private void swapDown(int parentIndex)
        {
            //Swap a parent with its smallest child if the child is smaller.
            int childIndex = getChildSmallest(parentIndex);

            while (heap[childIndex].value < heap[parentIndex].value)
            {
                swap(childIndex, parentIndex);
                parentIndex = childIndex;
                childIndex = getChildSmallest(parentIndex);
            }
        }

        public Node insert(int id, double distance, Node prev = null)
        {
            //Create a new node.
            Node n = new Node(id, distance, heap.Count);
            //Set the prev value.
            n.prev = prev;
            heap.Add(n);
            pointers[id] = n;

            swapUp(n.index);

            return n;
        }

        public Node insert(Node n, Node prev)
        {
            if (n.index > heap.Count)
                n.index = heap.Count;

            n.prev = prev;
            //O(1)
            heap.Add(n);
            //O(1)
            pointers[n.id] = n;
            
            //Move the node into the appropriate place in the queue. O(log|V|) swaps
            swapUp(n.index);

            return n;
        }

        public void update(int id, double distance, Node prev)
        {
            //Updates the value and prev variables of a node.
            pointers[id].value = distance;
            pointers[id].prev = prev;
            //Move the node into the appropriate place in the queue.
            swapUp(pointers[id].index);
        }

        public Node pop()
        {
            //Returns the lowest value node.
            if (heap.Count() > 0)
            {
                Node least = heap[0];
                //O(1) Swaps the top and bottom node.
                swap(0, heap.Count - 1);

                //Sets the node to removed in the pointer array.
                pointers[heap[0].id].index = -1;

                //Removes the node that originally at the top.
                //O(1) as no shifts are performed when the last element is removed.
                heap.RemoveAt(heap.Count - 1);

                if(heap.Count > 1)
                    //Percolates down the node that was swapped into the top spot. O(log|V|)
                    swapDown(0);

                return least;
            }

            return null;
        }

        public bool isNotEmpty()
        {
            //Checks to see if the priority queue is currently empty.
            if (heap.Count > 0)
                return true;
            return false;
        }

        public Node find(int id)
        {
            //Gets the node by id and returns null if it doesn't exist.
            if (id > pointers.Count - 1)
                return null;
            return pointers[id];
        }

        public Node peek()
        {
            //Returns the top node without removing it.
            return heap[0];
        }

    }
}
