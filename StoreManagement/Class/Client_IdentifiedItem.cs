using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StoreManagement.Models;

namespace StoreManagement.Class
{
    public class Client_IdentifiedItem
    {
        public int IdentifiedItemPK { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Art { get; set; }

        public string Model { get; set; }

        public string Item { get; set; }

        public int IdentifiedQuantitty { get; set; }

        public string PackID { get; set; }

        public Client_IdentifiedItem(IdentifiedItem identifiedItem,Accessory accessory,string packID)
        {
            IdentifiedItemPK = identifiedItem.IdentifiedItemPK;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Model = accessory.Model;
            Item = accessory.Item;
            IdentifiedQuantitty = identifiedItem.IdentifiedQuantity;
            PackID = packID;
        }
    }
}