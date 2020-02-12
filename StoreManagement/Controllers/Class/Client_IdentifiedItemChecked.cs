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

        public double IdentifiedQuantity { get; set; }

        public string PackID { get; set; }

        public bool? IsChecked { get; set; }

        public bool? IsOpened { get; set; }

        public bool? IsClassified { get; set; }

        public bool IsRestored { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Art { get; set; }

        public string Color { get; set; }

        public string Item { get; set; }

        public string TypeName { get; set; }

        public Client_IdentifiedItemChecked(IdentifiedItem identifiedItem, Accessory accessory, Pack pack, PackedItem packedItem, string typeName)
        {
            IsRestored = false;
            IdentifiedItemPK = identifiedItem.IdentifiedItemPK;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            IdentifiedQuantity = identifiedItem.IdentifiedQuantity;
            PackID = pack.PackID;
            IsChecked = identifiedItem.IsChecked;
            IsOpened = pack.IsOpened;
            IsClassified = packedItem.IsClassified;
            TypeName = typeName;
        }

        public Client_IdentifiedItemChecked(RestoredGroup restoredGroup, Accessory accessory, Restoration restoration, string typeName)
        {
            IsRestored = true;
            IdentifiedItemPK = restoredGroup.RestoredGroupPK;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            IdentifiedQuantity = restoredGroup.GroupQuantity;
            PackID = restoration.RestorationID;
            IsChecked = null;
            IsOpened = null;
            IsClassified = null;
            TypeName = typeName;
        }
    }
}