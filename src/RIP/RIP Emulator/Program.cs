using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RIP_Emulator
{
    class Program
    {
        const int DEFAULT_ROUTERAMOUNT = 5;
        const int DEFAULT_SIMULATIONCYCLES = 4;
      
        static void PrintHelp()
        {
            Console.WriteLine("\nHelp menu:");
            Console.WriteLine("[Command]\t\t\t[Description]\n");
            Console.WriteLine("'{0}'\t\t\t\t{1}", "help", "shows this menu");
            Console.WriteLine("'{0}'\t\t\t\t{1}", "exit", "exits the program");
            Console.WriteLine("'{0}'\t\t\t\t{1}", "print", "prints out the network");
            Console.WriteLine("'{0}'\t\t{1}", "print_table <ip>", "prints out the table of the given router");
            Console.WriteLine("'{0}'\t\t{1}", "generate <amount>", "generates a given amount of routers with random connections");
            Console.WriteLine("'{0}'\t\t{1}", "simulate <cycles>", "simulates the network with given amount of cycles");
            Console.WriteLine("'{0}'\t\t{1}", "add_router <ip>", "adds a router to network");
            Console.WriteLine("'{0}'\t\t{1}", "remove_router <ip>", "removes router from network");
            Console.WriteLine("'{0}'\t\t{1}", "add_link <ip1> <ip2>", "adds a link between routers");
            Console.WriteLine("'{0}'\t{1}", "remove_link <ip1> <ip2>", "removes a link between routers");

            Console.WriteLine("\n");
        }

        static string GetStringArgument(string text, int argument)
        {
            string[] args = text.Split(' ');
            if(args.Length - 1 < argument)
            {
                Console.WriteLine("Invalid amount of arguments!");
                return null;
            }
            return args[argument+1];
        }

        static int GetIntegerArgument(string text, int argument)
        {
            string[] args = text.Split(' ');
            if (args.Length - 2 < argument)
            {
                Console.WriteLine("Invalid amount of arguments!");
                return -1;
            }

            int ret = -1;
            bool parsed = int.TryParse(args[argument + 1], out ret);
            if(!parsed)
            {
                Console.WriteLine("Invalid argument type!");
                return -1;
            }
            return ret;
        }

        static bool CheckIP(string ip)
        {
            IPAddress chk;
            return IPAddress.TryParse(ip, out chk);
        }

        static void Cli()
        {
            Network currentNetwork = new Network();

            Console.WriteLine("Welcome to manual control!");
            Console.WriteLine("Type 'help' to see the available commands");

            Console.Write("Input: ");
            string command = "";
            string fullcommand = "";
            bool exit = false;

            while(!exit)
            {
                fullcommand = Console.ReadLine().ToLower();
                command = fullcommand.Trim().Split(' ')[0];
                switch (command)
                {
                    case "help":
                        PrintHelp();
                        break;
                    case "exit":
                        exit = true;
                        break;
                    case "generate":
                        int arg = GetIntegerArgument(fullcommand, 0);
                        if (arg == -1) break;
                        currentNetwork.Generate(arg);
                        break;
                    case "simulate":
                        int arg9 = GetIntegerArgument(fullcommand, 0);
                        if (arg9 == -1) break;
                        currentNetwork.Simulate(arg9);
                        break;
                    case "print":
                        currentNetwork.PrintNodes();
                        break;
                    case "add_router":
                        string arg2 = GetStringArgument(fullcommand, 0);
                        if (arg2 == null) break;
                        if (!CheckIP(arg2)) break;
                        currentNetwork.AddRouter(arg2);
                        break;
                    case "remove_router":
                        string arg3 = GetStringArgument(fullcommand, 0);
                        if (arg3 == null) break;
                        if (!CheckIP(arg3)) break;
                        currentNetwork.RemoveRouter(arg3);
                        break;
                    case "print_table":
                        string arg4 = GetStringArgument(fullcommand, 0);
                        if (arg4 == null) break;
                        if (!CheckIP(arg4)) break;
                        currentNetwork.PrintTable(arg4);
                        break;
                    case "add_link":
                        string arg5 = GetStringArgument(fullcommand, 0);
                        string arg6 = GetStringArgument(fullcommand, 1);
                        if (arg5 == null || arg6 == null) break;
                        if (!CheckIP(arg5) || !CheckIP(arg6)) break;
                        currentNetwork.AddLink(arg5,arg6);
                        break;
                    case "remove_link":
                        string arg7 = GetStringArgument(fullcommand, 0);
                        string arg8 = GetStringArgument(fullcommand, 1);
                        if (arg7 == null || arg8 == null) break;
                        if (!CheckIP(arg7) || !CheckIP(arg8)) break;
                        currentNetwork.AddLink(arg7, arg8);
                        break;
                    default:
                        Console.WriteLine("Unrecognized command: " + command);
                        break;

                }

                Console.Write("Input: ");
            }
        }

        static void RunSimulation(int routerAmount, int simulationCycles)
        {
            Network net = new Network();
            Console.WriteLine("Generating a network with {0} nodes.", routerAmount);
            net.Generate(routerAmount);

            Console.WriteLine("\nSimulating the generated network...");
            net.Simulate(simulationCycles);
        }

        static void Main(string[] args)
        {
            Console.Title = "RIP Emulator";
            if(args.Length == 0)
            {
                Console.WriteLine("Running default generated simulation!");
                Console.WriteLine("Use 'manual' argument to be able to control it manually!\n");
                RunSimulation(DEFAULT_ROUTERAMOUNT, DEFAULT_SIMULATIONCYCLES);
                Console.WriteLine("End of execution!");
                Console.WriteLine("Press any key to exit!");
                Console.ReadLine();
                return;
            }
            else
            {
                if (args[0].ToLower().Trim() == "manual")
                    Cli();
            }                    
        }
    }
}
