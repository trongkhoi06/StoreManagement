using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_DemandedItem
    {
        public Client_DemandedItem()
        {
        }

        public Client_DemandedItem(DemandedItem demandedItem, Accessory accessory, double? issuedQuantity, List<Client_Box_Shelf_Row> itemPosition, string typeName)
        {
            DemandedItemPK = demandedItem.DemandedItemPK;
            DemandedQuantity = demandedItem.DemandedQuantity;
            AccessoryPK = accessory.AccessoryPK;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            IssuedQuantity = issuedQuantity;
            ItemPosition = itemPosition;
        }

        public int DemandedItemPK { get; set; }

        public double DemandedQuantity { get; set; }

        public int AccessoryPK { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Art { get; set; }

        public string Color { get; set; }

        public string Item { get; set; }

        public string TypeName { get; set; }

        public double? IssuedQuantity { get; set; }

        public List<Client_Box_Shelf_Row> ItemPosition { get; set; }
    }

    //public class Client_RequestedItem2
    //{
    //    public Client_RequestedItem2()
    //    {
    //    }

    //    public Client_RequestedItem2(Request request, Accessory accessory, double inStoredQuantity, List<Client_Box_Shelf_Row2> requestedItemPosition)
    //    {
    //        RequestPK = request.RequestPK;
    //        RequestedQuantity = request.RequestID;
    //        AccessoryID = accessory.AccessoryID;
    //        AccessoryDescription = accessory.AccessoryDescription;
    //        Art = accessory.Art;
    //        Color = accessory.Color;
    //        Item = accessory.Item;
    //        InStoredQuantity = inStoredQuantity;
    //        RequestedItemPosition = requestedItemPosition;
    //    }

    //    public int RequestPK { get; set; }

    //    public string RequestedQuantity { get; set; }

    //    public string AccessoryID { get; set; }

    //    public string AccessoryDescription { get; set; }

    //    public string Art { get; set; }

    //    public string Color { get; set; }

    //    public string Item { get; set; }

    //    public double InStoredQuantity { get; set; }

    //    public List<Client_Box_Shelf_Row2> RequestedItemPosition { get; set; }
    //}
}