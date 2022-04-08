using System;
using System.IO;
using System.Threading.Tasks;
using GBN.Types;

namespace GBN.IO {
    public static class WriterExtensions {
        public async static Task WriteAddressAsync(this Stream writer, Address address) 
        {
            var data = new byte[address.address.Length];
            Array.Copy(address.address, data, address.address.Length);
            Array.Reverse(data);
            await writer.WriteAsync(data, 0, 6);
        }

        public async static Task WriteFrameAsync(this Stream writer, Frame frame) 
        {
            await writer.WriteAddressAsync(frame.dst_addr);
            await writer.WriteAddressAsync(frame.src_addr);

            if (frame.data.Length > 1500 || frame.data.Length < 46)
                throw new InvalidDataException ("Invalid frame data length");

            await writer.WriteAsync(BitConverter.GetBytes ((ushort) frame.data.Length));
            await writer.WriteAsync(frame.data);

            var hash = new Utils.CRC32();
            await writer.WriteAsync(
                BitConverter.GetBytes(hash.ComputeHash(frame.data, frame.data.Length))
            );
        }
    }
}
