using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Database
{
    public class DBConnect
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        private string timeout;
        public DBConnect()
        {
            Initialize();
        }
        private void Initialize()
        {
            server = "localhost";
            database = "mydb";
            uid = "root";
            password = "tiny";
            timeout = "100";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";" + "Connect Timeout="+timeout;

            connection = new MySqlConnection(connectionString);
        }
        public void SetMaxAddressQuerryTime(int time)
        {
            string querry = "SET SESSION MAX_STATEMENT_TIME="+time.ToString()+";";
            MySqlCommand cmd = new MySqlCommand(querry, connection);
            cmd.ExecuteNonQuery();
        }

        public void UnlockTables()
        {
            string querry = "UNLOCK TABLES;";
            MySqlCommand cmd = new MySqlCommand(querry, connection);
            cmd.ExecuteNonQuery();
        }
        public bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }
        public bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public void InsertOutputs(List<SimplifiedOutput> outputs)
        {
            StringBuilder query = new StringBuilder();
            query.Append("LOCK TABLES output WRITE; INSERT INTO output (value, publicAddress, outputIndex, transactionHash, timestamp) VALUES");
            bool isFirstElement = true;
            foreach(SimplifiedOutput output in outputs)
            {
                if(!isFirstElement)
                {
                    query.Append(",");
                }
                query.Append("(");
                query.Append(output.value.ToString());
                query.Append(",'");
                query.Append(output.publicAddress.ToString());
                query.Append("',");
                query.Append(output.index.ToString());
                query.Append(",'");
                query.Append(output.transactionHash);
                query.Append("',");
                query.Append(output.timestamp.ToString());
                query.Append(")");
                isFirstElement = false;
            }
            query.Append(";");


            //create command and assign the query and connection from the constructor
            MySqlCommand cmd = new MySqlCommand(query.ToString(), connection);

            //Execute command
            cmd.ExecuteNonQuery();

            //close connection
            //this.CloseConnection();

        }
        public void InsertInputs(List<Simplifiednput> inputs)
        {
            StringBuilder query = new StringBuilder();
            query.Append("LOCK TABLES input WRITE; INSERT INTO input (transactionHash, previousTransactionHash, previousTransactionOutputIndex) VALUES");
            bool isFirstElement = true;
            foreach (Simplifiednput input in inputs)
            {
                if (!isFirstElement)
                {
                    query.Append(",");

                }
                query.Append("('");
                query.Append(input.transactionHash);
                query.Append("','");
                query.Append(input.previousTransactionHash);
                query.Append("',");
                query.Append(input.previousTransactionOutputIndex.ToString());
                query.Append(")");
                isFirstElement = false;
            }
            query.Append(";");
            //create command and assign the query and connection from the constructor
            MySqlCommand cmd = new MySqlCommand(query.ToString(), connection);

            //Execute command
            cmd.ExecuteNonQuery();

            //close connection
            //this.CloseConnection();
            
        }
        public List<Transaction> getRecivedFrom(string address)
        {
            List<Transaction> jsonList = new List<Transaction>();
            StringBuilder query = new StringBuilder();
            query.Append("Select output.publicAddress as source, SUM(totals.value) as value, COUNT(output.publicAddress) AS weight ");
            query.Append("from output,( SELECT previousTransactionHash, value, previousTransactionOutputIndex from input ");
                query.Append("left outer join output on input.transactionHash = output.transactionHash ");
                query.Append("where publicAddress = '");
                query.Append(address);
                query.Append("') AS totals ");
                query.Append("where totals.previousTransactionHash = output.transactionHash and totals.previousTransactionOutputIndex = output.outputIndex group by output.publicAddress;");

                //Create Command
                MySqlCommand cmd = new MySqlCommand(query.ToString(), connection);
                cmd.CommandTimeout = 120;
                //Create a data reader and Execute the command
                try
                {
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    //Read the data and store them in the list
                    while (dataReader.Read())
                    {
                        Transaction transaction = new Transaction();
                        transaction.source = (string)dataReader["source"];
                        transaction.target = address;
                        transaction.value = Convert.ToUInt64(dataReader["value"]);
                        transaction.weight = Convert.ToUInt16(dataReader["weight"]);
                        jsonList.Add(transaction);
                    }

                    //close Data Reader
                    dataReader.Close();

                    //close Connection
                    //this.CloseConnection();

                    //return list to be displayed
                    return jsonList;
                }
                catch(MySqlException e)
                {
                    Console.WriteLine("Skipping");
                    return null;
                }
        }
        public List<Transaction> getSentTo(string address)
        {
            List<Transaction> jsonList = new List<Transaction>();
            StringBuilder query = new StringBuilder();

            query.Append("select output.publicAddress as target, SUM(totals.value) as value, COUNT(output.publicAddress) AS weight from output,");
            query.Append("(SELECT input.transactionHash,output.value ");
            query.Append("from input ");
            query.Append("left outer join output on input.previousTransactionHash = output.transactionHash and input.previousTransactionOutputIndex = output.outputIndex ");
            query.Append("where publicAddress = '");
            query.Append(address);
            query.Append("') AS totals ");
            query.Append("where totals.transactionHash = output.transactionHash group by output.publicAddress;");

            //Create Command
            MySqlCommand cmd = new MySqlCommand(query.ToString(), connection);
            cmd.CommandTimeout = 120;
            //Create a data reader and Execute the command
            try
            {
                MySqlDataReader dataReader = cmd.ExecuteReader();
                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    Transaction transaction = new Transaction();
                    transaction.source = address;
                    transaction.target = (string)dataReader["target"];
                    transaction.value = Convert.ToUInt64(dataReader["value"]);
                    transaction.weight = Convert.ToUInt16(dataReader["weight"]);
                    jsonList.Add(transaction);
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                //this.CloseConnection();

                //return list to be displayed
                return jsonList;
            }
            catch (MySqlException e)
            {
                Console.WriteLine("Skipping");
                return null;
            }

        }
    }
}
