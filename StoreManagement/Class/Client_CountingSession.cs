using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_CountingSession
    {
        public Client_CountingSession()
        {
        }

        public Client_CountingSession(Accessory accessory, PackedItem packedItem)
        {

            PackedItemPK = packedItem.PackedItemPK;
            AccessoryPK = accessory.AccessoryPK;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            Comment = accessory.Comment;
        }

        public int PackedItemPK { get; set; }

        public int AccessoryPK { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Art { get; set; }

        public string Color { get; set; }

        public string Item { get; set; }

        public string Comment { get; set; }
    }
}