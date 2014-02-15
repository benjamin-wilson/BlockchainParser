using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser
{
    public class Input
    {
        public byte[] previousTransactionHash;
        public uint previousTransactionIndex;
        public ulong VL_scriptLength;
        public byte[] VL_inputScript;
        public uint sequenceNumber;
    }
}
