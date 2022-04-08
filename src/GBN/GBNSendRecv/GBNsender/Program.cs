using System;
using System.Threading.Tasks;

namespace GBNsender {
    class Program {
        static async Task Main (string[] args) 
        {
            string[] message_pool = { "This is a test text message to send via GBN protocol", "Another test message to check GBN protocol", "Some text here to check packets" };
            foreach(var message in message_pool)
            {
                var ctl = new Control("14:7d:da:6a:44:78", "ac:de:48:00:11:22");
                await ctl.Send(message);
                Console.WriteLine($"Message sent:'{message}'");
            }

            Console.ReadLine();
        }
    }
}
