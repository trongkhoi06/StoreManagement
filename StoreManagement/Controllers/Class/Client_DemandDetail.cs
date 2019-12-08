using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_DemandDetail
    {
        public Client_DemandDetail()
        {
        }

        public Client_DemandDetail(DemandedItem demandedItem, Accessory accessory, double totalRequestedQuantity, double availableQuantity)
        {
            DemandedItemPK = demandedItem.DemandedItemPK;
            DemandedQuantity = demandedItem.DemandedQuantity;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            TotalRequestedQuantity = totalRequestedQuantity;
            AvailableQuantity = availableQuantity;
        }

        public int DemandedItemPK { get; set; }

        public double DemandedQuantity { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Art { get; set; }

        public string Color { get; set; }

        public string Item { get; set; }

        public double TotalRequestedQuantity { get; set; }

        public double AvailableQuantity { get; set; }

    }
}