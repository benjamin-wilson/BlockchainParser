using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser
{
    public class Input
    {
        public byte[] transactionHash;
        public uint transactionIndex;
        public ulong VL_scriptLength;
        public byte[] VL_inputScript;
        public uint sequenceNumber;
    }
}
