using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            DBTableManagement temp = new DBTableManagement();

            //temp.Insert("3", "newSender", "newReciver", 10.00);
            //temp.Update("newAddress2", "The balance stays the same this is updated", -1);
            //temp.Delete("newAddress2");
            //temp.Delete("3");

            var list = temp.Select("Address");
            foreach(var t in list)
            {
                foreach(var j in t)
                {
                    Console.WriteLine(j);
                }
            }
        }
    }
}
