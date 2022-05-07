using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIP_Emulator
{
    class Edge
    {
        private NetworkNode Left = null, Right = null;       

        public Edge() { }
        public Edge(NetworkNode Left, NetworkNode Right)
        {
            this.Left = Left;
            this.Right = Right;
        }

        public void SetRight(NetworkNode e)
        {
            Right = e;
        }

        public  void SetLeft(NetworkNode e)
        {
            Left = e;
        }

        public NetworkNode GetRight()
        {
            return Right;
        }

        public NetworkNode GetLeft()
        {
            return Left;
        }
    }
}
