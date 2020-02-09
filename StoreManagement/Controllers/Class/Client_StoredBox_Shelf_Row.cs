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

        public Client_Box_Shelf_Row(string boxID, int storedBoxPK, string shelfID, string rowID, int itemPK, bool isRestored, string containerID, double? availableQuantity)
        {
            BoxID = boxID;
            StoredBoxPK = storedBoxPK;
            ShelfID = shelfID;
            RowID = rowID;
            ItemPK = itemPK;
            ContainerID = containerID;
            IsRestored = isRestored;
            AvailableQuantity = availableQuantity;
        }

        public string BoxID { get; set; }

        public int StoredBoxPK { get; set; }

        public string ShelfID { get; set; }

        public string RowID { get; set; }

        public int ItemPK { get; set; }

        public string ContainerID { get; set; }

        public bool IsRestored { get; set; }

        public double? AvailableQuantity { get; set; }

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
