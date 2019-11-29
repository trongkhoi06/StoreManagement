using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_IdentifyingSession
    {
        public Client_IdentifyingSession()
        {
        }

        public Client_IdentifyingSession(Supplier supplier, Pack pack, IdentifyingSession session)
        {
            IdentifyingSessionPK = session.IdentifyingSessionPK;
            SupplierName = supplier.SupplierName;
            PackID = pack.PackID;
            DateCreated = session.ExecutedDate;
            IsOpened = pack.IsOpened;
        }
        public int IdentifyingSessionPK { get; set; }

        public string SupplierName { get; set; }

        public string PackID { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsOpened { get; set; }
    }
}