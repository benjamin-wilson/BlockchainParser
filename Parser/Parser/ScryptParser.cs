using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blockchain;

namespace Parser
{
    public static class ScryptParser
    {
        public static int unparsibleOuptuAddresses = 0;
        public static int invalidOutputAddresses = 0;
        public static ulong outputs = 0;
        public static string getPublicKey(byte[] scrypt)
        {
            outputs++;
            int length = scrypt.Length;
            if (length == 67)
            {
                return sixtySevenByte(scrypt);
            }
            else if (length == 66)
            {
                return sixtySixByte(scrypt);
            }
            else if (length == 25)
            {
                return twentyFiveByte(scrypt);
            }
            else if (length < 20)
            {
                invalidOutputAddresses++;
                return lessThanTwenty();
            }
            else
            {
                unparsibleOuptuAddresses++;
            }
            unparsibleOuptuAddresses++;
            return "";
        }
        private static string sixtySevenByte(byte[] scrypt)
        {
            byte[] key = new byte[65];
            Array.Copy(scrypt, 1, key, 0, 65);
            return AddressHelper.EllipticCurveToBTCAddress(key);
        }
        private static string sixtySixByte(byte[] scrypt)
        {
            byte[] key = new byte[65];
            Array.Copy(scrypt, key, 65);
            return AddressHelper.EllipticCurveToBTCAddress(key);
        }
        private static string twentyFiveByte(byte[] scrypt)
        {
            byte[] key = new byte[20];
            Array.Copy(scrypt, 3, key, 0, 20);
            return AddressHelper.ripemdToBTCAddress(key);
        }
        private static string lessThanTwenty()
        {
            return "Unspendable";
        }

    }
}
