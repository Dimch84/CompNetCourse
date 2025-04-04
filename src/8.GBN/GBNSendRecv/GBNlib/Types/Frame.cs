namespace GBN.Types {
    public class Frame {
        public Address dst_addr, src_addr;

        public byte[] data;
        
        public override string ToString () 
        {
            return $"<{src_addr}->{dst_addr}>\n{Utils.HexDump.Get(data)}";
        }
    }
}