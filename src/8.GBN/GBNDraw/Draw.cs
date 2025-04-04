using System;
using System.Threading;

namespace goBackN
{
    public partial class Sender
    {
        static Mutex mutex = new Mutex(false, "goBackN");
        public void Draw(int sf, int sn, int rn, int sw, int dataLength)
        {
            mutex.WaitOne();
            Console.Write("Sender: ");
            for (int i = 0; i < sf; i++) sended(i);
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Write("[ ");
            for (int i = sf; i < sn; i++) waitForResponse(i);
            for (int i = sn; i < Math.Min(sf + sw, dataLength); i++) dataInWindow(i);
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Write(" ]");
            for (int i = sf + sw; i < dataLength; i++) unsend(i);
            Console.ResetColor();

            // Receiver
            Console.Write("{0,20}      ", "Recver");
            for (int i = 0; i < rn; i++) sended(i);
            if (rn < 16) waitForResponse(rn);
            for (int i = rn + 1; i < dataLength; i++) unsend(i);

            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            mutex.ReleaseMutex();
        }

        static void unsend(int i)
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.Write(i.ToString() + ' ');
        }

        static void sended(int i)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(i.ToString() + ' ');
        }

        static void waitForResponse(int i)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Write(i.ToString() + ' ');
        }

        static void dataInWindow(int i)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.Write(i.ToString() + ' ');
        }
    }
}
