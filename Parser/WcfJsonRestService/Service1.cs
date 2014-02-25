using System;
using System.ServiceModel.Web;
using API;
using Database;
using System.Collections.Generic;

namespace WcfJsonRestService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {
        public List<JSON> getRecivedFrom(string id)
        {
            DBConnect database = new DBConnect();
            return database.getRecivedFrom(id);
        }

        public List<JSON> getSentTo(string id)
        {
            DBConnect database = new DBConnect();
            return database.getSentTo(id);
        }
    }
}
