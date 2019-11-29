using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_ClassifyingSessionDetail
    {
        public Client_ClassifyingSessionDetail()
        {

        }

        public Client_ClassifyingSessionDetail(Accessory accessory, Pack pack, ClassifyingSession classifyingSession, ClassifiedItem classifiedItem, PackedItem packedItem, double sample, double defectLimit, double sumIdentifiedQuantity, double sumCountedQuantity, double sumCheckedQuantity, double sumUnqualifiedQuantity)
        {
            ClassifyingSessionPK = classifyingSession.ClassifyingSessionPK;
            ExecutedDate = classifyingSession.ExecutedDate;
            Comment = classifyingSession.Comment;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            PackID = pack.PackID;
            QualityState = classifiedItem.QualityState;
            PackedQuantity = packedItem.PackedQuantity;
            Sample = sample;
            DefectLimit = defectLimit;
            SumIdentifiedQuantity = sumIdentifiedQuantity;
            SumCountedQuantity = sumCountedQuantity;
            SumCheckedQuantity = sumCheckedQuantity;
            SumUnqualifiedQuantity = sumUnqualifiedQuantity;
        }

        public int ClassifyingSessionPK { get; set; }

        public DateTime ExecutedDate { get; set; }

        public string Comment { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Art { get; set; }

        public string Color { get; set; }

        public string Item { get; set; }

        public string PackID { get; set; }

        public int QualityState { get; set; }

        public double PackedQuantity { get; set; }

        public double Sample { get; set; }

        public double DefectLimit { get; set; }

        public double SumIdentifiedQuantity { get; set; }

        public double SumCountedQuantity { get; set; }

        public double SumCheckedQuantity { get; set; }

        public double SumUnqualifiedQuantity { get; set; }
    }
}