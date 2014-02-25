using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database;
using Blockchain;

namespace API
{
    public static class Querrys
    {
        public static List<MySQLOutput> getRecivedFromOutputs(string address)
        {
            DBConnect mysql = new DBConnect();
            List<MySQLOutput> returnOutputs = new List<MySQLOutput>();

            List<MySQLOutput> outputs = mysql.SelectOutputsBasedOnAddress(address);
            foreach(MySQLOutput output in outputs)
            {
                List<MySQLInput> inputs = mysql.SelectInputsBasedOnTransactionHash(output.transactionHash);
                foreach(MySQLInput input in inputs)
                {
                    returnOutputs.AddRange(mysql.SelectOutputsBasedOnTransactionHashAndIndex(input.previousTransactionHash, input.previousTransactionOutputIndex));
                }
            }
            return returnOutputs;
        }
        public static List<MySQLOutput> getSentToOutputs(string address)
        {
            DBConnect mysql = new DBConnect();
            List<MySQLOutput> returnOutputs = new List<MySQLOutput>();

            List<MySQLOutput> outputs = mysql.SelectOutputsBasedOnAddress(address);
            foreach (MySQLOutput output in outputs)
            {
                List<MySQLInput> inputs = mysql.SelectInputsBasedOnPreviousTransactionHashAndPreviousIndex(output.transactionHash, output.index);
                foreach (MySQLInput input in inputs)
                {
                    returnOutputs.AddRange(mysql.SelectOutputsBasedOnTransactionHash(input.transactionHash));
                }
            }
            return returnOutputs;
        }
    }
}
