using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_PackedItemAngular
    {
        public Client_PackedItemAngular()
        {
        }

        public Client_PackedItemAngular(Accessory accessory, PackedItem packedItem, double actualQuantity)
        {

            PackedItemPK = packedItem.PackedItemPK;
            PackedQuantity = packedItem.PackedQuantity;
            ActualQuantity = actualQuantity;
            ContractNumber = packedItem.ContractNumber;
            AccessoryPK = accessory.AccessoryPK;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            Comment = packedItem.Comment;
        }

        public int PackedItemPK { get; set; }

        public double PackedQuantity { get; set; }

        public double ActualQuantity { get; set; }

        public string ContractNumber { get; set; }

        public int AccessoryPK { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Item { get; set; }

        public string Art { get; set; }

        public string Color { get; set; }

        public string Comment { get; set; }
    }
}