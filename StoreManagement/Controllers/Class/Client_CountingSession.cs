﻿using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_CountingSession
    {
        public Client_CountingSession()
        {
        }

        public Client_CountingSession(Accessory accessory, Pack pack, CountingSession countingSession)
        {
            CountingSessionPK = countingSession.CountingSessionPK;
            ExecutedDate = countingSession.ExecutedDate;
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

        public DateTime ExecutedDate { get; set; }
    }
}