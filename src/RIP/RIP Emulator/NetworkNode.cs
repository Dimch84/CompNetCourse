using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIP_Emulator
{
    class NetworkNode
    {
        private object ThreadLock = new object(); // for locking the thread

        private string IPAddress = "0.0.0.0";
        private List<Edge> Edges = new List<Edge>();
        private volatile RoutingTable Table;
        private Network CurrentNetwork;

        private int SimulationCycles = 0;

        public NetworkNode() { }

        public NetworkNode(string IPAddress, Network CurrentNetwork)
        {
            this.IPAddress = IPAddress;
            this.CurrentNetwork = CurrentNetwork;
        }

        public void AddEdge(Edge edge)
        {
            Edges.Add(edge);
        }

        public void RemoveEdge(Edge edge)
        {
            Edges.Remove(edge);
        }

        public List<Edge> GetEdges()
        {
            return Edges;
        }

        public string GetIP()
        {
            return IPAddress;
        }

        public void InitTable()
        {
            Table = new RoutingTable(this, CurrentNetwork);
        }

        public void SetSimulationCycles(int cycles)
        {
            SimulationCycles = cycles;
        }

        public void Simulate()
        {         
            for(int i = 0; i < SimulationCycles; i++)
            {
                Table.PrintTable(i + 1);
                Broadcast();
                System.Threading.Thread.Sleep(1000); // Give one second wait for updates from other routers.               
            }           
        }

        public void PrintTable()
        {
            Table.PrintTable();
        }
       
        private void Broadcast()
        {
            foreach(Edge e in Edges)
            {
                NetworkNode n = e.GetLeft() == this ? e.GetRight() : e.GetLeft(); // Get neighboring node and not ourselves
                n.Receive(this, Table);
            }
        }

        public void Receive(NetworkNode node, RoutingTable receivedTable)
        {
            lock(ThreadLock)
            {
                foreach (RoutingState n_state in receivedTable.GetRoutingStates())
                {
                    if (n_state.NextNode.GetIP() == "0.0.0.0") // Other router does not know the way to the target (yet?)
                        continue;

                    List<RoutingState> cstates = Table.GetRoutingStates();
                    for (int i = 0; i < cstates.Count; i++)
                    {
                        RoutingState curState = cstates[i];
                        if (curState.DestinationNode == n_state.DestinationNode)
                        {
                            int targetMetric = n_state.Metric + 1;
                            if (targetMetric < curState.Metric)
                            {
                                curState.Metric = targetMetric;
                                curState.NextNode = node;
                                cstates[i] = curState;
                            }
                        }
                    }
                }
            }          
        }
    }
}
