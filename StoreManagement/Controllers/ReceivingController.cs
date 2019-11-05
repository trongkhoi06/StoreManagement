using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Collections;
using StoreManagement.Class;
using StoreManagement.Models;

namespace StoreManagement.Controllers
{
    public class ReceivingController : ApiController
    {
        private UserModel db = new UserModel();
        [Route("api/ReceivingController/CreateOrderBusiness")]
        [HttpPost]
        public IHttpActionResult CreateOrderBusiness(string OrderID, int SupplierPK, string EmployeeCode, [FromBody] List<Client_Accessory_OrderedQuantity_Comment> list)
        {
            OrdersController ordersController = new OrdersController();
            Models.Order order = null;
            try
            {
                // create order
                order = ordersController.CreateOrder(OrderID, SupplierPK, EmployeeCode);
                // create order items
                OrderedItemsController orderedItemsController = new OrderedItemsController();
                if (!orderedItemsController.isOrderedItemCreated(order.OrderPK, list))
                {
                    if (order != null)
                    {
                        ordersController.DeleteOrder(order.OrderPK);
                    }
                    return Content(HttpStatusCode.Conflict, "Something is wrong!");
                }

            }
            catch (Exception e)
            {
                if (order != null)
                {
                    ordersController.DeleteOrder(order.OrderPK);
                }
                return Content(HttpStatusCode.Conflict, e.Message);
            }
            return Content(HttpStatusCode.OK, "TẠO ĐƠN HÀNG THÀNH CÔNG");
        }

        [Route("api/ReceivingController/UpdateOrderBusiness")]
        [HttpPut]
        public IHttpActionResult UpdateOrderBusiness([FromBody] OrderedItem orderedItems)
        {
            OrdersController ordersController = new OrdersController();
            OrderedItemsController orderedItemsController = new OrderedItemsController();
            try
            {
                if (ordersController.isContainPack(orderedItems.OrderPK))
                {
                    return Content(HttpStatusCode.Conflict, "ĐƠN HÀNG ĐÃ CHỨA PACK");
                }
                else
                {
                    if (!orderedItemsController.isUpdatedOrderedItem(orderedItems))
                    {
                        return Content(HttpStatusCode.Conflict, "UPDATE THẤT BẠI");
                    }
                }

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, e.Message);
            }
            return Content(HttpStatusCode.OK, "UPDATE THÀNH CÔNG");
        }

        [Route("api/ReceivingController/DeleteOrderBusiness")]
        [HttpDelete]
        public IHttpActionResult DeleteOrderBusiness(int orderPK)
        {
            List<OrderedItem> listOrderedItem;
            OrdersController ordersController = new OrdersController();
            OrderedItemsController orderedItemsController = new OrderedItemsController();
            try
            {
                Order order = ordersController.GetOrderByOrderPK(orderPK);

                IQueryable<OrderedItem> temp = orderedItemsController.GetOrderedItemsByOrderPK(order.OrderPK);
                listOrderedItem = temp.ToList();

                if (order == null || listOrderedItem == null)
                {
                    return Content(HttpStatusCode.Conflict, "CÓ LỖI");
                }

                for (int i = 0; i < listOrderedItem.Count; i++)
                {
                    orderedItemsController.DeleteOrderedItem(listOrderedItem[i].OrderedItemPK);
                }
                ordersController.DeleteOrder(order.OrderPK);
                return Content(HttpStatusCode.OK, "DELETE THÀNH CÔNG");
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, e.Message);
            }

        }

        [Route("api/ReceivingController/SwiftOrderState")]
        [HttpPut]
        public IHttpActionResult SwiftOrderState(int orderPK)
        {
            Order order = db.Orders.Find(orderPK);
            OrdersController ordersController = new OrdersController();
            try
            {
                ordersController.SwiftOrderState(orderPK);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, e.Message);
            }

            return Content(HttpStatusCode.OK, "SWIFT THÀNH CÔNG");
        }

        [Route("api/ReceivingController/CreateOrderBusiness")]
        [HttpPost]
        public IHttpActionResult CreatePackBusiness(string OrderID, int SupplierPK, string EmployeeCode, [FromBody] List<Client_Accessory_OrderedQuantity_Comment> list)
        {
            OrdersController ordersController = new OrdersController();
            Models.Order order = null;
            try
            {
                // create order
                order = ordersController.CreateOrder(OrderID, SupplierPK, EmployeeCode);
                // create order items
                OrderedItemsController orderedItemsController = new OrderedItemsController();
                if (!orderedItemsController.isOrderedItemCreated(order.OrderPK, list))
                {
                    if (order != null)
                    {
                        ordersController.DeleteOrder(order.OrderPK);
                    }
                    return Content(HttpStatusCode.Conflict, "Something is wrong!");
                }

            }
            catch (Exception e)
            {
                if (order != null)
                {
                    ordersController.DeleteOrder(order.OrderPK);
                }
                return Content(HttpStatusCode.Conflict, e.Message);
            }
            return Content(HttpStatusCode.OK, "TẠO ĐƠN HÀNG THÀNH CÔNG");
        }

        [Route("api/ReceivingController/UpdateOrderBusiness")]
        [HttpPut]
        public IHttpActionResult UpdatePackBusiness([FromBody] OrderedItem orderedItems)
        {
            OrdersController ordersController = new OrdersController();
            OrderedItemsController orderedItemsController = new OrderedItemsController();
            try
            {
                if (ordersController.isContainPack(orderedItems.OrderPK))
                {
                    return Content(HttpStatusCode.Conflict, "ĐƠN HÀNG ĐÃ CHỨA PACK");
                }
                else
                {
                    if (!orderedItemsController.isUpdatedOrderedItem(orderedItems))
                    {
                        return Content(HttpStatusCode.Conflict, "UPDATE THẤT BẠI");
                    }
                }

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, e.Message);
            }
            return Content(HttpStatusCode.OK, "UPDATE THÀNH CÔNG");
        }

        [Route("api/ReceivingController/DeleteOrderBusiness")]
        [HttpDelete]
        public IHttpActionResult DeletePackBusiness(int orderPK)
        {
            List<OrderedItem> listOrderedItem;
            OrdersController ordersController = new OrdersController();
            OrderedItemsController orderedItemsController = new OrderedItemsController();
            try
            {
                Order order = ordersController.GetOrderByOrderPK(orderPK);

                IQueryable<OrderedItem> temp = orderedItemsController.GetOrderedItemsByOrderPK(order.OrderPK);
                listOrderedItem = temp.ToList();

                if (order == null || listOrderedItem == null)
                {
                    return Content(HttpStatusCode.Conflict, "CÓ LỖI");
                }

                for (int i = 0; i < listOrderedItem.Count; i++)
                {
                    orderedItemsController.DeleteOrderedItem(listOrderedItem[i].OrderedItemPK);
                }
                ordersController.DeleteOrder(order.OrderPK);
                return Content(HttpStatusCode.OK, "DELETE THÀNH CÔNG");
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, e.Message);
            }

        }
    }
}
