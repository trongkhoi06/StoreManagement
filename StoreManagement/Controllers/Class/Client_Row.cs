using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_Row
    {
        public Client_Row()
        {

        }

        public Client_Row(string boxID, string rowID)
        {
            BoxID = boxID;
            RowID = rowID;
        }

        public string BoxID { get; set; }

        public string RowID { get; set; }
    }
}
