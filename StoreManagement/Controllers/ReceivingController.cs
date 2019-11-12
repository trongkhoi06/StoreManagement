using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Collections;
using StoreManagement.Class;
using StoreManagement.Models;
using System.Web.Http.Cors;

namespace StoreManagement.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReceivingController : ApiController
    {
        private UserModel db = new UserModel();
        // Get Accessory
        [Route("api/ReceivingController/GetAccessory")]
        [HttpGet]
        public IHttpActionResult GetAccessory()
        {
            List<Accessory> accessories;
            try
            {
                accessories = (from a in db.Accessories
                               select a).ToList();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, accessories);
        }

        // Get Supplier
        [Route("api/ReceivingController/GetSupplier")]
        [HttpGet]
        public IHttpActionResult GetSupplier()
        {
            List<Supplier> suppliers;
            try
            {
                suppliers = (from s in db.Suppliers
                             select s).ToList();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, suppliers);
        }

        // Order
        [Route("api/ReceivingController/GetOrder")]
        [HttpGet]
        public IHttpActionResult GetOrderWithFilter(DateTime start, DateTime end)
        {
            List<Order> orders;
            List<Client_Order> client_Orders = new List<Client_Order>();
            // make it one more day to make sure < end will be right answer
            end = end.AddDays(1);
            try
            {
                // if start > 1900 then select query
                if (start.Year > 1900)
                {
                    orders = (from o in db.Orders
                              where o.DateCreated >= start && o.DateCreated < end
                              select o).ToList();
                }
                // if start <= 1900 then select all
                else
                {
                    orders = (from o in db.Orders
                              select o).ToList();
                }

                foreach (var order in orders)
                {
                    client_Orders.Add(new Client_Order(order));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, client_Orders);
        }

        [Route("api/ReceivingController/CreateOrderBusiness")]
        [HttpPost]
        public IHttpActionResult CreateOrderBusiness(string OrderID, int SupplierPK, string EmployeeCode, [FromBody] List<Client_Accessory_OrderedQuantity_Comment> list)
        {
            OrdersController ordersController = new OrdersController();
            Order order = null;
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
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
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
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
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
                if (ordersController.isContainPack(orderPK))
                {
                    return Content(HttpStatusCode.Conflict, "ĐƠN HÀNG ĐÃ CHỨA PACK");
                }
                else
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
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
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
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, "SWIFT THÀNH CÔNG");
        }

        // Pack
        [Route("api/ReceivingController/CreatePackBusiness")]
        [HttpPost]
        public IHttpActionResult CreatePackBusiness(string PackID, int OrderPK, [FromBody] List<Client_OrderedItemPK_PackedQuantity_Comment> list)
        {
            Order order = db.Orders.Find(OrderPK);
            if (order.IsOpened)
            {
                PacksController packsController = new PacksController();
                Pack pack = null;
                try
                {
                    // create pack
                    pack = packsController.CreatePack(PackID, OrderPK);
                    // create pack items
                    PackedItemsController packedItemsController = new PackedItemsController();
                    if (!packedItemsController.isPackedItemCreated(pack.PackPK, list))
                    {
                        if (pack != null)
                        {
                            packsController.DeletePack(pack.PackPK);
                        }
                        return Content(HttpStatusCode.Conflict, "Something is wrong!");
                    }

                }
                catch (Exception e)
                {
                    if (pack != null)
                    {
                        packsController.DeletePack(pack.PackPK);
                    }
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "TẠO PACK THÀNH CÔNG");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "ĐƠN HÀNG ĐÃ ĐÓNG NÊN KHÔNG TẠO ĐƯỢC PACK");
            }
        }

        [Route("api/ReceivingController/UpdatePackBusiness")]
        [HttpPut]
        public IHttpActionResult UpdatePackBusiness([FromBody] PackedItem packedItem)
        {
            PacksController packsController = new PacksController();
            PackedItemsController packedItemsController = new PackedItemsController();
            try
            {
                if (packsController.isContainIdentifiedItem(packedItem.PackPK))
                {
                    return Content(HttpStatusCode.Conflict, "ĐƠN HÀNG ĐÃ CHỨA PACK");
                }
                else
                {
                    if (!packedItemsController.isUpdatedPackedItem(packedItem))
                    {
                        return Content(HttpStatusCode.Conflict, "UPDATE THẤT BẠI");
                    }
                }

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, "UPDATE THÀNH CÔNG");
        }

        [Route("api/ReceivingController/DeletePackBusiness")]
        [HttpDelete]
        public IHttpActionResult DeletePackBusiness(int packPK)
        {
            List<PackedItem> listPackedItem;
            PacksController packsController = new PacksController();
            PackedItemsController packedItemsController = new PackedItemsController();
            try
            {
                Pack pack = packsController.GetPackByPackPK(packPK);

                IQueryable<PackedItem> temp = packedItemsController.GetPackedItemsByPackPK(pack.PackPK);
                listPackedItem = temp.ToList();

                if (packsController.isContainIdentifiedItem(packPK))
                {
                    return Content(HttpStatusCode.Conflict, "PACK ĐÃ CHỨA CLASSIFIED ITEM");
                }
                else if (pack == null || listPackedItem == null)
                {
                    return Content(HttpStatusCode.Conflict, "CÓ LỖI");
                }

                for (int i = 0; i < listPackedItem.Count; i++)
                {
                    packedItemsController.DeletePackedItem(listPackedItem[i].PackedItemPK);
                }
                packsController.DeletePack(pack.PackPK);
                return Content(HttpStatusCode.OK, "DELETE THÀNH CÔNG");
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        [Route("api/ReceivingController/SwiftPackState")]
        [HttpPut]
        public IHttpActionResult SwiftPackState(int packPK)
        {
            Pack pack = db.Packs.Find(packPK);
            PacksController packsController = new PacksController();
            try
            {
                packsController.SwiftPackState(packPK);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, "SWIFT THÀNH CÔNG");
        }

        // Identify
        [Route("api/ReceivingController/IdentifyItemBusiness")]
        [HttpPost]
        public IHttpActionResult IdentifyItemBusiness(string BoxID, [FromBody] List<Client_PackItemPK_ActualQuantity> list)
        {
            // kiểm trước khi chạy lệnh

            // chạy lệnh identify
            try
            {
                IdentifyItemController identifyItemController = new IdentifyItemController();
                // kiếm Box by box ID
                Box box = (from b in db.Boxes
                           where b.BoxID == BoxID
                           select b).FirstOrDefault();
                // kiểm xem box đã store hay chưa
                StoredBox sBox = (from sB in db.StoredBoxes
                                  where sB.BoxPK == box.BoxPK
                                  select sB).FirstOrDefault();
                if (sBox != null)
                {
                    return Content(HttpStatusCode.Conflict, "BOX ĐÃ ĐƯỢC STORE");
                }
                // kiếm unstoredBox by box ID
                UnstoredBox uBox = (from uB in db.UnstoredBoxes
                                    where uB.BoxPK == box.BoxPK
                                    select uB).FirstOrDefault();
                // kiểm unstoredBox đã identified chưa
                if (uBox.IsIdentified == true)
                {
                    return Content(HttpStatusCode.Conflict, "UNSTORED BOX ĐÃ ĐƯỢC IDENTIFY");
                }
                // tạo session
                IdentifyingSession iS = identifyItemController.createdIdentifyingSession();
                // chạy lệnh tạo n-indentifiedItem
                foreach (var el in list)
                {
                    identifyItemController.createIndentifyItem(new IdentifiedItem(el.ActualQuantity, el.PackedItemPK, iS.IdentifyingSessionPK, uBox.UnstoredBoxPK));
                }
                // đổi state của unstoredBox
                identifyItemController.updateIsIdentifyUnstoreBox(uBox);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, "Identify THÀNH CÔNG");
        }
    }
}
