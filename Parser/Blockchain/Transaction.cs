using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blockchain
{
    public class Transaction
    {
        public Transaction()
        {
            inputs = new List<Input>();
            outputs = new List<Output>();
            thisTransactionHash = new byte[32];
        }
        public uint transactionVersionNumber;
        public ulong VL_inputCount;
        public ulong VL_outputCount;
        public uint transactionLockTime;
        public List<Input> inputs;
        public List<Output> outputs;

        private byte[] _thisTransactionHash;

        public byte[] thisTransactionHash
        {
            set { this._thisTransactionHash = Sha256(Sha256(value)); }
            get { return this._thisTransactionHash; }
        }
        private static byte[] Sha256(byte[] array)
        {
            SHA256Managed hashstring = new SHA256Managed();
            return hashstring.ComputeHash(array);
        }
    }
}
