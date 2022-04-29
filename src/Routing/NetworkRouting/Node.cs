namespace NetworkRouting
{
    public class Node
    {
        public int id;
        public int index;
        public double value;
        public Node prev = null;

        public Node(int id, double value, int index)
        {
            this.id = id;
            this.value = value;
            this.index = index;
        }
    }

}
