using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.MySQLClasses
{
    public static class MySQLBlockHelper
    {
        public static void pushToMySQL(Block block)
        {
            List<MySQLInput> mysqlInputList = new List<MySQLInput>();
            List<MySQLOutput> mysqlOutputList = new List<MySQLOutput>();
            
            foreach(Transaction transaction in block.transactions)
            {
                foreach(Input input in transaction.inputs)
                {
                    MySQLInput mysqlInput = new MySQLInput();
                    mysqlInput.transactionHash = transaction.thisTransactionHash;
                    mysqlInput.previousTransactionHash = input.previousTransactionHash;
                    mysqlInput.previousTransactionOutputIndex = input.previousTransactionIndex;
                    mysqlInputList.Add(mysqlInput);
                }
                uint outputIndexCounter = 0;
                foreach(Output output in transaction.outputs)
                {
                    MySQLOutput mysqlOutput = new MySQLOutput();
                    mysqlOutput.value = output.value;
                    mysqlOutput.publicAddress = output.publicKeyAddress;
                    mysqlOutput.index = outputIndexCounter;
                    mysqlOutput.transactionHash = transaction.thisTransactionHash;
                    mysqlOutput.timestamp = block.timeStamp;
                    mysqlOutputList.Add(mysqlOutput);
                    outputIndexCounter++;
                }
            }
            DBConnect mysql = new DBConnect();
            mysql.InsertInputs(mysqlInputList);
            mysql.InsertOutputs(mysqlOutputList);

        }
    }
}
