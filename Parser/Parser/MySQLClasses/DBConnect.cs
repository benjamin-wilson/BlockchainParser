using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Parser.MySQLClasses
{
    public class DBConnect
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        //Constructor
        public DBConnect()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            server = "localhost";
            database = "mydb";
            uid = "admin";
            password = "blackandyellow";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        //open connection to database
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

        //Close connection
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
            query.Append("INSERT INTO output (value, publicAddress, outputIndex, TransactionHash, timestamp) VALUES");
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
                query.Append(BitConverter.ToString(output.transactionHash).Replace("-", string.Empty));
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
                query.Append(BitConverter.ToString(input.transactionHash).Replace("-", string.Empty));
                query.Append("','");
                query.Append(BitConverter.ToString(input.previousTransactionHash).Replace("-", string.Empty));
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
    }
}
