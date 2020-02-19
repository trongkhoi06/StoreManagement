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

        public Client_CheckingSessionDetail(Accessory accessory, Pack pack, CheckingSession checkingSession, string boxID, PackedItem packedItem, double sample, string typeName)
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
            BoxID = boxID;
            IsClassified = packedItem.IsClassified;
            Sample = sample;
            TypeName = typeName;
        }

        public int CheckingSessionPK { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Art { get; set; }

        public string Color { get; set; }

        public string Item { get; set; }

        public string TypeName { get; set; }

        public string PackID { get; set; }

        public DateTime ExecutedDate { get; set; }

        public double CheckedQuantity { get; set; }

        public double UnqualifiedQuantity { get; set; }

        public string Comment { get; set; }

        public string BoxID { get; set; }

        public bool IsClassified { get; set; }

        public double Sample { get; set; }

    }
}