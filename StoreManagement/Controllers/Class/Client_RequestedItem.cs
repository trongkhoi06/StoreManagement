using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_RequestedItem
    {
        public Client_RequestedItem()
        {
        }

        public Client_RequestedItem(RequestedItem requestedItem, Accessory accessory,double inStoredQuantity, List<Client_Box_Shelf_Row> requestedItemPosition)
        {
            RequestedItemPK = requestedItem.RequestPK;
            RequestedQuantity = requestedItem.RequestedQuantity;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            InStoredQuantity = inStoredQuantity;
            RequestedItemPosition = requestedItemPosition;
        }

        public int RequestedItemPK { get; set; }

        public double RequestedQuantity { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Art { get; set; }

        public string Color { get; set; }

        public string Item { get; set; }

        public double InStoredQuantity { get; set; }

        public List<Client_Box_Shelf_Row> RequestedItemPosition { get; set; }
    }

    public class Client_RequestedItem2
    {
        public Client_RequestedItem2()
        {
        }

        public Client_RequestedItem2(Request request, Accessory accessory, double inStoredQuantity, List<Client_Box_Shelf_Row2> requestedItemPosition)
        {
            RequestPK = request.RequestPK;
            RequestedQuantity = request.RequestID;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            InStoredQuantity = inStoredQuantity;
            RequestedItemPosition = requestedItemPosition;
        }

        public int RequestPK { get; set; }

        public string RequestedQuantity { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Art { get; set; }

        public string Color { get; set; }

        public string Item { get; set; }

        public double InStoredQuantity { get; set; }

        public List<Client_Box_Shelf_Row2> RequestedItemPosition { get; set; }
    }
}