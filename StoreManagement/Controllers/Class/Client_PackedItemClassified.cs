using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_PackedItemClassified
    {
        public Client_PackedItemClassified()
        {
        }

        public Client_PackedItemClassified(Accessory accessory, Pack pack, PackedItem packedItem)
        {
            PackedItemPK = packedItem.PackedItemPK;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            PackID = pack.PackID;
            IsOpened = pack.IsOpened;
            QualityState = null;
            IsEditable = null;
        }

        public Client_PackedItemClassified(Accessory accessory, Pack pack, PackedItem packedItem, bool isEditable, ClassifiedItem classifiedItem)
        {
            PackedItemPK = packedItem.PackedItemPK;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            PackID = pack.PackID;
            IsOpened = pack.IsOpened;
            QualityState = classifiedItem.QualityState;
            IsEditable = isEditable;
        }

        public int PackedItemPK { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Art { get; set; }

        public string Color { get; set; }

        public string Item { get; set; }

        public string PackID { get; set; }

        public bool IsOpened { get; set; }

        public int? QualityState { get; set; }

        public bool? IsEditable { get; set; }

        
    }
}