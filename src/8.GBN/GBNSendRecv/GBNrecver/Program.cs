using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using GBN.IO;
using GBN.Types;

namespace GBNrecver {
    class Program {
        static async Task Main (string[] args) {
            var virtual_mac_addr = new Address("ac:de:48:00:11:22");

            byte[] buffer = new byte[4];

            TcpListener listener = new TcpListener (IPAddress.Loopback, 1234);
            listener.Start ();

            while (true) 
            {
                Console.WriteLine ("Listener started to receive messages");
                var client = await listener.AcceptTcpClientAsync ();

                Console.WriteLine ("Listener accepted\n");
                using (var stream = client.GetStream()) 
                {
                    int current_ack = -1;
                    while (true) 
                    {
                        if (!stream.CanRead) 
                            break;
                        try 
                        {
                            if (await stream.ReadAsync(buffer, 0, 4) == 0) 
                                break;

                            int id = BitConverter.ToInt32(buffer);
                            Console.WriteLine("packet id: " + id);

                            var frm = await stream.ReadFrameAsync(virtual_mac_addr);
                            Console.WriteLine(frm);

                            if (id == current_ack + 1)
                                current_ack++;
                        } catch (InvalidDataException e) 
                        {
                            Console.WriteLine (e);
                        }

                        try {
                            await stream.WriteAsync(BitConverter.GetBytes(current_ack));
                        } catch 
                        { 
                            break; 
                        }
                    }
                }
            }
        }
    }
}
