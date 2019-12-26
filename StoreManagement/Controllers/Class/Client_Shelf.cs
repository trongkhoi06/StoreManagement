using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_Shelf
    {
        public Client_Shelf()
        {
        }

        public Client_Shelf(string shelfID, string rowID)
        {
            ShelfID = shelfID;
            RowID = rowID;
        }

        public string ShelfID { get; set; }

        public string RowID { get; set; }
    }
}
