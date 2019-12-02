using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_RequestedItemDetail
    {
        public Client_RequestedItemDetail()
        {
        }

        public Client_RequestedItemDetail(Request request, Accessory accessory, double demandedQuantity, double sumOtherRequestedQuantity, double availableQuantity)
        {
            RequestPK = request.RequestPK;
            RequestedQuantity = request.RequestID;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            DemandedQuantity = demandedQuantity;
            SumOtherRequestedQuantity = sumOtherRequestedQuantity;
            AvailableQuantity = availableQuantity;
        }

        public int RequestPK { get; set; }

        public string RequestedQuantity { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Art { get; set; }

        public string Color { get; set; }

        public string Item { get; set; }

        public double DemandedQuantity { get; set; }

        public double SumOtherRequestedQuantity { get; set; }

        public double AvailableQuantity { get; set; }
    }
}