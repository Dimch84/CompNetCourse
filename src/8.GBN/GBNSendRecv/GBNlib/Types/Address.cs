using System;
using System.Linq;

namespace GBN.Types {
    public class Address {
        public byte[] address = new byte[6];

        public override string ToString () 
        {
            return String.Join (':', address.Select((b) => b.ToString("X2")));
        }

        public Address() { }

        public Address(string str_rep) 
        {
            address = str_rep.Split (':').Select (
                (c) => Utils.HexConvert.GetByte (c)
            ).ToArray ();
            if (address.Length != 6) throw new Exception ("invalid MAC");
        }
    }
}