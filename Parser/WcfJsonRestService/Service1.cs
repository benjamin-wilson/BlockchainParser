using System;
using System.ServiceModel.Web;
using API;
using Database;
using System.Collections.Generic;
using Mining;

namespace WcfJsonRestService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {
        public JSONHolder getjson(string id)
        {
            DBConnect database = new DBConnect();
            return Mining.Mining.Mine(id);
        }
    }
}
