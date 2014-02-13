using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public class ScryptExtraction
    {
        public static int counter = 0;

        public static void responseScrypt(byte[] scrypt)
        {

        }
        public static string challengeScrypt(byte[] scrypt)
        {
            bool keyFound = false;
            byte[] key = null;
            switch(scrypt.Length)
            {
                case 67:
                    key = format1(scrypt);
                    keyFound = true;
                    break;
                case 66:
                    key = format2(scrypt);
                    keyFound = true;
                    break;
                case 25:
                    key = format3(scrypt);
                    keyFound = true;
                    break;
                case 5:
                    key = null;
                    break;
                case 35:
                    key = format4(scrypt);
                    break;
            }
            //if still no key, search for it
            if(!keyFound)
            {
                key = searchForKey(scrypt, ref keyFound);
            }

            //no key exists
            if(!keyFound)
            {
                counter++;
                return "";
            }
            //key found!
            else
            {

                if(key.Length == 65)
                {
                    string address = AddressFormatter.EllipticCurveToBTCAddress(key);
                }
                else if(key.Length == 20)
                {
                    string address = AddressFormatter.ripemdToBTCAddress(key);
                }
                else if (key.Length == 32)
                {
                    //not sure what to do here
                }
                return "";
            }
        }
        private static byte[] format1(byte[] scrypt)
        {
            byte[] key = new byte[65];
            Array.Copy(scrypt, 1, key, 0, 65);
            return key;
        }
        private static byte[] format2(byte[] scrypt)
        {
            byte[] key = new byte[65];
            Array.Copy(scrypt, key, 65);
            return key;
        }
        private static byte[] format3(byte[] scrypt)
        {
            byte[] key = new byte[20];
            Array.Copy(scrypt, 3, key, 0, 20);
            return key;
        }
        private static byte[] format4(byte[] scrypt)
        {
            byte[] key = new byte[32];
            Array.Copy(scrypt, 2, key, 0, 32);
            return key;
        }
        private static byte[] searchForKey(byte[] scrypt,ref bool keyFound)
        {
            uint cursor = 0;
            while (cursor + 23 < scrypt.Length)
            {
                if (scrypt[cursor] == 118)
                {
                    if (scrypt[cursor + 1] == 169)
                    {
                        if (scrypt[cursor + 2] == 20)
                        {
                            keyFound = true;

                            byte[] key = new byte[20];
                            Array.Copy(scrypt, cursor + 3, key, 0, 20);

                            return key;
                        }
                    }
                }
                cursor++;
            }
            return null;
        }
    }
}
