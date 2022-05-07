using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace RIP_Emulator
{
    class Network
    {
        private Random RandomGenerator = new Random();
        private List<NetworkNode> NetworkNodes = new List<NetworkNode>();

        public Network() { }

        public void Generate(int Networks)
        {
            if(Networks <= 1)
                throw new ArgumentException("Cannot create a network with less than two networks!");

            NetworkNodes.Clear();

            //Generate network point nodes
            List<string> uniqueIPs = GenerateUniqueIPs(Networks);

            Console.WriteLine("Generated IP Addresses: ");
            foreach(string ip in uniqueIPs)
            {
                Console.WriteLine(ip);
                NetworkNodes.Add(new NetworkNode(ip, this));
            }

            Console.WriteLine("Generated connections: ");
            //Generate a randomly connected network graph
            for(int i = 0; i < Networks; i++)
            {
                //Generate a random network to target
                int r = RandomGenerator.Next(Networks);
                while(r == i)
                    r = RandomGenerator.Next(Networks);

                NetworkNode currentNetwork = NetworkNodes[i];
                NetworkNode targetNetwork = NetworkNodes[r];
                                  
                Edge e = new Edge(currentNetwork, targetNetwork);
                currentNetwork.AddEdge(e);
                targetNetwork.AddEdge(e);

                Console.WriteLine("{0} -- {1}", currentNetwork.GetIP(), targetNetwork.GetIP());
            }

        }

        public void AddLink(string ip1, string ip2)
        {
            NetworkNode selected1 = FindNode(ip1);
            NetworkNode selected2 = FindNode(ip2);
            if (selected1 == null || selected2 == null)
            {
                Console.WriteLine("One or both routers are not found!");
                return;
            }
            Edge e = new Edge(selected1, selected2);
            selected1.AddEdge(e);
            selected2.AddEdge(e);
            Console.WriteLine("Link added!");
        }

        public void RemoveLink(string ip1, string ip2)
        {
            NetworkNode selected1 = FindNode(ip1);
            NetworkNode selected2 = FindNode(ip2);
            if (selected1 == null || selected2 == null)
            {
                Console.WriteLine("One or both routers are not found!");
                return;
            }

            Edge sEdge = null;
            foreach(Edge e in selected1.GetEdges())
            {
                if (e.GetLeft() == selected2 || e.GetRight() == selected2)
                    sEdge = e;
            }

            if(sEdge == null)
            {
                Console.WriteLine("Link not found!");
                return;
            }

            selected1.RemoveEdge(sEdge);
            selected2.RemoveEdge(sEdge);
            Console.WriteLine("Link removed!");
        }

        public void AddRouter(string ip)
        {
            NetworkNode selected = FindNode(ip);

            if (selected != null)
            {
                Console.WriteLine("Router already exists!");
                return;
            }

            NetworkNodes.Add(new NetworkNode(ip, this));
            Console.WriteLine("Router {0} addded!", ip);
        }

        public void RemoveRouter(string ip)
        {
            NetworkNode selected = FindNode(ip);

            if (selected == null)
            {
                Console.WriteLine("Router not found!");
                return;
            }

            NetworkNodes.Remove(selected);
            foreach (NetworkNode ns in NetworkNodes)
            {
                List<Edge> edges = ns.GetEdges();
                List<Edge> toRemove = new List<Edge>();
                foreach (Edge ee in edges)
                {
                    if (ee.GetLeft() == selected || ee.GetRight() == selected)
                        toRemove.Add(ee);
                }

                foreach (Edge ee in toRemove)
                {
                    edges.Remove(ee);
                }
            }
            Console.WriteLine("Router {0} removed!", ip);
        }


        public void PrintNodes()
        {
            foreach(NetworkNode n in NetworkNodes)
            {
                Console.WriteLine("Router: {0}", n.GetIP());
                Console.WriteLine("Connections:");
                foreach(Edge e in n.GetEdges())
                {
                    NetworkNode en = e.GetLeft() == n ? e.GetRight() : e.GetLeft();
                    Console.WriteLine("{0} -- {1}", n.GetIP(), en.GetIP());
                }
                Console.WriteLine();
            }
        }

        public void Simulate(int simulationCycles)
        {
            List<Thread> threads = new List<Thread>();

            foreach (NetworkNode n in NetworkNodes)
                n.InitTable();

            foreach (NetworkNode n in NetworkNodes)
            {
                n.SetSimulationCycles(simulationCycles);

                Thread th = new Thread(n.Simulate);
                threads.Add(th);
                th.Start();
            }

            //Wait for all threads to finish...
            foreach (Thread th in threads)
                th.Join();

            foreach (NetworkNode n in NetworkNodes)
            {
                Console.WriteLine("Final state of router {0} table:", n.GetIP());
                n.PrintTable();
            }            
        }

        public int GetNodeCount()
        {
            return NetworkNodes.Count;
        }

        public List<NetworkNode> GetNetworkNodes()
        {
            return NetworkNodes;
        }

        private List<string> GenerateUniqueIPs(int amount)
        {
            List<string> uniqueIPS = new List<string>();

            for(int i = 0; i < amount; i++)
            {
                string ipAddr = GenerateIPAddress();
                while (uniqueIPS.Contains(ipAddr))
                    ipAddr = GenerateIPAddress();
                uniqueIPS.Add(ipAddr);
            }
            
            return uniqueIPS;
        }

        public void PrintTable(string ip)
        {
            NetworkNode selected = FindNode(ip);

            if (selected == null)
            {
                Console.WriteLine("Router not found!");
                return;
            }

            selected.PrintTable();
        }

        private NetworkNode FindNode(string ip)
        {
            NetworkNode selected = null;
            foreach (NetworkNode ns in NetworkNodes)
            {
                if (ns.GetIP() == ip)
                {
                    selected = ns;
                    break;
                }
            }
            return selected;
        }

        private string GenerateIPAddress()
        {
            return string.Format("{0}.{1}.{2}.{3}", RandomGenerator.Next(1,254), RandomGenerator.Next(1, 254), RandomGenerator.Next(1, 254), RandomGenerator.Next(1, 254));
        }

    }
}
