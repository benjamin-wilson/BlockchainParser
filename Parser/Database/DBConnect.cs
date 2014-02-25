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
            uid = "admin";
            password = "blackandyellow";
            timeout = "120";
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
        public void InsertOutputs(List<MySQLOutput> outputs)
        {
            StringBuilder query = new StringBuilder();
            query.Append("INSERT INTO output (value, publicAddress, outputIndex, transactionHash, timestamp) VALUES");
            bool isFirstElement = true;
            foreach(MySQLOutput output in outputs)
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
        public void InsertInputs(List<MySQLInput> inputs)
        {
            StringBuilder query = new StringBuilder();
            query.Append("INSERT INTO input (transactionHash, previousTransactionHash, previousTransactionOutputIndex) VALUES");
            bool isFirstElement = true;
            foreach (MySQLInput input in inputs)
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
        public List<MySQLOutput> SelectOutputsBasedOnAddress(string address)
        {
            string query = "SELECT * FROM mydb.output where publicAddress = '"+address+"'";
            List<MySQLOutput> outputs = new List<MySQLOutput>();

            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.CommandTimeout = 120;
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    MySQLOutput output = new MySQLOutput();
                    
                    output.value = (ulong)dataReader["value"];
                    output.publicAddress = (string)dataReader["publicAddress"];
                    output.index = (uint)dataReader["outputIndex"];
                    output.transactionHash = (string)dataReader["transactionHash"];
                    output.timestamp = (uint)dataReader["timestamp"];
                    outputs.Add(output);
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return outputs;
            }
            else
            {
                return outputs;
            }
        }
        public List<MySQLInput> SelectInputsBasedOnTransactionHash(string hash)
        {
            string query = "SELECT * FROM mydb.input where transactionHash = '" + hash + "'";
            List<MySQLInput> inputs = new List<MySQLInput>();

            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.CommandTimeout = 120;
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    MySQLInput input = new MySQLInput();

                    input.transactionHash = (string)dataReader["transactionHash"];
                    input.previousTransactionHash = (string)dataReader["previousTransactionHash"];
                    input.previousTransactionOutputIndex = (uint)dataReader["previousTransactionOutputIndex"];
                    inputs.Add(input);
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return inputs;
            }
            else
            {
                return inputs;
            }
        }

        public List<MySQLInput> SelectInputsBasedOnPreviousTransactionHashAndPreviousIndex(string hash, uint index)
        {
            string query = "SELECT * FROM mydb.input where previousTransactionHash = '" + hash + "'"+" and previousTransactionOutputIndex = "+index.ToString();
            List<MySQLInput> inputs = new List<MySQLInput>();

            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.CommandTimeout = 120;
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    MySQLInput input = new MySQLInput();

                    input.transactionHash = (string)dataReader["transactionHash"];
                    input.previousTransactionHash = (string)dataReader["previousTransactionHash"];
                    input.previousTransactionOutputIndex = (uint)dataReader["previousTransactionOutputIndex"];
                    inputs.Add(input);
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return inputs;
            }
            else
            {
                return inputs;
            }
        }
        public List<MySQLOutput> SelectOutputsBasedOnTransactionHashAndIndex(string hash, uint index)
        {
            string query = "SELECT * FROM mydb.output where transactionHash = '" + hash + "'"+" and outputIndex = "+index.ToString();
            List<MySQLOutput> outputs = new List<MySQLOutput>();

            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.CommandTimeout = 120;
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    MySQLOutput output = new MySQLOutput();

                    output.value = (ulong)dataReader["value"];
                    output.publicAddress = (string)dataReader["publicAddress"];
                    output.index = (uint)dataReader["outputIndex"];
                    output.transactionHash = (string)dataReader["transactionHash"];
                    output.timestamp = (uint)dataReader["timestamp"];
                    outputs.Add(output);
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return outputs;
            }
            else
            {
                return outputs;
            }
        }

        public List<JSON> getRecivedFrom(string address)
        {
            List<JSON> jsonList = new List<JSON>();
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
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    //Read the data and store them in the list
                    while (dataReader.Read())
                    {
                        JSON json = new JSON();
                        json.source = (string)dataReader["source"];
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
                else
                {
                    return jsonList;
                }
        }

        public List<JSON> getSentTo(string address)
        {
            List<JSON> jsonList = new List<JSON>();
            StringBuilder query = new StringBuilder();
            query.Append("select totals.publicAddress as source ,totals.value,output.publicAddress as target ");
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
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    JSON json = new JSON();
                    json.source = (string)dataReader["source"];
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
            else
            {
                return jsonList;
            }
        }

        public IEnumerable<MySQLOutput> SelectOutputsBasedOnTransactionHash(string hash)
        {
            string query = "SELECT * FROM mydb.output where transactionHash = '" + hash + "'";
            List<MySQLOutput> outputs = new List<MySQLOutput>();

            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.CommandTimeout = 120;
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    MySQLOutput output = new MySQLOutput();

                    output.value = (ulong)dataReader["value"];
                    output.publicAddress = (string)dataReader["publicAddress"];
                    output.index = (uint)dataReader["outputIndex"];
                    output.transactionHash = (string)dataReader["transactionHash"];
                    output.timestamp = (uint)dataReader["timestamp"];
                    outputs.Add(output);
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return outputs;
            }
            else
            {
                return outputs;
            }
        }
    }
}
