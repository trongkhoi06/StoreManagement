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
                accessories = (from a in db.Accessories.OrderByDescending(unit => unit.AccessoryPK)
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
                suppliers = (from s in db.Suppliers.OrderByDescending(unit => unit.SupplierPK)
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
                    orders = (from o in db.Orders.OrderByDescending(unit => unit.OrderPK)
                              where o.DateCreated >= start && o.DateCreated < end
                              select o).ToList();
                }
                // if start <= 1900 then select all
                else
                {
                    orders = (from o in db.Orders.OrderByDescending(unit => unit.OrderPK)
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
        [Route("api/ReceivingController/GetPack")]
        [HttpGet]
        public IHttpActionResult GetPack()
        {
            List<Pack> packs;
            List<Client_Pack> client_Packs = new List<Client_Pack>();
            try
            {
                packs = (from p in db.Packs.OrderByDescending(unit => unit.PackPK)
                         where p.IsOpened == true
                         select p).ToList();
                foreach (var pack in packs)
                {
                    Order order = db.Orders.Find(pack.OrderPK);
                    string supplierName = db.Suppliers.Find(order.SupplierPK).SupplierName;
                    client_Packs.Add(new Client_Pack(pack, supplierName));
                }


            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, client_Packs);
        }

        [Route("api/ReceivingController/GetPackedItemsByPackPK")]
        [HttpGet]
        public IHttpActionResult GetPackedItemsByPackPK(int PackPK)
        {
            List<PackedItem> packedItems;
            List<Client_PackedItem> client_packedItems = new List<Client_PackedItem>();
            try
            {
                packedItems = (from pI in db.PackedItems.OrderByDescending(unit => unit.PackedItemPK)
                               where pI.PackPK == PackPK
                               select pI).ToList();
                foreach (var packedItem in packedItems)
                {
                    OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);
                    Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);
                    client_packedItems.Add(new Client_PackedItem(accessory, packedItem));
                }


            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, client_packedItems);
        }

        [Route("api/ReceivingController/CreatePackBusiness")]
        [HttpPost]
        public IHttpActionResult CreatePackBusiness(int OrderPK, [FromBody] List<Client_OrderedItemPK_PackedQuantity_Comment> list)
        {
            Order order = db.Orders.Find(OrderPK);
            int noPackID;
            if (order.IsOpened)
            {
                PacksController packsController = new PacksController();
                Pack pack = null;
                try
                {
                    // create pack
                    // get last pack
                    Pack lastPack = (from p in db.Packs.OrderByDescending(unit => unit.PackPK)
                                     where p.PackID.Contains(order.OrderID)
                                     select p).FirstOrDefault();
                    if (lastPack == null)
                    {
                        noPackID = 1;
                    }
                    else
                    {
                        noPackID = Int32.Parse(lastPack.PackID.Substring(lastPack.PackID.Length - 2)) + 1;
                    }
                    // init packid
                    string PackID = (noPackID >= 10) ? (order.OrderID + "#" + noPackID) : (order.OrderID + "#" + "0" + noPackID);
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

        [Route("api/ReceivingController/EditContractNumber")]
        [HttpPut]
        public IHttpActionResult EditContractNumber(int packedItemPK, [FromBody] string contractNumber)
        {
            // kiểm trước khi chạy lệnh

            // Edit
            PackedItemsController packedItemsController = new PackedItemsController();
            try
            {
                packedItemsController.changeContract(packedItemPK, contractNumber);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, "Edit Contract Number THÀNH CÔNG");
        }

        // Identify
        [Route("api/ReceivingController/IdentifyItemBusiness")]
        [HttpPost]
        public IHttpActionResult IdentifyItemBusiness(string BoxID, string EmployeeCode, [FromBody] List<Client_PackItemPK_IdentifiedQuantity> list)
        {
            // kiểm trước khi chạy lệnh
            IdentifyItemController identifyItemController = new IdentifyItemController();
            // chạy lệnh identify
            try
            {
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

                // chạy lệnh tạo n-indentifiedItem
                foreach (var el in list)
                {
                    // querry lấy pack
                    PackedItem packedItem = db.PackedItems.Find(el.PackedItemPK);
                    Pack pack = db.Packs.Find(packedItem.PackPK);
                    // pack đang mở
                    if (pack.IsOpened)
                    {
                        // tạo session
                        IdentifyingSession iS = identifyItemController.createdIdentifyingSession(EmployeeCode);
                        identifyItemController.createIndentifyItem(new IdentifiedItem(el.IdentifiedQuantity, el.PackedItemPK, iS.IdentifyingSessionPK, uBox.UnstoredBoxPK));
                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "PACK ĐANG CLOSE, KO IDENTIFY ĐƯỢC");
                    }

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

        [Route("api/ReceivingController/EditIdentificationBusiness")]
        [HttpPut]
        public IHttpActionResult EditIdentificationBusiness(int IdentifyingSessionPK, int IdentifiedItemPK, int IdentifiedQuantity, string state)
        {
            // kiểm trước khi chạy lệnh

            // edit identification
            IdentifyingSession identifyingSession = db.IdentifyingSessions.Find(IdentifyingSessionPK);
            IdentifyItemController identifyItemController = new IdentifyItemController();
            try
            {
                // querry lấy pack
                IdentifiedItem identifiedItem = db.IdentifiedItems.Find(IdentifiedItemPK);
                PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                Pack pack = db.Packs.Find(packedItem.PackPK);
                // pack đang mở
                if (pack.IsOpened)
                {
                    // switch case xoa sua
                    switch (state)
                    {
                        case "update":
                            identifyItemController.updateIdentifiedItem(IdentifiedItemPK, IdentifiedQuantity);
                            break;
                        case "delete":
                            identifyItemController.deleteIdentifiedItem(IdentifiedItemPK);
                            break;
                        default:
                            break;
                    }

                    identifyItemController.changeExecutedDate(identifyingSession);
                }
                else
                {
                    return Content(HttpStatusCode.Conflict, "PACK ĐANG CLOSE, KO IDENTIFY ĐƯỢC");
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, "Edit Identification THÀNH CÔNG");
        }

        [Route("api/ReceivingController/DeleteIdentificationBusiness")]
        [HttpDelete]
        public IHttpActionResult DeleteIdentificationBusiness(int IdentifyingSessionPK)
        {
            // kiểm trước khi chạy lệnh

            // Delete
            IdentifyItemController identifyItemController = new IdentifyItemController();
            try
            {
                // querry lấy pack
                IdentifiedItem identifiedItem = (from iI in db.IdentifiedItems
                                                 where iI.IdentifyingSessionPK == IdentifyingSessionPK
                                                 select iI).FirstOrDefault();
                PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                Pack pack = db.Packs.Find(packedItem.PackPK);
                // pack đang mở
                if (pack.IsOpened)
                {
                    identifyItemController.deleteIdentifiedItemsOfSession(IdentifyingSessionPK);
                }
                else
                {
                    return Content(HttpStatusCode.Conflict, "PACK ĐANG CLOSE, KO IDENTIFY ĐƯỢC");
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, "Delete Identification THÀNH CÔNG");
        }

        [Route("api/ReceivingController/ArrangeIdentifiedItemsBusiness")]
        [HttpPut]
        public IHttpActionResult ArrangeIdentifiedItemsBusiness(string boxFromID, string boxToID, string employeeCode, [FromBody] List<int> listIdentifiedItemsPK)
        {
            // kiểm trước khi chạy lệnh


            // Arrange
            IdentifyItemController identifyItemController = new IdentifyItemController();
            try
            {
                // take box
                Box boxFrom = (from b in db.Boxes
                               where b.BoxID == boxFromID
                               select b).FirstOrDefault();

                Box boxTo = (from b in db.Boxes
                             where b.BoxID == boxToID
                             select b).FirstOrDefault();
                // select unstoredBox to check if box from really contain that item
                UnstoredBox uBoxFrom = (from uB in db.UnstoredBoxes
                                        where uB.BoxPK == boxFrom.BoxPK
                                        select uB).FirstOrDefault();

                // select unstoredBox to arrange
                UnstoredBox uBoxTo = (from uB in db.UnstoredBoxes
                                      where uB.BoxPK == boxTo.BoxPK
                                      select uB).FirstOrDefault();
                // Create arranging session
                ArrangingSession arrangingSession = new ArrangingSession(DateTime.Now, boxFrom.BoxPK, boxTo.BoxPK, employeeCode);
                arrangingSession = identifyItemController.createArrangingSession(arrangingSession);
                // Arrange item
                foreach (var item in listIdentifiedItemsPK)
                {
                    IdentifiedItem identifiedItem = db.IdentifiedItems.Find(item);
                    // check if box from really contain that item
                    if (identifiedItem.UnstoredBoxPK == uBoxFrom.UnstoredBoxPK)
                    {
                        identifiedItem.UnstoredBoxPK = uBoxTo.UnstoredBoxPK;
                        identifyItemController.ArrangeIndentifiedItem(identifiedItem);
                        // Map session with item
                        IdentifiedItem_ArrangingSession identifiedItem_ArrangingSession = new IdentifiedItem_ArrangingSession();
                        identifiedItem_ArrangingSession.IdentifiedItemPK = identifiedItem.IdentifiedItemPK;
                        identifiedItem_ArrangingSession.ArrangingSessionPK = arrangingSession.ArrangingSessionPK;
                        identifyItemController.MapItemWithSession(identifiedItem_ArrangingSession);
                    }
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, "Arrange THÀNH CÔNG");
        }



    }
}
