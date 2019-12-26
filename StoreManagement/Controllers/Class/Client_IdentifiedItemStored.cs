using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StoreManagement.Models;

namespace StoreManagement.Class
{
    public class Client_IdentifiedItemStored
    {
        public int IdentifiedItemPK { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Art { get; set; }

        public string Color { get; set; }

        public string Item { get; set; }

        public double ActualQuantity { get; set; }

        public string PackID { get; set; }

        public Client_IdentifiedItemStored(IdentifiedItem identifiedItem, Accessory accessory, Pack pack, double actualQuantity)
        {
            IdentifiedItemPK = identifiedItem.IdentifiedItemPK;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            ActualQuantity = actualQuantity;
            PackID = pack.PackID;
        }

        
    }
}