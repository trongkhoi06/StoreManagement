using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_PackedItem
    {
        public Client_PackedItem()
        {
        }

        public Client_PackedItem(Accessory accessory, PackedItem packedItem)
        {
            this.PackedItemPK = packedItem.PackedItemPK;
            this.PackedQuantity = packedItem.PackedQuantity;
            this.AccessoryID = accessory.AccessoryID;
            this.AccessoryDescription = accessory.AccessoryDescription;
        }

        public int PackedItemPK { get; set; }

        public double PackedQuantity { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }
    }
}