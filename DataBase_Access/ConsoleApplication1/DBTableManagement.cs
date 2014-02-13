using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class DBTableManagement
    {
        //This is for the address_table
        public void Insert(string address, string description, double balance)
        {
            DBConnect temp = new DBConnect();

            string query = "INSERT INTO `mydb`.`address_table` (`address`, `description`, `balance`) VALUES ('" +
                                                                  address +"', '"+ description +"', "+ balance +")";

            temp.Insert(query);
        }

        //This is for the association_table
        public void Insert(string id, string sender, string recipient, double amount)
        {
            DBConnect temp = new DBConnect();
            string query = "INSERT INTO `mydb`.`association_table` (`transaction_ID`, `sender`, `recipient`, `amount`) VALUES ("+
                                                                        id + ", '"+ sender +"', '" + recipient +"', "+ amount +")";

            temp.Insert(query);
        }

        //This is an update for the address_table
        //If the user does not want a value update then they need to enter -1
        public void Update(string address, string description, double balance)
        {
            DBConnect temp = new DBConnect();

            string query = "";

            if(description.Equals("-1"))
                 query = "UPDATE `mydb`.`address_table` SET balance=" + balance + " WHERE address='" + address + "'";

            else if(balance == -1)
                query = "UPDATE `mydb`.`address_table` SET description='" + description + "' WHERE address='" + address + "'";
            
            else
                query = "UPDATE `mydb`.`address_table` SET description='" + description + "', balance=" + balance + " WHERE address='" + address + "'";
            

            temp.Update(query);
        }

        public void Delete(string address)
        {
            DBConnect temp = new DBConnect();
            int x;
            string query = "";

            if(int.TryParse(address, out x))
            {
                query = "DELETE FROM `mydb`.`association_table` WHERE transaction_ID='"+address+"'";
            }
            else
            {
                query = "DELETE FROM `mydb`.`address_table` WHERE address='"+address+"'";
            }

            temp.Delete(query);
        }

        public List<List<string>> Select(string table)
        {
            DBConnect temp = new DBConnect();
            string query = "";
            List<List<string>> list = new List<List<string>>() ;

            if(table.Equals("Address"))
            {
                query = "SELECT * FROM `mydb`.`address_table`";
                list = temp.Select(query, 3);
            }
            else if(table.Equals("Association"))
            {
                query = "SELECT * FROM `mydb`.`association_table`";
                list = temp.Select(query, 4);
            }
            return list;
        }
    }
}
