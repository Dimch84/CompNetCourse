using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace goBackN
{
    class Program
    {
        static List<Frame> frames;
        static ConcurrentQueue<Frame> sque = new ConcurrentQueue<Frame>();
        static ConcurrentQueue<Frame> rque = new ConcurrentQueue<Frame>();

        static void Main()
        {
            Console.WriteLine("Количество пакетов = 16，размер окна = 4\n");
            
            frames = new List<Frame>();
            for (int i = 0; i <= 16; i++)
            {
                frames.Add(new Frame(i));
            }

            Sender sender = new Sender(frames, sque, rque);
            Recver recver = new Recver(frames, sque, rque, sender);
            new Thread(() => sender.Run()).Start();
            new Thread(() => recver.Run()).Start();
        }
    }
}
