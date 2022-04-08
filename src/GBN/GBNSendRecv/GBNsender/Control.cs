using System.Collections.Generic;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GBN.IO;
using GBN.Types;

namespace GBNsender {
    class BrokenFrameException : Exception 
    {
        public int broken_frame { get; private set; }
        public BrokenFrameException (int b) { broken_frame = b; }
    }

    class Control 
    {
        int current_id, window_size;
        List<(int, Frame, Task)> window = new List<(int, Frame, Task)>();
        string src, dst;
        TcpClient client;
        Queue<(int, CancellationTokenSource, CancellationTokenSource)> tokenSources
            = new Queue<(int, CancellationTokenSource, CancellationTokenSource)>();

        public Control (string src, string dst, string ip = "127.0.0.1", int port = 1234, int window_size = 4) {
            this.src = src;
            this.dst = dst;
            this.client = new TcpClient(ip, port);
            this.window_size = window_size;
        }

        async Task SendFrame(Stream stream, int id, Frame frm)
        {
            byte[] ack_buf = new byte[4];
            await stream.WriteAsync(BitConverter.GetBytes(id), 0, 4);
            await stream.WriteFrameAsync(frm);

            var brokenTokenSource = new CancellationTokenSource();
            var ackedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                brokenTokenSource.Token
            );
            tokenSources.Enqueue((id, ackedTokenSource, brokenTokenSource));
            var token = ackedTokenSource.Token;

            if (!stream.ReadAsync(ack_buf, 0, 4).Wait (2000, token)) 
            {
                if (!token.IsCancellationRequested || brokenTokenSource.IsCancellationRequested)
                    throw new BrokenFrameException (id);
                // successfully delivered if acked cancellation requested
                return;
            }

            int ack_id = BitConverter.ToInt32(ack_buf);

            if (window.Count > 0 && (ack_id < window[0].Item1 || ack_id > window[0].Item1 + window_size))
                return;  // broken ack, ignored

            // no need to wait for prior acks
            while (tokenSources.Count > 0 && tokenSources.Peek().Item1 < ack_id)
                tokenSources.Dequeue().Item2.Cancel();

            if (tokenSources.Count > 0 && ack_id < id) // frame not properly received
                tokenSources.Dequeue().Item3.Cancel();
        }

        async Task WaitFirstTask(Stream stream)
        {
            try {
                await window[0].Item3;
                window.RemoveAt(0);
            } 
            catch (BrokenFrameException e)
            {
                while (window[0].Item1 < e.broken_frame)
                    window.RemoveAt(0);
                int current_count = window.Count;
                for (int i = 0; i < current_count; i++)
                    window.Add(
                        (window[i].Item1, window[i].Item2, SendFrame(stream, window[i].Item1, window[i].Item2))
                    );
                window.RemoveRange (0, current_count);
            }
        }

        public async Task Send (string data) 
        {
            var builder = new FrameBuilder(src, dst);
            using (var stream = client.GetStream())
            {
                foreach (var frm in builder.GetFrames(Encoding.UTF8.GetBytes(data))) 
                {
                    if (window.Count > 0 && current_id > window[0].Item1 + window_size) 
                    {
                        await WaitFirstTask(stream);
                    }
                    window.Add((current_id, frm, SendFrame(stream, current_id++, frm)));
                }

                while (window.Count > 0) 
                    await WaitFirstTask(stream);
            }
        }
    }
}