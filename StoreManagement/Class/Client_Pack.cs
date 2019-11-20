using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_Pack
    {
        public Client_Pack()
        {
        }

        public Client_Pack(Pack pack, string supplierName)
        {
            SupplierName = supplierName;
            this.PackPK = pack.PackPK;
            this.PackID = pack.PackID;
            this.DateCreated = pack.DateCreated;
        }

        public int PackPK { get; set; }

        public string PackID { get; set; }

        public DateTime DateCreated { get; set; }

        public string EmployeeCode { get; set; }

        public string SupplierName { get; set; }
    }
}