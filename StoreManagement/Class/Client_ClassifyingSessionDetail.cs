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

        public Client_ClassifyingSessionDetail(Accessory accessory, Pack pack, ClassifyingSession classifyingSession, ClassifiedItem classifiedItem, PackedItem packedItem, int sample, int defectLimit, int sumIdentifiedQuantity, int sumCountedQuantity, int sumCheckedQuantity, int sumUnqualifiedQuantity)
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

        public int PackedQuantity { get; set; }

        public int Sample { get; set; }

        public int DefectLimit { get; set; }

        public int SumIdentifiedQuantity { get; set; }

        public int SumCountedQuantity { get; set; }

        public int SumCheckedQuantity { get; set; }

        public int SumUnqualifiedQuantity { get; set; }
    }
}