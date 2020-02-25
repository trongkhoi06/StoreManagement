using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StoreManagement.Models;

namespace StoreManagement.Class
{
    public class Client_IdentifiedItemRead
    {
        public int IdentifiedItemPK { get; set; }

        public double IdentifiedQuantity { get; set; }

        public string PackID { get; set; }

        public int? QualityState { get; set; }

        public bool IsRestored { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Art { get; set; }

        public string Color { get; set; }

        public string Item { get; set; }

        public string TypeName { get; set; }

        public Client_IdentifiedItemRead(IdentifiedItem identifiedItem, Accessory accessory, string packID, int? qualityState, string typeName, double actualQuantity)
        {
            IsRestored = false;
            IdentifiedItemPK = identifiedItem.IdentifiedItemPK;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            IdentifiedQuantity = actualQuantity;
            PackID = packID;
            QualityState = qualityState;
            TypeName = typeName;
        }

        public Client_IdentifiedItemRead(RestoredGroup restoredGroup, Accessory accessory, string restorationID,string typeName)
        {
            IsRestored = true;
            IdentifiedItemPK = restoredGroup.RestoredGroupPK;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            IdentifiedQuantity = restoredGroup.GroupQuantity;
            PackID = restorationID;
            QualityState = null;
            TypeName = typeName;
        }
    }
}