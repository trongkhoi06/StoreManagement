using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_Box_Shelf_Row
    {
        public Client_Box_Shelf_Row()
        {
        }

        public Client_Box_Shelf_Row(string boxID, string shelfID, string rowID, double inBoxQuantity)
        {
            BoxID = boxID;
            ShelfID = shelfID;
            RowID = rowID;
            InBoxQuantity = inBoxQuantity;
        }

        public string BoxID { get; set; }

        public string ShelfID { get; set; }

        public string RowID { get; set; }

        public double InBoxQuantity { get; set; }
    }
}
