using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StoreManagement.Models;

namespace StoreManagement.Class
{
    public class Client_IdentifiedItemCheckedDetail
    {
        public int IdentifiedItemPK { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Art { get; set; }

        public string Color { get; set; }

        public string Item { get; set; }

        public int IdentifiedQuantitty { get; set; }

        public string PackID { get; set; }

        public int Sample { get; set; }

        public int SumCheckedQuantity { get; set; }

        public Client_IdentifiedItemCheckedDetail(IdentifiedItem identifiedItem, Accessory accessory, string packID, int sample, int sumCheckedQuantity)
        {
            IdentifiedItemPK = identifiedItem.IdentifiedItemPK;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            IdentifiedQuantitty = identifiedItem.IdentifiedQuantity;
            PackID = packID;
            Sample = sample;
            SumCheckedQuantity = sumCheckedQuantity;
        }
    }
}