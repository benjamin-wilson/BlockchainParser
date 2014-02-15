using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.MySQLClasses
{
    public class MySQLInput
    {
        public byte[] transactionHash;
        public byte[] previousTransactionHash;
        public uint previousTransactionOutputIndex;
    }
}
