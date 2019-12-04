using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_StoredBoxPK_IssuedQuantity
    {
        public Client_StoredBoxPK_IssuedQuantity()
        {
        }

        public Client_StoredBoxPK_IssuedQuantity(int storedBoxPK, double issuedQuantity)
        {
            StoredBoxPK = storedBoxPK;
            IssuedQuantity = issuedQuantity;
        }

        public int StoredBoxPK { get; set; }

        public double IssuedQuantity { get; set; }
    }
}
