using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_Order
    {
        public Client_Order()
        {
        }

        public Client_Order(Order order)
        {
            OrderID = order.OrderID;
            DateCreated = order.DateCreated;
            IsOpened = order.IsOpened;
            SupplierPK = order.SupplierPK;
            EmployeeCode = order.EmployeeCode;
        }


        public string OrderID { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsOpened { get; set; }

        public int SupplierPK { get; set; }

        public string EmployeeCode { get; set; }
    }
}