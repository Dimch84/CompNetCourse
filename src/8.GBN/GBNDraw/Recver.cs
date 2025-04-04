using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace goBackN
{
    public class Recver
    {
        public List<Frame> Frames { get; private set; }
        public ConcurrentQueue<Frame> send_que { get; private set; }
        public ConcurrentQueue<Frame> rcv_que { get; private set; }
        public int rcv_n { get; set; }
        public Sender Sender { get; set; }

        public Recver(List<Frame> frames, ConcurrentQueue<Frame> sque, ConcurrentQueue<Frame> rque,Sender sender)
        {
            this.Frames = frames;
            this.send_que = sque;
            this.rcv_que = rque;
            this.Sender = sender;
            rcv_n = 0;
        }

        public void SendACK(int i)
        {
            if (!Frame_Will_Lose())
            {
                rcv_que.Enqueue(Frames[i]);
            }
            else
            {
                Console.WriteLine("ACK{0} потерян", i);
            }
        }

        public void Recv()
        {
            if (send_que.TryDequeue(out Frame frame))
            {
                Sender.Draw(Sender.start_n, Sender.curr_n, rcv_n, Sender.window_size, 16);

                Console.WriteLine("Recver:полученный кадр{0}", frame.Id);
                if (frame.Id == rcv_n)
                {
                    Console.WriteLine("То же, что и Rn, отправить ACK{0}", rcv_n + 1);
                    rcv_n++;
                    if (rcv_n <= 16)
                        SendACK(rcv_n);
                }
                else
                {
                    Console.WriteLine("Отличается от Rn, отправить ACK{0}", rcv_n);

                    SendACK(rcv_n);
                }
            }
        }

        public void Run()
        {
            while (true)
            {
                Recv();
                Thread.Sleep(500);
            }
        }

        public bool Frame_Will_Lose()
        {
            Random rand = new Random();
            return rand.Next(1, 10) > 8;
        }
    }
}
