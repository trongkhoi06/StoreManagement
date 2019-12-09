using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_OrderAngular
    {
        public Client_OrderAngular()
        {
        }

        public Client_OrderAngular(Order order, bool isContainPack)
        {
            OrderPK = order.OrderPK;
            OrderID = order.OrderID;
            DateCreated = order.DateCreated;
            IsOpened = order.IsOpened;
            SupplierPK = order.SupplierPK;
            UserID = order.UserID;
            IsContainPack = isContainPack;
        }

        public int OrderPK { get; set; }

        public string OrderID { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsOpened { get; set; }

        public int SupplierPK { get; set; }

        public string UserID { get; set; }

        public bool IsContainPack { get; set; }
    }
}