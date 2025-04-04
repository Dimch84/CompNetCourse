using System;
using System.IO;
using System.Threading.Tasks;
using GBN.Types;

namespace GBN.IO {
    public static class ReaderExtensions {
        public async static Task<Address> ReadAddressAsync(this Stream reader) 
        {
            var ret = new Address();
            await reader.ReadAsync(ret.address, 0, 6);
            Array.Reverse(ret.address);
            // MACs are reversely transported
            return ret;
        }

        public async static Task<Frame> ReadFrameAsync(this Stream reader, Address self_addr = null) 
        {
            byte[] buffer = new byte[1504];
            Address dst_addr = await reader.ReadAddressAsync();
            if (self_addr != null && dst_addr.ToString() != self_addr.ToString())
                throw new InvalidDataException ("frame not bound for this machine");

            Frame f = new Frame();
            f.dst_addr = dst_addr;
            f.src_addr = await reader.ReadAddressAsync();

            await reader.ReadAsync(buffer, 0, 2);
            short len = BitConverter.ToInt16(buffer, 0);

            // read data and do nothing
            await reader.ReadAsync(buffer, 0, len);
            
            var hash = new Utils.CRC32();
            await reader.ReadAsync(buffer, len, 4);
            if (BitConverter.ToUInt32(buffer, len) != hash.ComputeHash(buffer, len)) {
                throw new InvalidDataException ("CRC mismatch");
            }

            // read data
            f.data = new byte[len];
            Array.Copy(buffer, f.data, len);

            return f;
        }
    }
}