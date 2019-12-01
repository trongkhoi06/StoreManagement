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
using StoreManagement.Class;
using StoreManagement.Models;

namespace StoreManagement.Controllers
{
    public class OrderedItemsDAO
    {
        private UserModel db = new UserModel();
        
        public IQueryable<OrderedItem> GetOrderedItems()
        {
            return db.OrderedItems;
        }
        
        public OrderedItem GetOrderedItem(int pk)
        {
            OrderedItem orderedItem = db.OrderedItems.Find(pk);
            return orderedItem;
        }

        public IQueryable<OrderedItem> GetOrderedItemsByOrderPK(int orderPK)
        {
            try
            {
                //SqlParameter OrderParam = new SqlParameter("@OrderPK", orderPK);
                //result = (db.Database.SqlQuery<OrderedItem>("exec GetOrderedItemByOrderPK @OrderPK", OrderParam).ToList());
                return (from oi in db.OrderedItems
                       where oi.OrderPK == orderPK
                       select oi);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private bool OrderedItemExists(int pk)
        {
            return db.OrderedItems.Count(e => e.OrderedItemPK == pk) > 0;
        }

        public bool isUpdatedOrderedItem(OrderedItem orderedItem)
        {
            OrderedItem dbOrderedItem = GetOrderedItem(orderedItem.OrderedItemPK);
            if (dbOrderedItem.OrderPK == orderedItem.OrderPK)
            {
                if (orderedItem.OrderedQuantity == 0)
                {
                    db.OrderedItems.Remove(dbOrderedItem);
                }
                else
                {
                    dbOrderedItem.Comment = orderedItem.Comment;
                    dbOrderedItem.OrderedQuantity = orderedItem.OrderedQuantity;
                    db.Entry(dbOrderedItem).State = EntityState.Modified;
                }
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    throw e;
                }

                return true;
            }
            else
            {
                throw new Exception("KHÔNG ĐƯỢC THAY ĐỔI ORDERPK");
            }
        }

        public bool isOrderedItemCreated(int OrderPK, List<Client_Accessory_OrderedQuantity_Comment> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                db.OrderedItems.Add(new OrderedItem(OrderPK, list[i]));
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;
        }

        public void DeleteOrderedItem(int OrderedItemPK)
        {
            OrderedItem  orderedItem = db.OrderedItems.Find(OrderedItemPK);
            if (orderedItem == null)
            {
                throw new Exception("ITEM ĐƠN HÀNG KHÔNG TỒN TẠI");
            }
            db.OrderedItems.Remove(orderedItem);
            db.SaveChanges();
        }
    }
}