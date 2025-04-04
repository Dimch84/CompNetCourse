using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace goBackN
{
    public partial class Sender
    {
        public List<Frame> Frames { get; set; }
        public ConcurrentQueue<Frame> send_que { get; set; }
        public ConcurrentQueue<Frame> rcv_que { get; set; }
        
        public int start_n, curr_n, window_size;

        public Sender(List<Frame> frames, ConcurrentQueue<Frame> sque, ConcurrentQueue<Frame> rque)
        {
            start_n = curr_n = 0;
            window_size = 4;
            this.Frames = frames;
            this.send_que = sque;
            this.rcv_que = rque;
        }

        public void Send()
        {
            if (curr_n < start_n + window_size && curr_n < 16)
            {
                Console.WriteLine("Sender: отправить пакет {0}", curr_n);
                Frames[curr_n].Timer = new Timer(new TimerCallback(Timeout), Frames[curr_n], 3000, 0);
                if (!Frame_Will_Lose())
                {
                    Console.WriteLine("Успешно отправлено", curr_n);
                    send_que.Enqueue(Frames[curr_n]);
                }
                else
                {
                    Console.WriteLine("Пакет {0} потерян", curr_n);
                }
                curr_n++;
            }
            Thread.Sleep(500);
        }

        public void Recv()
        {
            if (rcv_que.TryDequeue(out Frame ack))
            {
                if (ack.Id <= curr_n && ack.Id > start_n)
                {
                    Console.WriteLine("ACK получен для пакета {0}", ack.Id - 1);
                    Draw(start_n, curr_n, ack.Id , window_size, 16);
                    for (int i = start_n; i < ack.Id; i++)
                    {
                        Frames[i].Timer.Dispose();
                    }
                    start_n = ack.Id;
                    if (ack.Id == 16)
                    {
                        Console.WriteLine("ACK 16 получен, передача завершена");
                        Draw(16, curr_n, ack.Id, window_size, 16);
                        Console.WriteLine("Нажмите любую клавишу для выхода...");
                        Console.Read();
                        System.Environment.Exit(0);
                    }
                }
            }
            Thread.Sleep(500);
        }

        public void Timeout(Object state)
        {
            Frame frame = (Frame)state;
            Console.WriteLine($"Таймер пакета {frame.Id} истек, повторите передачу");
            for (int i = frame.Id; i < curr_n; i++)
            {
                Frames[i].Timer.Dispose();
            }
            curr_n = frame.Id;
        }

        public void Run()
        {
            while (true)
            {
                Recv();
                Send();
            }
        }

        public bool Frame_Will_Lose()
        {
            Random rand = new Random();
            return rand.Next(1, 10) > 8;
        }
    }
}
