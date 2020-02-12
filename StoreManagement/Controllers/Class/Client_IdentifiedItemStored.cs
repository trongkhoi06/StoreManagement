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

        public double ActualQuantity { get; set; }

        public string PackID { get; set; }

        public bool IsRestored { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Art { get; set; }

        public string Color { get; set; }

        public string Item { get; set; }

        public string TypeName { get; set; }

        public Client_IdentifiedItemStored(IdentifiedItem identifiedItem, Accessory accessory, Pack pack, double actualQuantity, string typeName)
        {
            IsRestored = false;
            IdentifiedItemPK = identifiedItem.IdentifiedItemPK;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            ActualQuantity = actualQuantity;
            PackID = pack.PackID;
            TypeName = typeName;
        }

        public Client_IdentifiedItemStored(RestoredGroup restoredGroup, Accessory accessory, Restoration restoration, string typeName)
        {
            IsRestored = true;
            IdentifiedItemPK = restoredGroup.RestoredGroupPK;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            ActualQuantity = restoredGroup.GroupQuantity;
            PackID = restoration.RestorationID;
            TypeName = typeName;
        }


    }
}