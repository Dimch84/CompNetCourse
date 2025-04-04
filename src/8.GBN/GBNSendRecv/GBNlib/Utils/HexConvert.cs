namespace GBN.Utils {
    class HexConvert {
        public static int GetHexVal (char hex) {
            int val = (int) hex;
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
        public static byte GetByte (string hex) {
            if (hex.Length != 2)
                throw new System.Exception ("Converts a single hex byte");
            return (byte) ((GetHexVal (hex[0]) << 4) + (GetHexVal (hex[1])));
        }
        public static byte[] GetBytes (string hex) {
            if (hex.Length % 2 == 1)
                throw new System.Exception ("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i) {
                arr[i] = (byte) ((GetHexVal (hex[i << 1]) << 4) + (GetHexVal (hex[(i << 1) + 1])));
            }
            return arr;
        }
    }
}