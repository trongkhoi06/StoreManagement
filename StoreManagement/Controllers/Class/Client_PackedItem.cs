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

        public Client_PackedItem(Accessory accessory, PackedItem packedItem, string accessoryTypeName)
        {
            PackedItemPK = packedItem.PackedItemPK;
            PackedQuantity = packedItem.PackedQuantity;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Comment = packedItem.Comment;
            AccessoryTypeName = accessoryTypeName;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
        }

        public int PackedItemPK { get; set; }

        public double PackedQuantity { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Art { get; set; }

        public string Color { get; set; }

        public string Item { get; set; }

        public string AccessoryTypeName { get; set; }

        public string Comment { get; set; }
    }
}