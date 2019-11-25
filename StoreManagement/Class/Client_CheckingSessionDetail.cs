using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_CheckingSessionDetail
    {
        public Client_CheckingSessionDetail()
        {
        }

        public Client_CheckingSessionDetail(Accessory accessory, Pack pack, CheckingSession checkingSession)
        {
            CheckingSessionPK = checkingSession.CheckingSessionPK;
            ExecutedDate = checkingSession.ExecutedDate;
            CheckedQuantity = checkingSession.CheckedQuantity;
            UnqualifiedQuantity = checkingSession.UnqualifiedQuantity;
            Comment = checkingSession.Comment;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            PackID = pack.PackID;
        }

        public int CheckingSessionPK { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Art { get; set; }

        public string Color { get; set; }

        public string Item { get; set; }

        public string PackID { get; set; }

        public DateTime ExecutedDate { get; set; }

        public int CheckedQuantity { get; set; }

        public int UnqualifiedQuantity { get; set; }

        public string Comment { get; set; }

    }
}