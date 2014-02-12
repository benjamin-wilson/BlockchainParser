using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser
{
    public class Transaction
    {
        public Transaction()
        {
            inputs = new List<Input>();
            outputs = new List<Output>();
        }
        public uint transactionVersionNumber;
        public ulong VL_inputCount;
        public ulong VL_outputCount;
        public uint transactionLockTime;
        public List<Input> inputs;
        public List<Output> outputs;
    }
}
