using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIP_Emulator
{
    struct RoutingState
    {
        public NetworkNode NextNode;
        public NetworkNode DestinationNode;
        public int Metric;
    }

    class RoutingTable
    {      
        private static object ThreadLock = new object(); //A lock to keep thread output synchronized
        private const int MAX_HOPS = 15; //IOS fails on 15 hops

        private NetworkNode ParentNode;
        private Network CurrentNetwork;
        private volatile List<RoutingState> States = new List<RoutingState>();

        public RoutingTable() { }
        public RoutingTable(NetworkNode node, Network net)
        {
            CurrentNetwork = net;
            ParentNode = node;
            InitStates();
        }
     
        private void InitStates()
        {
            States.Clear();

            foreach(NetworkNode node in CurrentNetwork.GetNetworkNodes())
            {
                if (node == ParentNode)
                    continue;

                NetworkNode nextNode = new NetworkNode();
                int targetMetric = MAX_HOPS + 1;

                foreach (Edge en in ParentNode.GetEdges())
                {
                    if(en.GetLeft() == node)
                    {
                        if (en.GetLeft().GetIP() == "0.0.0.0")
                            continue;

                        nextNode = en.GetLeft();
                        targetMetric = 1;
                        break;
                    }

                    if (en.GetRight() == node)
                    {
                        if (en.GetRight().GetIP() == "0.0.0.0")
                            continue;

                        nextNode = en.GetRight();
                        targetMetric = 1;
                        break;
                    }
                }

                States.Add(new RoutingState()
                {
                    NextNode = nextNode,
                    DestinationNode = node,
                    Metric = targetMetric
                });
            }
        }

        public List<RoutingState> GetRoutingStates()
        {
            return States;
        }

        public void PrintTable(int step=-1)
        {
            lock(ThreadLock)
            {
                if (step != -1)
                    Console.WriteLine("Simulation step {0} of router {1}", step, ParentNode.GetIP());

                Console.WriteLine("[Source IP]\t[Destination IP]\t[Next Hop]\t[Metric]");
                foreach (RoutingState state in States)
                {
                    Console.WriteLine("{0}\t{1}\t\t{2}\t\t{3}",
                        ParentNode.GetIP(),
                        state.DestinationNode.GetIP(),
                        state.NextNode.GetIP(),
                        (state.Metric >= MAX_HOPS + 1) ? "inf" : state.Metric.ToString()
                        );
                }
                Console.WriteLine(""); //Formatting
            }            
        }
    }
}
