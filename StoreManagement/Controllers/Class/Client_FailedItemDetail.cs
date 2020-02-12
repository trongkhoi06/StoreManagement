using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_FailedItemDetail
    {
        public Client_FailedItemDetail()
        {

        }

        public Client_FailedItemDetail(Accessory accessory, Pack pack, ClassifyingSession classifyingSession, IdentifiedItem identifiedItem, FailedItem failedItem, SystemUser systemUser, PackedItem packedItem, double sample, double defectLimit, double sumIdentifiedQuantity, double sumCountedQuantity, double sumCheckedQuantity, double sumUnqualifiedQuantity, HashSet<string> boxIDs, string typeName)
        {
            FailedItemPK = failedItem.FailedItemPK;
            ExecutedDate = classifyingSession.ExecutedDate;
            Comment = classifyingSession.Comment;
            UserID = systemUser.UserID;
            Name = systemUser.Name;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            TypeName = typeName;
            PackID = pack.PackID;
            PackedQuantity = packedItem.PackedQuantity;
            Sample = sample;
            DefectLimit = defectLimit;
            SumIdentifiedQuantity = sumIdentifiedQuantity;
            SumCountedQuantity = sumCountedQuantity;
            SumCheckedQuantity = sumCheckedQuantity;
            SumUnqualifiedQuantity = sumUnqualifiedQuantity;
            BoxIDs = boxIDs;
        }

        public int FailedItemPK { get; set; }

        public DateTime ExecutedDate { get; set; }

        public string Comment { get; set; }

        public string UserID { get; set; }

        public string Name { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Art { get; set; }

        public string Color { get; set; }

        public string Item { get; set; }

        public string TypeName { get; set; }

        public string PackID { get; set; }

        public double PackedQuantity { get; set; }

        public double Sample { get; set; }

        public double DefectLimit { get; set; }

        public double SumIdentifiedQuantity { get; set; }

        public double SumCountedQuantity { get; set; }

        public double SumCheckedQuantity { get; set; }

        public double SumUnqualifiedQuantity { get; set; }

        public HashSet<string> BoxIDs { get; set; }
    }
}