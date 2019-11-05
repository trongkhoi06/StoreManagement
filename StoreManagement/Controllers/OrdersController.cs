using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using StoreManagement.Models;

namespace StoreManagement.Controllers
{
    public class OrdersController
    {
        private UserModel db = new UserModel();


        public IQueryable<Order> GetOrders()
        {
            return db.Orders;
        }
        public Order GetOrderByOrderPK(int orderPK)
        {
            return db.Orders.Find(orderPK);
        }
        public Order GetOrderByOrderID(string orderID)
        {
            try
            {
                SqlParameter OrderParam = new SqlParameter("@OrderID", orderID);
                Order order = db.Database.SqlQuery<Order>("exec GetOrder @OrderID", OrderParam).FirstOrDefault();
                return order;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Order CreateOrder(string OrderID, int SupplierPK, string EmployeeCode)
        {
            PrimitiveType primitiveType = new PrimitiveType();
            if (primitiveType.isOrderID(OrderID))
            {
                // Khởi tạo order
                Order order = new Order(OrderID, SupplierPK, EmployeeCode);
                db.Orders.Add(order);
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateException e)
                {
                    if (OrderExists(order.OrderID))
                    {
                        throw new Exception("OrderID ĐÃ TỒN TẠI");
                    }
                    else
                    {
                        throw e;
                    }
                }
                return order;
            }
            else
            {
                throw new Exception("MÃ ĐƠN ĐẶT HÀNG KHÔNG PHÙ HỢP");
            }
        }

        public void DeleteOrder(int OrderPK)
        {
            Order order = db.Orders.Find(OrderPK);
            if (order == null)
            {
                throw new Exception("ĐƠN HÀNG KHÔNG TỒN TẠI");
            }
            db.Orders.Remove(order);
            db.SaveChanges();
        }

        private bool OrderExists(string id)
        {
            return db.Orders.Count(e => e.OrderID == id) > 0;
        }

        public bool isContainPack(int orderPK)
        {
            bool result = false;
            try
            {
                SqlParameter OrderParam = new SqlParameter("@OrderPK", orderPK);
                result = (db.Database.SqlQuery<string>("exec isContainPack @OrderPK", OrderParam).FirstOrDefault()).Equals("True");
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }

        public void SwiftOrderState(int orderPK)
        {
            Order order = db.Orders.Find(orderPK);
            order.IsOpened = !order.IsOpened;
            db.Entry(order).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}