using System.Collections.Generic;
using GBN.Types;

namespace GBNsender {
    public class FrameBuilder 
    {
        Address src_addr, dst_addr;
    
        public FrameBuilder (string src_addr, string dst_addr) 
        {
            this.src_addr = new Address(src_addr);
            this.dst_addr = new Address(dst_addr);
        }

        public IEnumerable<Frame> GetFrames(byte[] data) 
        {
            int cursor = 0;
            while (cursor < data.Length) 
            {
                Frame f = new Frame ();
                f.dst_addr = this.dst_addr;
                f.src_addr = this.src_addr;

                int packet_length = data.Length - cursor;
                if (packet_length > 30) packet_length = 30;
                if (packet_length < 10) packet_length = 10;
                byte[] buffer = new byte[packet_length];
                int copy_length =
                    data.Length - cursor < 10 ?
                    data.Length - cursor : packet_length;
                System.Array.Copy(data, cursor, buffer, 0, copy_length);
                f.data = buffer;
                cursor += copy_length;
                
                yield return f;
            }
        }
    }
}
