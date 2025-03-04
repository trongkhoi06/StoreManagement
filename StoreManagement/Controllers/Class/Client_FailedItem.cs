﻿using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_FailedItem
    {
        public Client_FailedItem()
        {

        }

        public Client_FailedItem(Accessory accessory, Pack pack, ClassifyingSession classifyingSession, FailedItem failedItem, double sumIdentifiedQuantity)
        {
            FailedItemPK = failedItem.FailedItemPK;
            ExecutedDate = classifyingSession.ExecutedDate;
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            PackID = pack.PackID;
            SumIdentifiedQuantity = sumIdentifiedQuantity;
        }

        public int FailedItemPK { get; set; }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Art { get; set; }

        public string Color { get; set; }

        public string Item { get; set; }

        public string PackID { get; set; }

        public double SumIdentifiedQuantity { get; set; }

        public DateTime ExecutedDate { get; set; }
    }
}