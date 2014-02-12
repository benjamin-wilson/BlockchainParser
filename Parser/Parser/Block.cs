using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public class Block
    {
        public Block()
        {
            transactions = new List<Transaction>();
        }
        public uint versionNumber;
        public byte[] previousBlockHash;
        public byte[] merkleRootHash;
        public uint timeStamp;
        public uint targetDifficulty;
        public uint nonce;
        public ulong VL_transactionCount;
        public List<Transaction> transactions;
    }
}
