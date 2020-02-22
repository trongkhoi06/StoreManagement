using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_ClassifyingSession
    {
        public Client_ClassifyingSession()
        {

        }

        public Client_ClassifyingSession(Accessory accessory, Pack pack, ClassifyingSession classifyingSession, ClassifiedItem classifiedItem, bool isStoredOrReturn, string typeName)
        {
            ClassifyingSessionPK = classifyingSession.ClassifyingSessionPK;
            ExecutedDate = classifyingSession.ExecutedDate;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            TypeName = typeName;
            PackID = pack.PackID;
            QualityState = classifiedItem.QualityState;
            IsStoredOrReturn = isStoredOrReturn;
        }

        public int ClassifyingSessionPK { get; set; }

        public DateTime ExecutedDate { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Art { get; set; }

        public string Color { get; set; }

        public string Item { get; set; }

        public string PackID { get; set; }

        public string TypeName { get; set; }

        public int QualityState { get; set; }

        public bool IsStoredOrReturn { get; set; }
    }
}