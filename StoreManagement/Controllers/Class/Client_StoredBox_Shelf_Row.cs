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

        public Client_Box_Shelf_Row(string boxID, string shelfID, string rowID, int itemPK, bool isRestored, double inBoxQuantity)
        {
            BoxID = boxID;
            ShelfID = shelfID;
            RowID = rowID;
            ItemPK = itemPK;
            IsRestored = isRestored;
            InBoxQuantity = inBoxQuantity;
        }

        public string BoxID { get; set; }

        public string ShelfID { get; set; }

        public string RowID { get; set; }

        public int ItemPK { get; set; }

        public bool IsRestored { get; set; }

        public double InBoxQuantity { get; set; }

    }

    public class Client_Box_Shelf_Row2
    {
        public Client_Box_Shelf_Row2()
        {
        }

        public Client_Box_Shelf_Row2(string boxID, string shelfID, string rowID, int itemPK, bool isRestored, double issuingQuantity)
        {
            BoxID = boxID;
            ShelfID = shelfID;
            RowID = rowID;
            ItemPK = itemPK;
            IsRestored = isRestored;
            IssuingQuantity = issuingQuantity;
        }

        public string BoxID { get; set; }

        public string ShelfID { get; set; }

        public string RowID { get; set; }

        public int ItemPK { get; set; }

        public bool IsRestored { get; set; }

        public double IssuingQuantity { get; set; }

    }
}
