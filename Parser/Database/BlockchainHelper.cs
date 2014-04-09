using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blockchain;

namespace Database
{
    public static class BlockchainHelper
    {
        public static void pushToMySQL(Block block, DBConnect mysql)
        {
            List<Simplifiednput> mysqlInputList = new List<Simplifiednput>();
            List<SimplifiedOutput> mysqlOutputList = new List<SimplifiedOutput>();
            
            foreach(BlockchainTransaction transaction in block.transactions)
            {
                foreach(Input input in transaction.inputs)
                {
                    Simplifiednput mysqlInput = new Simplifiednput();
                    mysqlInput.transactionHash = truncateTransactionHashSixteen(BitConverter.ToString(transaction.thisTransactionHash).Replace("-", string.Empty));
                    mysqlInput.previousTransactionHash = truncateTransactionHashSixteen(BitConverter.ToString(input.previousTransactionHash).Replace("-", string.Empty));
                    mysqlInput.previousTransactionOutputIndex = input.previousTransactionIndex;
                    mysqlInputList.Add(mysqlInput);
                }
                uint outputIndexCounter = 0;
                foreach(Output output in transaction.outputs)
                {
                    SimplifiedOutput mysqlOutput = new SimplifiedOutput();
                    mysqlOutput.value = output.value;
                    mysqlOutput.publicAddress = output.publicKeyAddress;
                    mysqlOutput.index = outputIndexCounter;
                    mysqlOutput.transactionHash = truncateTransactionHashSixteen(BitConverter.ToString(transaction.thisTransactionHash).Replace("-", string.Empty));
                    mysqlOutput.timestamp = block.timeStamp;
                    mysqlOutputList.Add(mysqlOutput);
                    outputIndexCounter++;
                }
            }
            
            mysql.InsertInputs(mysqlInputList);
            mysql.InsertOutputs(mysqlOutputList);

        }
        private static string truncateTransactionHashSixteen(string hash)
        {
            return hash.Substring(0, 16);
        }
    }
}
