using System;
using System.IO;

namespace GBN.Utils {
    public class CRC32 {
        private readonly uint[] ChecksumTable;
        private readonly uint Polynomial = 0xEDB88320;

        public CRC32 () {
            ChecksumTable = new uint[0x100];

            for (uint index = 0; index < 0x100; ++index) {
                uint item = index;
                for (int bit = 0; bit < 8; ++bit)
                    item = ((item & 1) != 0) ? (Polynomial ^ (item >> 1)) : (item >> 1);
                ChecksumTable[index] = item;
            }
        }

        public uint ComputeHash (Stream stream) {
            uint result = 0xFFFFFFFF;

            int current;
            while ((current = stream.ReadByte ()) != -1)
                result = ChecksumTable[(result & 0xFF) ^ (byte) current] ^ (result >> 8);

            byte[] hash = BitConverter.GetBytes (~result);
            Array.Reverse (hash);
            return BitConverter.ToUInt32(hash);
        }

        public uint ComputeHash (byte[] data, int length) {
            using (MemoryStream stream = new MemoryStream (data, 0, length))
                return ComputeHash (stream);
        }
    }
}