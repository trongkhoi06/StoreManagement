using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StoreManagement.Models;

namespace StoreManagement.Class
{
    public class Client_IdentifiedItemChecked
    {
        public int IdentifiedItemPK { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Art { get; set; }

        public string Color { get; set; }

        public string Item { get; set; }

        public double IdentifiedQuantitty { get; set; }

        public string PackID { get; set; }

        public bool IsChecked { get; set; }

        public bool IsOpened { get; set; }

        public bool IsClassified { get; set; }

        public Client_IdentifiedItemChecked(IdentifiedItem identifiedItem,Accessory accessory,Pack pack,PackedItem packedItem)
        {
            IdentifiedItemPK = identifiedItem.IdentifiedItemPK;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            IdentifiedQuantitty = identifiedItem.IdentifiedQuantity;
            PackID = pack.PackID;
            IsChecked = identifiedItem.IsChecked;
            IsOpened = pack.IsOpened;
            IsClassified = packedItem.IsClassified;
        }
    }
}