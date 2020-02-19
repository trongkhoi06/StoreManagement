using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_CountingSessionDetail
    {
        public Client_CountingSessionDetail()
        {
        }

        public Client_CountingSessionDetail(Accessory accessory, Pack pack, CountingSession countingSession, IdentifiedItem identifiedItem, string boxID, PackedItem packedItem,string typeName)
        {
            CountingSessionPK = countingSession.CountingSessionPK;
            ExecutedDate = countingSession.ExecutedDate;
            CountedQuantity = countingSession.CountedQuantity;
            IdentifiedQuantity = identifiedItem.IdentifiedQuantity;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            PackID = pack.PackID;
            BoxID = boxID;
            IsClassified = packedItem.IsClassified;
            TypeName = typeName;
        }

        public int CountingSessionPK { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Art { get; set; }

        public string Color { get; set; }

        public string Item { get; set; }

        public string PackID { get; set; }

        public DateTime ExecutedDate { get; set; }

        public double CountedQuantity { get; set; }

        public double IdentifiedQuantity { get; set; }

        public string BoxID { get; set; }

        public bool IsClassified { get; set; }

        public string TypeName { get; set; }
    }
}