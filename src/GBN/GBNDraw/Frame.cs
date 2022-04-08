using System.Threading;

namespace goBackN
{
    public class Frame
    {
        public int Id { get; set; }

        public Frame(int id)
        {
            this.Id = id;
        }

        public Timer Timer;
    }
}
