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
        [Route("api/ReceivingController/GetOrderWithFilter")]
        [HttpGet]
        public IHttpActionResult GetOrderWithFilter(DateTime start, DateTime end)
        {
            List<Order> orders;
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
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, orders);
        }

        [Route("api/ReceivingController/CreateOrderBusiness")]
        [HttpPost]
        public IHttpActionResult CreateOrderBusiness(string orderID, int supplierPK, string userID, [FromBody] List<Client_Accessory_OrderedQuantity_Comment> list)
        {
            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 3)
            {
                OrdersController ordersController = new OrdersController();
                Order order = null;
                try
                {
                    // create order
                    order = ordersController.CreateOrder(orderID, supplierPK, userID);
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
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }

        }

        [Route("api/ReceivingController/UpdateOrderBusiness")]
        [HttpPut]
        public IHttpActionResult UpdateOrderBusiness([FromBody] OrderedItem orderedItems, string userID)
        {
            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 3)
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
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }

        }

        [Route("api/ReceivingController/DeleteOrderBusiness")]
        [HttpDelete]
        public IHttpActionResult DeleteOrderBusiness(int orderPK, string userID)
        {
            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 3)
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
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }
        }

        [Route("api/ReceivingController/SwiftOrderState")]
        [HttpPut]
        public IHttpActionResult SwiftOrderState(int orderPK, string userID)
        {
            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 3)
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
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }
        }

        // Pack
        [Route("api/ReceivingController/GetPackByIsOpened")]
        [HttpGet]
        public IHttpActionResult GetPackByIsOpenedMobile()
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

        [Route("api/ReceivingController/GetPackWithFilter")]
        [HttpGet]
        public IHttpActionResult GetPackWithFilterAngular(DateTime start, DateTime end)
        {
            List<Pack> packs;
            List<Client_Pack_Angular> client_Pack_Angulars = new List<Client_Pack_Angular>();
            // make it one more day to make sure < end will be right answer
            end = end.AddDays(1);
            try
            {
                // if start > 1900 then select query
                if (start.Year > 1900)
                {
                    packs = (from o in db.Packs.OrderByDescending(unit => unit.PackPK)
                             where o.DateCreated >= start && o.DateCreated < end
                             select o).ToList();
                }
                // if start <= 1900 then select all
                else
                {
                    packs = (from o in db.Packs.OrderByDescending(unit => unit.PackPK)
                             select o).ToList();
                }

                foreach (var pack in packs)
                {
                    Order order = db.Orders.Find(pack.OrderPK);
                    string supplierName = db.Suppliers.Find(order.SupplierPK).SupplierName;
                    client_Pack_Angulars.Add(new Client_Pack_Angular(pack, supplierName));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, client_Pack_Angulars);
        }


        [Route("api/ReceivingController/GetPackedItemsByPackPK")]
        [HttpGet]
        public IHttpActionResult GetPackedItemsByPackPK(int packPK, string boxID)
        {
            List<PackedItem> packedItems;
            List<Client_PackedItem> client_packedItems = new List<Client_PackedItem>();
            BoxController boxController = new BoxController();
            try
            {
                // check box coi có phải mới hay ko
                Box box = boxController.GetBoxByBoxID(boxID);
                UnstoredBox unstoredBox = boxController.GetUnstoredBoxbyBoxPK(box.BoxPK);
                if (unstoredBox.IsIdentified == false)
                {
                    packedItems = (from pI in db.PackedItems.OrderByDescending(unit => unit.PackedItemPK)
                                   where pI.PackPK == packPK
                                   select pI).ToList();
                    foreach (var packedItem in packedItems)
                    {


                        // get packedItem
                        OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);
                        Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);
                        client_packedItems.Add(new Client_PackedItem(accessory, packedItem));
                    }
                }


            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, client_packedItems);
        }

        [Route("api/ReceivingController/GetPackedItemsByPackPKAngular")]
        [HttpGet]
        public IHttpActionResult GetPackedItemsByPackPKAngular(int PackPK)
        {
            List<PackedItem> packedItems;
            List<Client_PackedItemAngular> client_PackedItemAngulars = new List<Client_PackedItemAngular>();
            try
            {
                packedItems = (from pI in db.PackedItems.OrderByDescending(unit => unit.PackedItemPK)
                               where pI.PackPK == PackPK
                               select pI).ToList();
                foreach (var packedItem in packedItems)
                {
                    OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);
                    Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);
                    client_PackedItemAngulars.Add(new Client_PackedItemAngular(accessory, packedItem));
                }


            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, client_PackedItemAngulars);
        }

        [Route("api/ReceivingController/GetPackByPackPK")]
        [HttpGet]
        public IHttpActionResult GetPackByPackPK(int PackPK)
        {
            Client_Pack_Angular client_Pack_Angular;
            try
            {
                Pack pack = db.Packs.Find(PackPK);
                Order order = db.Orders.Find(pack.OrderPK);
                string supplierName = db.Suppliers.Find(order.SupplierPK).SupplierName;
                SystemUser systemUser = db.SystemUsers.Find(pack.UserID);
                client_Pack_Angular = (new Client_Pack_Angular(pack, supplierName, systemUser));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, client_Pack_Angular);
        }

        [Route("api/ReceivingController/CreatePackBusiness")]
        [HttpPost]
        public IHttpActionResult CreatePackBusiness(int orderPK, [FromBody] List<Client_OrderedItemPK_PackedQuantity_Comment> list, string userID)
        {
            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 2)
            {
                Order order = db.Orders.Find(orderPK);
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
                        string packID = (noPackID >= 10) ? (order.OrderID + "#" + noPackID) : (order.OrderID + "#" + "0" + noPackID);
                        pack = packsController.CreatePack(packID, orderPK, userID);

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
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }

        }

        [Route("api/ReceivingController/UpdatePackBusiness")]
        [HttpPut]
        public IHttpActionResult UpdatePackBusiness([FromBody] PackedItem packedItem, string userID)
        {
            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 2)
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
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }

        }

        [Route("api/ReceivingController/DeletePackBusiness")]
        [HttpDelete]
        public IHttpActionResult DeletePackBusiness(int packPK, string userID)
        {
            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 2)
            {

                List<PackedItem> listPackedItem;
                PacksController packsController = new PacksController();
                PackedItemsController packedItemsController = new PackedItemsController();
                try
                {
                    Pack pack = db.Packs.Find(packPK);

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
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }
        }

        [Route("api/ReceivingController/SwiftPackState")]
        [HttpPut]
        public IHttpActionResult SwiftPackState(int packPK, string userID)
        {
            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 2)
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
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }
        }

        [Route("api/ReceivingController/EditContractNumber")]
        [HttpPut]
        public IHttpActionResult EditContractNumber(int packedItemPK, [FromBody] string contractNumber, string userID)
        {
            // kiểm trước khi chạy lệnh

            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 2)
            {
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
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }
        }

        // Identify
        [Route("api/ReceivingController/GetIdentifyingSessionByUserID")]
        [HttpGet]
        public IHttpActionResult GetIdentifyingSessionByUserID(string userID)
        {
            List<Client_IdentifyingSession> client_IdentifiedItems = new List<Client_IdentifyingSession>();
            BoxController boxController = new BoxController();
            PacksController packsController = new PacksController();
            try
            {
                List<IdentifyingSession> identifyingSessions = (from ss in db.IdentifyingSessions.OrderByDescending(unit => unit.IdentifyingSessionPK)
                                                                where ss.UserID == userID
                                                                select ss).ToList();
                foreach (var identifyingSession in identifyingSessions)
                {
                    IdentifiedItem identifiedItems = (from iI in db.IdentifiedItems
                                                      where iI.IdentifyingSessionPK == identifyingSession.IdentifyingSessionPK
                                                      select iI).FirstOrDefault();
                    PackedItem packedItem = (from pI in db.PackedItems
                                             where pI.PackedItemPK == identifiedItems.PackedItemPK
                                             select pI).FirstOrDefault();
                    Pack pack = (from p in db.Packs
                                 where p.PackPK == packedItem.PackPK
                                 select p).FirstOrDefault();
                    Supplier supplier = packsController.GetSupplierByPack(pack);
                    client_IdentifiedItems.Add(new Client_IdentifyingSession(supplier,pack,identifyingSession));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, client_IdentifiedItems);
        }

        [Route("api/ReceivingController/GetPackedIdentifyItemByBoxID")]
        [HttpGet]
        public IHttpActionResult GetPackedIdentifyItemByBoxID(string boxID)
        {
            List<IdentifiedItem> identifiedItems;
            List<Client_IdentifiedItem> client_IdentifiedItems = new List<Client_IdentifiedItem>();
            BoxController boxController = new BoxController();
            try
            {
                Box box = boxController.GetBoxByBoxID(boxID);
                UnstoredBox uBox = boxController.GetUnstoredBoxbyBoxPK(box.BoxPK);
                identifiedItems = (from iI in db.IdentifiedItems.OrderByDescending(unit => unit.PackedItemPK)
                                   where iI.UnstoredBoxPK == uBox.UnstoredBoxPK
                                   select iI).ToList();

                foreach (var identifiedItem in identifiedItems)
                {
                    PackedItem packedItem = (from pI in db.PackedItems
                                             where pI.PackedItemPK == identifiedItem.PackedItemPK
                                             select pI).FirstOrDefault();
                    // lấy pack ID
                    Pack pack = (from p in db.Packs
                                 where p.PackPK == packedItem.PackPK
                                 select p).FirstOrDefault();

                    // lấy phụ liệu tương ứng
                    OrderedItem orderedItem = (from oI in db.OrderedItems
                                               where oI.OrderedItemPK == packedItem.OrderedItemPK
                                               select oI).FirstOrDefault();

                    Accessory accessory = (from a in db.Accessories
                                           where a.AccessoryPK == orderedItem.AccessoryPK
                                           select a).FirstOrDefault();

                    client_IdentifiedItems.Add(new Client_IdentifiedItem(identifiedItem, accessory, pack.PackID));
                }

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, client_IdentifiedItems);
        }

        [Route("api/ReceivingController/IdentifyItemBusiness")]
        [HttpPost]
        public IHttpActionResult IdentifyItemBusiness(string boxID, string userID, [FromBody] List<Client_PackItemPK_IdentifiedQuantity> list)
        {
            // kiểm trước khi chạy lệnh

            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 4)
            {
                IdentifyItemController identifyItemController = new IdentifyItemController();
                BoxController boxController = new BoxController();
                // chạy lệnh identify
                try
                {
                    // kiếm Box by box ID
                    Box box = boxController.GetBoxByBoxID(boxID);

                    // kiểm xem box đã store hay chưa
                    if (boxController.isStored(box.BoxPK))
                    {
                        return Content(HttpStatusCode.Conflict, "BOX ĐÃ ĐƯỢC STORE");
                    }
                    // kiếm unstoredBox by boxPK
                    UnstoredBox uBox = boxController.GetUnstoredBoxbyBoxPK(box.BoxPK);
                    // kiểm unstoredBox đã identified chưa
                    if (uBox.IsIdentified == true)
                    {
                        return Content(HttpStatusCode.Conflict, "UNSTORED BOX ĐÃ ĐƯỢC IDENTIFY");
                    }
                    else
                    {
                        // tạo session
                        IdentifyingSession iS = identifyItemController.createdIdentifyingSession(userID);
                        // chạy lệnh tạo n-indentifiedItem
                        foreach (var el in list)
                        {
                            // querry lấy pack
                            PackedItem packedItem = db.PackedItems.Find(el.PackedItemPK);
                            Pack pack = db.Packs.Find(packedItem.PackPK);
                            // pack đang mở
                            if (pack.IsOpened)
                            {
                                identifyItemController.createIndentifyItem(new IdentifiedItem(el.IdentifiedQuantity, el.PackedItemPK, iS.IdentifyingSessionPK, uBox.UnstoredBoxPK));
                            }
                            else
                            {
                                return Content(HttpStatusCode.Conflict, "PACK ĐANG CLOSE, KO IDENTIFY ĐƯỢC");
                            }

                        }
                        // đổi state của unstoredBox thành true
                        boxController.updateIsIdentifyUnstoreBox(uBox, true);
                    }
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }

                return Content(HttpStatusCode.OK, "Identify THÀNH CÔNG");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }

        }

        [Route("api/ReceivingController/EditIdentificationBusiness")]
        [HttpPut]
        public IHttpActionResult EditIdentificationBusiness(int IdentifyingSessionPK, int IdentifiedItemPK, int IdentifiedQuantity, string state, string userID)
        {
            // kiểm trước khi chạy lệnh

            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 4)
            {
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
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }

        }

        [Route("api/ReceivingController/DeleteIdentificationBusiness")]
        [HttpDelete]
        public IHttpActionResult DeleteIdentificationBusiness(int IdentifyingSessionPK, string userID)
        {
            // kiểm trước khi chạy lệnh

            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 4)
            {
                // Delete
                IdentifyItemController identifyItemController = new IdentifyItemController();
                BoxController boxController = new BoxController();
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
                        boxController.updateIsIdentifyUnstoreBox(db.UnstoredBoxes.Find(identifiedItem.UnstoredBoxPK), false);
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
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }
        }

        [Route("api/ReceivingController/ArrangeIdentifiedItemsBusiness")]
        [HttpPut]
        public IHttpActionResult ArrangeIdentifiedItemsBusiness(string boxFromID, string boxToID, string userID, [FromBody] List<int> listIdentifiedItemsPK)
        {
            // kiểm trước khi chạy lệnh

            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 4)
            {
                // Arrange
                IdentifyItemController identifyItemController = new IdentifyItemController();
                BoxController boxController = new BoxController();
                try
                {
                    // take box by boxID
                    Box boxFrom = boxController.GetBoxByBoxID(boxFromID);
                    Box boxTo = boxController.GetBoxByBoxID(boxToID);

                    // check xem box đã store hay chưa
                    if (boxController.isStored(boxFrom.BoxPK) || boxController.isStored(boxTo.BoxPK))
                    {
                        return Content(HttpStatusCode.Conflict, "THÙNG ĐÃ ĐƯỢC LƯU KHO, KHÔNG THỂ CHUYỂN ĐỔI");
                    }

                    // select unstoredBox to check if box from really contain that item
                    UnstoredBox uBoxFrom = boxController.GetUnstoredBoxbyBoxPK(boxFrom.BoxPK);
                    if (uBoxFrom == null) return Content(HttpStatusCode.NotFound, "THÙNG KHÔNG TỒN TẠI");

                    // check identified
                    if (uBoxFrom.IsIdentified == true)
                    {
                        // select unstoredBox to arrange
                        UnstoredBox uBoxTo = boxController.GetUnstoredBoxbyBoxPK(boxTo.BoxPK);
                        if (uBoxTo == null) return Content(HttpStatusCode.NotFound, "THÙNG KHÔNG TỒN TẠI");

                        // Nếu box mới thì chuyển thành box cũ
                        if (uBoxTo.IsIdentified == false)
                        {
                            boxController.updateIsIdentifyUnstoreBox(uBoxTo, true);
                        }

                        // Create arranging session
                        ArrangingSession arrangingSession = new ArrangingSession(DateTime.Now, boxFrom.BoxPK, boxTo.BoxPK, userID);
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
                    else
                    {
                        return Content(HttpStatusCode.OK, "THÙNG KHÔNG HỢP LỆ");
                    }

                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }

                return Content(HttpStatusCode.OK, "Arrange THÀNH CÔNG");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }

        }
    }
}
