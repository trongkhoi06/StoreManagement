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

        public Client_CountingSessionDetail(Accessory accessory, Pack pack, CountingSession countingSession, IdentifiedItem identifiedItem)
        {
            CountingSessionPK = countingSession.CountingSessionPK;
            DateCreated = countingSession.ExecutedDate;
            CountedQuantity = countingSession.CountedQuantity;
            IdentifiedQuantity = identifiedItem.IdentifiedQuantity;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            PackID = pack.PackID;
        }

        public int CountingSessionPK { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Art { get; set; }

        public string Color { get; set; }

        public string Item { get; set; }

        public string PackID { get; set; }

        public DateTime DateCreated { get; set; }

        public int CountedQuantity { get; set; }

        public int IdentifiedQuantity { get; set; }

    }
}