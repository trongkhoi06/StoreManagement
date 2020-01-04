using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_Pack_Angular
    {
        public Client_Pack_Angular()
        {
        }

        public Client_Pack_Angular(Pack pack, string supplierName)
        {
            SupplierName = supplierName;
            PackPK = pack.PackPK;
            PackID = pack.PackID;
            DateCreated = pack.DateCreated;
            IsOpened = pack.IsOpened;
            OrderPK = pack.OrderPK;
            UserID = pack.UserID;
        }

        public Client_Pack_Angular(Pack pack, string supplierName, SystemUser systemUser)
        {
            SupplierName = supplierName;
            PackPK = pack.PackPK;
            PackID = pack.PackID;
            DateCreated = pack.DateCreated;
            IsOpened = pack.IsOpened;
            OrderPK = pack.OrderPK;
            UserID = pack.UserID;
            SystemUserName = systemUser.Name;
        }

        public int PackPK { get; set; }

        public string PackID { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsOpened { get; set; }

        public int OrderPK { get; set; }

        public string UserID { get; set; }

        public string SystemUserName { get; set; }

        public string SupplierName { get; set; }

        public bool IsIdentified { get; set; }

    }

    public class Client_Pack_Detail_Angular
    {
        public Client_Pack_Detail_Angular()
        {
        }

        public Client_Pack_Detail_Angular(Pack pack, string supplierName, string employeeName)
        {
            EmployeeName = employeeName;
            SupplierName = supplierName;
            PackPK = pack.PackPK;
            PackID = pack.PackID;
            DateCreated = pack.DateCreated;
            IsOpened = pack.IsOpened;
            OrderPK = pack.OrderPK;
            EmployeeCode = pack.UserID;
        }

        public int PackPK { get; set; }

        public string PackID { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsOpened { get; set; }

        public int OrderPK { get; set; }

        public string EmployeeCode { get; set; }

        public string EmployeeName { get; set; }

        public string SupplierName { get; set; }

    }
}