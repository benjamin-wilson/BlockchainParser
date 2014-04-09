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
            password = "";
            timeout = "1";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";" + "Connect Timeout="+timeout;

            connection = new MySqlConnection(connectionString);
        }
        private bool OpenConnection()
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
        private bool CloseConnection()
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
            query.Append("INSERT INTO output (value, publicAddress, outputIndex, transactionHash, timestamp) VALUES");
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

            //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Windows v2\bitcoinDatabaseLogs.txt", true))
            //{
            //    file.WriteLine(query);
            //    file.WriteLine();
            //}

            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query.ToString(), connection);

                //Execute command
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
            }
        }
        public void InsertInputs(List<Simplifiednput> inputs)
        {
            StringBuilder query = new StringBuilder();
            query.Append("INSERT INTO input (transactionHash, previousTransactionHash, previousTransactionOutputIndex) VALUES");
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
            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query.ToString(), connection);

                //Execute command
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
            }
        }
        public List<Transaction> getRecivedFrom(string address)
        {
            List<Transaction> jsonList = new List<Transaction>();
            StringBuilder query = new StringBuilder();
            query.Append("Select output.publicAddress as source,totals.value,totals.publicAddress as target ");
                query.Append("from output,( SELECT previousTransactionHash, value, previousTransactionOutputIndex,publicAddress ");
                query.Append("from input ");
                query.Append("join output on input.transactionHash = output.transactionHash ");
                query.Append("where publicAddress = '");
                query.Append(address);
                query.Append("') AS totals ");
                query.Append("where totals.previousTransactionHash = output.transactionHash and totals.previousTransactionOutputIndex = output.outputIndex;");

                if (this.OpenConnection() == true)
                {
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
                            Transaction json = new Transaction();
                            json.source = (string)dataReader["source"];
                            json.target = address;
                            json.value = (ulong)dataReader["value"];
                            jsonList.Add(json);
                        }

                        //close Data Reader
                        dataReader.Close();

                        //close Connection
                        this.CloseConnection();

                        //return list to be displayed
                        return jsonList;
                    }catch(MySqlException e)
                    {
                        Console.WriteLine(e);
                        return null;
                    }
                }
                else
                {
                    return jsonList;
                }
        }
        public List<Transaction> getSentTo(string address)
        {
            List<Transaction> jsonList = new List<Transaction>();
            StringBuilder query = new StringBuilder();

            query.Append("select output.publicAddress as target,totals.value ");
            query.Append("from output,(SELECT input.transactionHash,output.value,output.publicAddress ");
            query.Append("from input ");
            query.Append("join output on input.previousTransactionHash = output.transactionHash and input.previousTransactionOutputIndex = output.outputIndex ");
            query.Append("where publicAddress = '");
            query.Append(address);
            query.Append("') AS totals ");
            query.Append("where totals.transactionHash = output.transactionHash;");

            if (this.OpenConnection() == true)
            {
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
                    Transaction json = new Transaction();
                    json.source = address;
                    json.target = (string)dataReader["target"];
                    json.value = (ulong)dataReader["value"];
                    jsonList.Add(json);
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return jsonList;
                }
                catch (MySqlException e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }
            else
            {
                return jsonList;
            }
        }
    }
}
