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

        [Route("api/ReceivingController/GetAccessoryBySupplierPK")]
        [HttpGet]
        public IHttpActionResult GetAccessoryBySupplierPK(int supplierPK)
        {
            List<Accessory> accessories;
            try
            {
                accessories = (from a in db.Accessories.OrderByDescending(unit => unit.AccessoryPK)
                               where a.SupplierPK == supplierPK && a.IsActive == true
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
            List<Client_OrderAngular> resutls = new List<Client_OrderAngular>();
            List<Order> orders;
            try
            {
                // make it one more day to make sure < end will be right answer
                end = end.AddDays(1);
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
                    List<Pack> packs = (from p in db.Packs
                                        where p.OrderPK == order.OrderPK
                                        select p).ToList();
                    resutls.Add(new Client_OrderAngular(order, packs.Count > 0));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, resutls);
        }

        [Route("api/ReceivingController/CreateOrderBusiness")]
        [HttpPost]
        public IHttpActionResult CreateOrderBusiness(string orderID, int supplierPK, string userID, [FromBody] List<Client_Accessory_OrderedQuantity_Comment> list)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                OrdersDAO ordersController = new OrdersDAO();
                Order order = null;

                try
                {
                    if (ordersController.CheckAccessoryAndSupplier(supplierPK, list))
                    {
                        // INPUT GUARANTEE
                        if (orderID.Length > 25)
                        {
                            return Content(HttpStatusCode.Conflict, "MÃ ĐƠN ĐẶT KHÔNG ĐƯỢC DÀI QUÁ 25 KÝ TỰ");
                        }
                        // create order
                        order = ordersController.CreateOrder(orderID, supplierPK, userID);
                        // create order items
                        OrderedItemsDAO orderedItemsController = new OrderedItemsDAO();
                        if (!orderedItemsController.isOrderedItemCreated(order.OrderPK, list))
                        {
                            throw new Exception();
                        }
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
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                OrdersDAO ordersController = new OrdersDAO();
                OrderedItemsDAO orderedItemsController = new OrderedItemsDAO();
                try
                {
                    if (ordersController.IsContainPack(orderedItems.OrderPK))
                    {
                        return Content(HttpStatusCode.Conflict, "ĐƠN HÀNG ĐÃ CHỨA PHIẾU NHẬP");
                    }
                    else
                    {
                        if (!orderedItemsController.isUpdatedOrderedItem(orderedItems, userID))
                        {
                            return Content(HttpStatusCode.Conflict, "THAY ĐỔI THẤT BẠI");
                        }
                    }

                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "THAY ĐỔI THÀNH CÔNG");
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
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                List<OrderedItem> listOrderedItem;
                OrdersDAO ordersController = new OrdersDAO();
                OrderedItemsDAO orderedItemsController = new OrderedItemsDAO();
                try
                {
                    if (ordersController.IsContainPack(orderPK))
                    {
                        return Content(HttpStatusCode.Conflict, "ĐƠN HÀNG ĐÃ CHỨA PHIẾU NHẬP");
                    }
                    else
                    {
                        Order order = ordersController.GetOrderByOrderPK(orderPK);
                        if (order.UserID == userID)
                        {
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
                        else
                        {
                            return Content(HttpStatusCode.Conflict, "PHẢI LÀ USER TẠO ORDER NÀY MỚI XÓA ĐƯỢC");
                        }
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
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                Order order = db.Orders.Find(orderPK);
                OrdersDAO ordersController = new OrdersDAO();
                try
                {
                    if (order.UserID == userID)
                    {
                        ordersController.SwiftOrderState(orderPK);
                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "PHẢI LÀ USER TẠO ĐƠN HÀNG NÀY MỚI ĐỔI ĐƯỢC");
                    }
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
        [Route("api/ReceivingController/GetIsOrderContainsPack")]
        [HttpGet]
        public IHttpActionResult GetIsOrderContainsPack(int orderPK)
        {
            try
            {
                List<Pack> packs = (from p in db.Packs.OrderByDescending(unit => unit.PackPK)
                                    where p.OrderPK == orderPK
                                    select p).ToList();
                if (packs.Count == 0) return Content(HttpStatusCode.OK, false);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, true);
        }

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
        public IHttpActionResult GetPackedItemsByPackPK(int packPK)
        {
            List<PackedItem> packedItems;
            List<Client_PackedItem> client_packedItems = new List<Client_PackedItem>();
            BoxDAO boxController = new BoxDAO();
            try
            {
                packedItems = (from pI in db.PackedItems.OrderByDescending(unit => unit.PackedItemPK)
                               where pI.PackPK == packPK
                               select pI).ToList();
                foreach (var packedItem in packedItems)
                {
                    // get packedItem
                    OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);
                    Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);
                    AccessoryType accessoryType = db.AccessoryTypes.Find(accessory.AccessoryTypePK);
                    client_packedItems.Add(new Client_PackedItem(accessory, packedItem, accessoryType.Name));
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
                IdentifyItemDAO identifyItemDAO = new IdentifyItemDAO();
                packedItems = (from pI in db.PackedItems.OrderByDescending(unit => unit.PackedItemPK)
                               where pI.PackPK == PackPK
                               select pI).ToList();
                foreach (var packedItem in packedItems)
                {
                    double actualQuantity = 0;
                    double sumIdentifiedQuantity = 0;
                    ClassifiedItem classifiedItem = (from cI in db.ClassifiedItems
                                                     where cI.PackedItemPK == packedItem.PackedItemPK
                                                     select cI).FirstOrDefault();
                    List<IdentifiedItem> identifiedItems = (from iI in db.IdentifiedItems
                                                            where iI.PackedItemPK == packedItem.PackedItemPK
                                                            select iI).ToList();
                    foreach (var identifiedItem in identifiedItems)
                    {
                        sumIdentifiedQuantity += identifiedItem.IdentifiedQuantity;
                        if (classifiedItem != null)
                        {
                            if (classifiedItem.QualityState == 2)
                            {
                                actualQuantity += identifyItemDAO.ActualQuantity(identifiedItem.IdentifiedItemPK);
                            }
                        }
                    }

                    OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);
                    Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);
                    client_PackedItemAngulars.Add(new Client_PackedItemAngular(accessory, packedItem, actualQuantity, sumIdentifiedQuantity));
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
                client_Pack_Angular.IsIdentified = false;
                // check isIdentified
                List<PackedItem> packedItems = (from pI in db.PackedItems
                                                where pI.PackPK == PackPK
                                                select pI).ToList();
                foreach (var packedItem in packedItems)
                {
                    List<IdentifiedItem> identifiedItems = (from pI in db.IdentifiedItems
                                                            where pI.PackedItemPK == packedItem.PackedItemPK
                                                            select pI).ToList();
                    if (identifiedItems.Count > 0) client_Pack_Angular.IsIdentified = true;
                }
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
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Manager"))
            {
                Order order = db.Orders.Find(orderPK);
                int noPackID;
                if (order.IsOpened)
                {
                    PacksDAO packsController = new PacksDAO();
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
                        PackedItemsDAO packedItemsController = new PackedItemsDAO();
                        if (!packedItemsController.IsPackedItemCreated(pack.PackPK, list, orderPK))
                        {
                            throw new Exception();
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
                    return Content(HttpStatusCode.OK, "TẠO PHIẾU NHẬP THÀNH CÔNG");
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
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Manager"))
            {
                PacksDAO packsController = new PacksDAO();
                PackedItemsDAO packedItemsController = new PackedItemsDAO();
                try
                {
                    if (packsController.isIdentifiedOrClassified(packedItem.PackPK))
                    {
                        return Content(HttpStatusCode.Conflict, "ĐƠN HÀNG ĐÃ CHỨA PHIẾU NHẬP");
                    }
                    else
                    {
                        Pack pack = db.Packs.Find(packedItem.PackPK);
                        if (pack.UserID == userID)
                        {
                            if (!packedItemsController.IsUpdatedPackedItem(packedItem, pack.PackPK))
                            {
                                return Content(HttpStatusCode.Conflict, "THAY ĐỔI THẤT BẠI");
                            }
                        }
                        else
                        {
                            return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
                        }
                    }

                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "THAY ĐỔI THÀNH CÔNG");
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
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Manager"))
            {
                List<PackedItem> listPackedItem;
                PacksDAO packsController = new PacksDAO();
                PackedItemsDAO packedItemsController = new PackedItemsDAO();
                try
                {
                    Pack pack = db.Packs.Find(packPK);
                    if (pack.UserID == userID)
                    {
                        //IQueryable<PackedItem> temp = packedItemsController.GetPackedItemsByPackPK(pack.PackPK);
                        listPackedItem = (from pI in db.PackedItems
                                          where pI.PackPK == pack.PackPK
                                          select pI).ToList();

                        if (packsController.isIdentifiedOrClassified(packPK))
                        {
                            return Content(HttpStatusCode.Conflict, "PACK ĐÃ ĐƯỢC GHI NHẬN HOẶC ĐÁNH GIÁ");
                        }

                        foreach (var item in listPackedItem)
                        {
                            packedItemsController.DeletePackedItem(item.PackedItemPK);
                        }

                        packsController.DeletePack(pack.PackPK);
                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
                    }
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
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Manager"))
            {
                bool isClosable = false;
                Pack pack = db.Packs.Find(packPK);
                PacksDAO packsController = new PacksDAO();
                try
                {
                    if (pack.UserID == userID)
                    {
                        List<PackedItem> packedItems = (from pI in db.PackedItems
                                                        where pI.PackPK == pack.PackPK
                                                        select pI).ToList();
                        foreach (var packedItem in packedItems)
                        {
                            ClassifiedItem classifiedItem = (from cI in db.ClassifiedItems
                                                             where cI.PackedItemPK == packedItem.PackedItemPK
                                                             select cI).FirstOrDefault();
                            if (classifiedItem != null)
                                return Content(HttpStatusCode.Conflict, "PHIẾU NHẬP ĐÃ ĐƯỢC ĐÁNH GIÁ, KHÔNG ĐƯỢC THAY ĐỔI TRẠNG THÁI");

                            List<IdentifiedItem> identifiedItems = (from iI in db.IdentifiedItems
                                                                    where iI.PackedItemPK == packedItem.PackedItemPK
                                                                    select iI).ToList();
                            if (identifiedItems.Count > 0) isClosable = true;

                            foreach (var identifiedItem in identifiedItems)
                            {
                                if (identifiedItem.IsChecked || identifiedItem.IsCounted)
                                {
                                    return Content(HttpStatusCode.Conflict, "PHIẾU NHẬP ĐÃ ĐƯỢC ĐẾM HOẶC KIỂM, KHÔNG ĐƯỢC THAY ĐỔI TRẠNG THÁI");
                                }
                            }
                        }
                        if (pack.IsOpened && !isClosable)
                        {
                            throw new Exception();
                        }
                        packsController.SwiftPackState(packPK);
                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "KHÔNG CÓ QUYỀN");
                    }
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
        public IHttpActionResult EditContractNumber(int packedItemPK, string contractNumber, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Manager"))
            {
                PackedItem packedItem = db.PackedItems.Find(packedItemPK);
                Pack pack = db.Packs.Find(packedItem.PackPK);
                if (pack.UserID == userID)
                {
                    // Edit
                    PackedItemsDAO packedItemsController = new PackedItemsDAO();
                    try
                    {
                        if (contractNumber.Length <= 25)
                        {
                            packedItemsController.ChangeContract(packedItemPK, contractNumber);
                        }
                        else
                        {
                            return Content(HttpStatusCode.Conflict, SystemMessage.NotPassPrimitiveType);
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
                // done
                return Content(HttpStatusCode.OK, "THAY ĐỔI SỐ HỢP ĐỒNG THÀNH CÔNG");
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
            List<Client_IdentifyingSession> client_IdentifyingSessions = new List<Client_IdentifyingSession>();
            BoxDAO boxController = new BoxDAO();
            PacksDAO packsController = new PacksDAO();
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
                    client_IdentifyingSessions.Add(new Client_IdentifyingSession(supplier, pack, identifyingSession));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, client_IdentifyingSessions);
        }

        [Route("api/ReceivingController/GetIdentifiedItemByIdentifyingSessionPK")]
        [HttpGet]
        public IHttpActionResult GetIdentifiedItemByIdentifyingSessionPK(int identifyingSessionPK)
        {
            List<Client_IdentifiedItem> client_IdentifiedItems = new List<Client_IdentifiedItem>();
            BoxDAO boxController = new BoxDAO();
            PacksDAO packsController = new PacksDAO();
            try
            {
                List<IdentifiedItem> identifiedItems = (from iI in db.IdentifiedItems
                                                        where iI.IdentifyingSessionPK == identifyingSessionPK
                                                        select iI).ToList();
                foreach (var identifiedItem in identifiedItems)
                {
                    PackedItem packedItem = (from pI in db.PackedItems
                                             where pI.PackedItemPK == identifiedItem.PackedItemPK
                                             select pI).FirstOrDefault();
                    // lấy phụ liệu tương ứng
                    OrderedItem orderedItem = (from oI in db.OrderedItems
                                               where oI.OrderedItemPK == packedItem.OrderedItemPK
                                               select oI).FirstOrDefault();

                    Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);
                    AccessoryType accessoryType = db.AccessoryTypes.Find(accessory.AccessoryTypePK);

                    client_IdentifiedItems.Add(new Client_IdentifiedItem(identifiedItem, accessory, accessoryType.Name));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, client_IdentifiedItems);
        }

        [Route("api/ReceivingController/GetIdentifyItemByBoxID")]
        [HttpGet]
        public IHttpActionResult GetIdentifyItemByBoxID(string boxID)
        {
            List<IdentifiedItem> identifiedItems;
            List<RestoredGroup> restoredGroups;
            List<Client_IdentifiedItemArranged> result = new List<Client_IdentifiedItemArranged>();
            BoxDAO boxDAO = new BoxDAO();
            try
            {
                Box box = boxDAO.GetBoxByBoxID(boxID);
                UnstoredBox uBox = boxDAO.GetUnstoredBoxbyBoxPK(box.BoxPK);
                if (boxDAO.IsUnstoredCase(box.BoxPK))
                {
                    identifiedItems = (from iI in db.IdentifiedItems.OrderByDescending(unit => unit.PackedItemPK)
                                       where iI.UnstoredBoxPK == uBox.UnstoredBoxPK
                                       select iI).ToList();

                    restoredGroups = db.RestoredGroups.OrderByDescending(unit => unit.RestoredGroupPK)
                                        .Where(unit => unit.UnstoredBoxPK == uBox.UnstoredBoxPK).ToList();

                    foreach (var identifiedItem in identifiedItems)
                    {
                        PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                        // lấy pack ID
                        Pack pack = db.Packs.Find(packedItem.PackPK);

                        // lấy phụ liệu tương ứng
                        OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);
                        Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);
                        AccessoryType accessoryType = db.AccessoryTypes.Find(accessory.AccessoryTypePK);

                        result.Add(new Client_IdentifiedItemArranged(identifiedItem, accessory, pack.PackID, accessoryType.Name));
                    }

                    foreach (var restoredGroup in restoredGroups)
                    {
                        RestoredItem restoredItem = db.RestoredItems.Find(restoredGroup.RestoredItemPK);
                        Restoration restoration = db.Restorations.Find(restoredItem.RestorationPK);

                        Accessory accessory = db.Accessories.Find(restoredItem.AccessoryPK);
                        AccessoryType accessoryType = db.AccessoryTypes.Find(accessory.AccessoryTypePK);

                        result.Add(new Client_IdentifiedItemArranged(restoredGroup, accessory, restoration.RestorationID, accessoryType.Name));
                    }
                }
                else
                {
                    return Content(HttpStatusCode.Conflict, "ĐƠN VỊ CHỨA ĐÃ ĐƯỢC LƯU KHO HOẶC CHƯA GHI NHẬN");
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, result);
        }

        [Route("api/ReceivingController/GetIsBoxStoredOrIdentified")]
        [HttpGet]
        public IHttpActionResult GetIsBoxStoredOrIdentified(string boxID)
        {
            BoxDAO boxDAO = new BoxDAO();
            bool result = false;
            try
            {
                Box box = boxDAO.GetBoxByBoxID(boxID);
                result = boxDAO.IsUnstoredCase(box.BoxPK);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, result);
        }

        [Route("api/ReceivingController/IdentifyItemBusiness")]
        [HttpPost]
        public IHttpActionResult IdentifyItemBusiness(string boxID, string userID, [FromBody] List<Client_PackedItemPK_IdentifiedQuantity> list)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Staff"))
            {
                IdentifyItemDAO identifyItemDAO = new IdentifyItemDAO();
                BoxDAO boxDAO = new BoxDAO();
                IdentifyingSession Iss = null;
                // chạy lệnh identify
                try
                {
                    // kiếm Box by box ID
                    Box box = boxDAO.GetBoxByBoxID(boxID);

                    // kiểm xem box đã store hay chưa
                    if (!boxDAO.IsUnstoredCase(box.BoxPK))
                    {
                        return Content(HttpStatusCode.Conflict, "ĐƠN VỊ KHÔNG HỢP LỆ!");
                    }
                    // kiếm unstoredBox by boxPK
                    UnstoredBox uBox = boxDAO.GetUnstoredBoxbyBoxPK(box.BoxPK);

                    // tạo session
                    Iss = identifyItemDAO.createdIdentifyingSession(userID);
                    // chạy lệnh tạo n-indentifiedItem
                    identifyItemDAO.createIndentifyItem(Iss, list, uBox);
                }
                catch (Exception e)
                {
                    if (Iss != null)
                    {
                        Iss = db.IdentifyingSessions.Find(Iss.IdentifyingSessionPK);
                        db.IdentifyingSessions.Remove(Iss);
                        db.SaveChanges();
                    }
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }

                return Content(HttpStatusCode.OK, "GHI NHẬN NHẬP HÀNG THÀNH CÔNG");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }

        }

        [Route("api/ReceivingController/EditIdentificationBusiness")]
        [HttpPut]
        public IHttpActionResult EditIdentificationBusiness(int IdentifyingSessionPK, string userID, [FromBody] List<Client_IdentifiedItemPK_IdentifiedQuantity> list)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Staff"))
            {
                // edit identification
                IdentifyItemDAO identifyItemDAO = new IdentifyItemDAO();
                try
                {
                    identifyItemDAO.EditIdentification(IdentifyingSessionPK, userID, list);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }

                return Content(HttpStatusCode.OK, "THAY ĐỔI PHIÊN GHI NHẬN THÀNH CÔNG");
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
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Staff"))
            {
                // edit identification
                IdentifyingSession identifyingSession = db.IdentifyingSessions.Find(IdentifyingSessionPK);
                // Delete
                IdentifyItemDAO identifyItemController = new IdentifyItemDAO();
                BoxDAO boxController = new BoxDAO();
                try
                {
                    if (identifyingSession.UserID == userID)
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
                            return Content(HttpStatusCode.Conflict, "PHIẾU NHẬP ĐANG ĐÓNG, KO GHI NHẬN ĐƯỢC");
                        }
                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
                    }
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "XÓA PHIÊN GHI NHẬN THÀNH CÔNG");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }
        }

        public class Client_GroupItemArrange
        {
            public Client_GroupItemArrange()
            {
            }

            public int ItemPK { get; set; }

            public bool IsRestored { get; set; }
        }

        [Route("api/ReceivingController/ArrangeGroupItems")]
        [HttpPut]
        public IHttpActionResult ArrangeGroupItems(string boxFromID, string boxToID, string userID, [FromBody] List<Client_GroupItemArrange> items)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Staff"))
            {
                // Arrange
                IdentifyItemDAO identifyItemDAO = new IdentifyItemDAO();
                BoxDAO boxDAO = new BoxDAO();
                ArrangingSession arrangingSession = null;
                try
                {
                    if (boxFromID == boxToID)
                    {
                        return Content(HttpStatusCode.Conflict, "KHÔNG ĐƯỢC CHỌN CÙNG MỘT ĐƠN VỊ!");
                    }

                    // take box by boxID
                    Box boxFrom = boxDAO.GetBoxByBoxID(boxFromID);
                    Box boxTo = boxDAO.GetBoxByBoxID(boxToID);

                    // check xem box đã store hay chưa
                    if (boxFrom == null || boxTo == null || !boxDAO.IsUnstoredCase(boxFrom.BoxPK) || !boxDAO.IsUnstoredCase(boxTo.BoxPK))
                    {
                        return Content(HttpStatusCode.Conflict, "ĐƠN VỊ KHÔNG HỌP LỆ!");
                    }

                    // select unstoredBox to check if box from really contain that item
                    UnstoredBox uBoxFrom = boxDAO.GetUnstoredBoxbyBoxPK(boxFrom.BoxPK);
                    if (uBoxFrom == null) return Content(HttpStatusCode.NotFound, "ĐƠN VỊ KHÔNG HỌP LỆ!");

                    // select unstoredBox to arrange
                    UnstoredBox uBoxTo = boxDAO.GetUnstoredBoxbyBoxPK(boxTo.BoxPK);
                    if (uBoxTo == null) return Content(HttpStatusCode.NotFound, "ĐƠN VỊ KHÔNG HỌP LỆ!");

                    // Create arranging session
                    arrangingSession = identifyItemDAO.createArrangingSession(uBoxFrom.UnstoredBoxPK, uBoxTo.UnstoredBoxPK, userID);

                    // Arrange item
                    identifyItemDAO.ArrangeItem(items, uBoxFrom, uBoxTo, arrangingSession);

                }
                catch (Exception e)
                {
                    if (arrangingSession != null)
                    {
                        db.ArrangingSessions.Remove(arrangingSession);
                        db.SaveChanges();
                    }
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }

                return Content(HttpStatusCode.OK, "SẮP XẾP CỤM PHỤ LIỆU THÀNH CÔNG");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }

        }

        [Route("api/ReceivingController/GetIsBoxStored")]
        [HttpGet]
        public IHttpActionResult GetIsBoxStored(string boxID)
        {
            BoxDAO boxController = new BoxDAO();
            bool result = false;
            try
            {
                Box box = boxController.GetBoxByBoxID(boxID);
                if (boxController.IsStored(box.BoxPK)) result = true;
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, result);
        }
    }
}
