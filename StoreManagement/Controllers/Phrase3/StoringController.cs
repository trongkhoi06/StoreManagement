using StoreManagement.Class;
using StoreManagement.Controllers.Class;
using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;

namespace StoreManagement.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class StoringController : ApiController
    {
        private UserModel db = new UserModel();

        [Route("api/StoringController/GetIdentifyItemByBoxID")]
        [HttpGet]
        public IHttpActionResult GetIdentifyItemByBoxID(string boxID)
        {
            List<IdentifiedItem> identifiedItems;
            List<Client_IdentifiedItemStored> client_IdentifiedItems = new List<Client_IdentifiedItemStored>();
            BoxDAO boxController = new BoxDAO();
            IdentifyItemDAO identifyItemDAO = new IdentifyItemDAO();
            try
            {
                Box box = boxController.GetBoxByBoxID(boxID);
                UnstoredBox uBox = boxController.GetUnstoredBoxbyBoxPK(box.BoxPK);
                if (!boxController.IsStored(box.BoxPK))
                {
                    identifiedItems = (from iI in db.IdentifiedItems.OrderByDescending(unit => unit.PackedItemPK)
                                       where iI.UnstoredBoxPK == uBox.UnstoredBoxPK
                                       select iI).ToList();

                    foreach (var identifiedItem in identifiedItems)
                    {
                        PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                        ClassifiedItem classifiedItem = (from cI in db.ClassifiedItems
                                                         where cI.PackedItemPK == packedItem.PackedItemPK
                                                         select cI).FirstOrDefault();
                        if (classifiedItem != null)
                        {
                            PassedItem passedItem = (from pI in db.PassedItems
                                                     where pI.ClassifiedItemPK == classifiedItem.ClassifiedItemPK
                                                     select pI).FirstOrDefault();
                            if (passedItem != null)
                            {
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

                                client_IdentifiedItems.Add(new Client_IdentifiedItemStored(identifiedItem, accessory, pack,
                                    identifyItemDAO.ActualQuantity(identifiedItem.IdentifiedItemPK)));
                            }
                            else
                            {
                                return Content(HttpStatusCode.Conflict, "TỒN TẠI ÍT NHẤT MỘT CỤM PHỤ LIỆU KHÔNG ĐẠT!");
                            }
                        }
                        else
                        {
                            return Content(HttpStatusCode.Conflict, "TỒN TẠI ÍT NHẤT MỘT CỤM PHỤ LIỆU KHÔNG ĐẠT!");
                        }
                    }
                }
                else
                {
                    return Content(HttpStatusCode.Conflict, "BOX ĐÃ ĐƯỢC STORE HOẶC CHƯA IDENTIFIED !");
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, client_IdentifiedItems);
        }

        [Route("api/StoringController/GetShelfByShelfID")]
        [HttpGet]
        public IHttpActionResult GetShelfByShelfID(string shelfID)
        {
            Client_Shelf client_Shelf;
            try
            {
                Shelf shelf = (from sh in db.Shelves
                               where sh.ShelfID == shelfID && sh.ShelfID != "InvisibleShelf"
                               select sh).FirstOrDefault();
                Row row = db.Rows.Find(shelf.RowPK);
                client_Shelf = new Client_Shelf(shelf.ShelfID, row.RowID);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, client_Shelf);
        }

        [Route("api/StoringController/StoreBoxBusiness")]
        [HttpPost]
        public IHttpActionResult StoreBoxBusiness(string boxID, string shelfID, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Staff"))
            {
                BoxDAO boxDAO = new BoxDAO();
                StoringDAO storingItemDAO = new StoringDAO();
                StoredBox storedBox = null;
                StoringSession storingSession = null;
                try
                {
                    // khởi tạo
                    Box box = boxDAO.GetBoxByBoxID(boxID);
                    Shelf shelf = (from s in db.Shelves
                                   where s.ShelfID == shelfID && s.ShelfID != "InvisibleShelf"
                                   select s).FirstOrDefault();
                    if (boxDAO.IsUnstoredCase(box.BoxPK))
                    {
                        // chạy lệnh store box
                        storedBox = storingItemDAO.UpdateStoredBox(box.BoxPK, shelf.ShelfPK);
                        storingSession = storingItemDAO.CreateStoringSession(userID);
                        storingItemDAO.CreateEntriesAndUpdateItem(box, storedBox, storingSession);
                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "THÙNG KHÔNG HỢP LỆ");
                    }
                }
                catch (Exception e)
                {
                    if (storedBox != null)
                        storingItemDAO.UpdateStoredBox(storedBox.BoxPK, null);
                    if (storingSession != null)
                        storingItemDAO.DeleteStoringSession(storingSession.StoringSessionPK);
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }


                return Content(HttpStatusCode.OK, "STORE THÙNG THÀNH CÔNG");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }

        }

        public class Client_GroupItem_Store
        {
            public Client_GroupItem_Store()
            {
            }

            public Client_GroupItem_Store(int itemPK, bool isRestored, Accessory accessory, string typeName, string containerID, double quantity)
            {
                ItemPK = itemPK;
                IsRestored = isRestored;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                TypeName = typeName;
                PackID = containerID;
                ActualQuantity = quantity;
            }

            public int ItemPK { get; set; }

            public bool IsRestored { get; set; }

            public string PackID { get; set; }

            public double ActualQuantity { get; set; }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }

            public string TypeName { get; set; }
        }

        [Route("api/StoringController/GetGroupItemByBoxID")]
        [HttpGet]
        public IHttpActionResult GetGroupItemByBoxID(string boxID)
        {
            BoxDAO boxDAO = new BoxDAO();
            try
            {
                List<Client_GroupItem_Store> result = new List<Client_GroupItem_Store>();
                Box box = db.Boxes.Where(unit => unit.BoxID == boxID).FirstOrDefault();
                if (box == null || !boxDAO.IsUnstoredCase(box.BoxPK))
                {
                    return Content(HttpStatusCode.Conflict, "ĐƠN VỊ KHÔNG HỢP LỆ!");
                }
                UnstoredBox uBox = db.UnstoredBoxes.Where(unit => unit.BoxPK == box.BoxPK).FirstOrDefault();
                // list of identifiedItems
                List<IdentifiedItem> identifiedItems = db.IdentifiedItems.Where(unit => unit.UnstoredBoxPK == uBox.UnstoredBoxPK).ToList();
                foreach (var item in identifiedItems)
                {
                    PackedItem packedItem = db.PackedItems.Find(item.PackedItemPK);
                    OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);
                    Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);
                    AccessoryType accessoryType = db.AccessoryTypes.Find(accessory.AccessoryTypePK);
                    Pack pack = db.Packs.Find(packedItem.PackPK);
                    result.Add(new Client_GroupItem_Store(item.IdentifiedItemPK, false, accessory, accessoryType.Name,
                        pack.PackID, new IdentifyItemDAO().ActualQuantity(item.IdentifiedItemPK)));
                }

                // list of restoredGroups
                List<RestoredGroup> restoredGroups = db.RestoredGroups.Where(unit => unit.UnstoredBoxPK == uBox.UnstoredBoxPK).ToList();
                foreach (var item in restoredGroups)
                {
                    RestoredItem restoredItem = db.RestoredItems.Find(item.RestoredItemPK);
                    Accessory accessory = db.Accessories.Find(restoredItem.AccessoryPK);
                    AccessoryType accessoryType = db.AccessoryTypes.Find(accessory.AccessoryTypePK);
                    Restoration restoration = db.Restorations.Find(restoredItem.RestorationPK);
                    result.Add(new Client_GroupItem_Store(item.RestoredGroupPK, true, accessory, accessoryType.Name,
                        restoration.RestorationID, restoredItem.RestoredQuantity));
                }

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        public class Client_Shelf_Row_Store
        {
            public Client_Shelf_Row_Store()
            {
            }

            public string ShelfID { get; set; }

            public string RowID { get; set; }
        }

        [Route("api/StoringController/GetShelfAndRowByBoxID")]
        [HttpGet]
        public IHttpActionResult GetShelfAndRowByBoxID(string boxID)
        {
            BoxDAO boxDAO = new BoxDAO();
            try
            {
                Client_Shelf_Row_Store result;

                Box box = db.Boxes.Where(unit => unit.BoxID == boxID).FirstOrDefault();
                if (box == null || !boxDAO.IsStoredCase(box.BoxPK))
                {
                    return Content(HttpStatusCode.Conflict, "ĐƠN VỊ KHÔNG HỢP LỆ!");
                }
                StoredBox sBox = db.StoredBoxes.Where(unit => unit.BoxPK == box.BoxPK).FirstOrDefault();
                Shelf shelf = db.Shelves.Find(sBox.ShelfPK);
                Row row = db.Rows.Find(shelf.RowPK);

                result = new Client_Shelf_Row_Store()
                {
                    ShelfID = shelf.ShelfID,
                    RowID = row.RowID
                };

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }
        public class Client_GroupItem_Store2
        {
            public Client_GroupItem_Store2()
            {
            }

            public Client_GroupItem_Store2(int itemPK, bool isRestored)
            {
                ItemPK = itemPK;
                IsRestored = isRestored;
            }

            public int ItemPK { get; set; }

            public bool IsRestored { get; set; }
        }

        [Route("api/StoringController/StoreItemBusiness")]
        [HttpPost]
        public IHttpActionResult StoreItemBusiness(string boxID, string userID, List<Client_GroupItem_Store> input)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Staff"))
            {
                BoxDAO boxDAO = new BoxDAO();
                StoringDAO storingItemDAO = new StoringDAO();
                StoringSession storingSession = null;
                try
                {
                    // khởi tạo
                    Box box = boxDAO.GetBoxByBoxID(boxID);
                    if (boxDAO.IsStoredCase(box.BoxPK))
                    {
                        storingSession = storingItemDAO.CreateStoringSession(userID);
                        storingItemDAO.CreateEntriesAndUpdateItem2(box, storingSession, input);
                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "ĐƠN VỊ KHÔNG HỢP LỆ");
                    }
                }
                catch (Exception e)
                {
                    if (storingSession != null)
                        storingItemDAO.DeleteStoringSession(storingSession.StoringSessionPK);
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }


                return Content(HttpStatusCode.OK, "STORE THÙNG THÀNH CÔNG");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }

        }

        [Route("api/StoringController/GetInBoxItemByBoxID")]
        [HttpGet]
        public IHttpActionResult GetInBoxItemByBoxID(string boxID)
        {
            Client_InBoxItems_Shelf<Client_Shelf> result;
            Client_Shelf client_Shelf;
            List<Client_InBoxItem> client_InBoxItems = new List<Client_InBoxItem>();
            BoxDAO boxDAO = new BoxDAO();
            StoringDAO storingDAO = new StoringDAO();
            try
            {
                // Get client Shelf
                Box box = boxDAO.GetBoxByBoxID(boxID);
                StoredBox sBox = boxDAO.GetStoredBoxbyBoxPK(box.BoxPK);
                if (sBox == null)
                {
                    return Content(HttpStatusCode.Conflict, "BOX KHÔNG HỢP LỆ!");
                }
                if (sBox.ShelfPK != null)
                {
                    Shelf shelf = db.Shelves.Find(sBox.ShelfPK);
                    Row row = db.Rows.Find(shelf.RowPK);
                    client_Shelf = new Client_Shelf(shelf.ShelfID, row.RowID);
                }
                else
                {
                    client_Shelf = null;
                }
                // lấy tất cả entry theo box
                List<Entry> entries = (from e in db.Entries
                                       where e.StoredBoxPK == sBox.StoredBoxPK
                                       select e).ToList();

                // Hiện các item với key là 2 field itemPK và isRestored
                HashSet<Tuple<int, bool>> keys = new HashSet<Tuple<int, bool>>();
                foreach (var entry in entries)
                {
                    keys.Add(new Tuple<int, bool>(entry.ItemPK, entry.IsRestored));
                }
                foreach (var key in keys)
                {
                    // chạy các key để kiếm thông tin
                    PassedItem passedItem;
                    RestoredItem restoredItem;
                    if (key.Item2)
                    {
                        restoredItem = db.RestoredItems.Find(key.Item1);
                        Restoration restoration = db.Restorations.Find(restoredItem.RestorationPK);
                        Accessory accessory = db.Accessories.Find(restoredItem.AccessoryPK);
                        client_InBoxItems.Add(new Client_InBoxItem(accessory, restoration.RestorationID, storingDAO.AvailableQuantity(sBox, key.Item1, key.Item2), restoredItem.RestoredItemPK, true));
                    }
                    else
                    {
                        passedItem = db.PassedItems.Find(key.Item1);
                        ClassifiedItem classifiedItem = db.ClassifiedItems.Find(passedItem.ClassifiedItemPK);
                        PackedItem packedItem = db.PackedItems.Find(classifiedItem.PackedItemPK);
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
                        client_InBoxItems.Add(new Client_InBoxItem(accessory, pack.PackID, storingDAO.AvailableQuantity(sBox, key.Item1, key.Item2), passedItem.PassedItemPK, false));
                    }
                }
                result = new Client_InBoxItems_Shelf<Client_Shelf>(client_Shelf, client_InBoxItems);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        [Route("api/StoringController/TransferStoredItems")]
        [HttpPost]
        public IHttpActionResult TransferStoredItems(string boxFromID, string boxToID, string userID, [FromBody] List<Client_ItemPK_TransferQuantity_IsRestored> list)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Staff"))
            {
                BoxDAO boxDAO = new BoxDAO();
                StoringDAO storingDAO = new StoringDAO();
                TransferringSession transferringSession = null;
                try
                {
                    if (boxFromID != boxToID)
                    {
                        // lấy inBoxQuantity
                        Box boxFrom = boxDAO.GetBoxByBoxID(boxFromID);
                        StoredBox sBoxFrom = boxDAO.GetStoredBoxbyBoxPK(boxFrom.BoxPK);
                        Box boxTo = boxDAO.GetBoxByBoxID(boxToID);
                        StoredBox sBoxTo = boxDAO.GetStoredBoxbyBoxPK(boxTo.BoxPK);
                        // kiểm box
                        if (!boxDAO.IsStoredCase(boxFrom.BoxPK) || !boxDAO.IsStoredCase(boxTo.BoxPK))
                        {
                            return Content(HttpStatusCode.Conflict, "THÙNG KHÔNG HỢP LỆ");
                        }
                        // kiểm số lượng trong box
                        foreach (var item in list)
                        {
                            if (item.TransferQuantity > storingDAO.AvailableQuantity(sBoxFrom, item.ItemPK, item.IsRestored)
                                && !PrimitiveType.isValidQuantity(item.TransferQuantity))
                            {
                                return Content(HttpStatusCode.Conflict, SystemMessage.NotPassPrimitiveType);
                            }
                        }
                        // tạo phiên chuyển
                        transferringSession = storingDAO.CreateTransferingSession(boxFromID, boxToID, userID);
                        // tạo entry
                        storingDAO.CreateInAndOutEntry(list, sBoxFrom, sBoxTo, transferringSession);
                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "THÙNG KHÔNG HỢP LỆ");
                    }
                    return Content(HttpStatusCode.OK, "CHUYỂN HÀNG TRONG THÙNG THÀNH CÔNG");
                }
                catch (Exception e)
                {
                    if (transferringSession != null)
                    {
                        storingDAO.DeleteTransferingSession(transferringSession.TransferingSessionPK);
                    }
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }
        }

        [Route("api/StoringController/MoveStoredBox")]
        [HttpPost]
        public IHttpActionResult MoveStoredBox(string boxID, string shelfID, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Staff"))
            {
                BoxDAO boxDAO = new BoxDAO();
                StoringDAO storingDAO = new StoringDAO();
                MovingSession movingSession = null;
                try
                {
                    Box box = boxDAO.GetBoxByBoxID(boxID);
                    StoredBox storedBox = boxDAO.GetStoredBoxbyBoxPK(box.BoxPK);
                    if (storedBox != null)
                    {
                        Shelf shelf = boxDAO.GetShelfByShelfID(shelfID);
                        if (storedBox.ShelfPK != shelf.ShelfPK)
                        {
                            movingSession = storingDAO.CreateMovingSession(storedBox, shelf, userID);
                            storingDAO.UpdateStoredBoxShelfPK(storedBox.StoredBoxPK, shelf.ShelfPK);
                        }
                        else
                        {
                            return Content(HttpStatusCode.Conflict, "KỆ KHÔNG HỢP LỆ!");
                        }
                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "THÙNG KHÔNG HỢP LỆ!");
                    }
                }
                catch (Exception e)
                {
                    if (movingSession != null)
                    {
                        storingDAO.DeleteMovingSession(movingSession.MovingSessionPK);
                    }
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "CHUYỂN THÙNG THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/StoringController/AdjustInventory")]
        [HttpPost]
        public IHttpActionResult AdjustInventory(string boxID, int itemPK, double adjustedQuantity, bool isRestored, string userID, string comment)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Staff"))
            {
                BoxDAO boxDAO = new BoxDAO();
                StoringDAO storingDAO = new StoringDAO();
                AdjustingSession adjustingSession = null;
                try
                {
                    if (!PrimitiveType.isValidComment(comment))
                    {
                        return Content(HttpStatusCode.Conflict, SystemMessage.NotPassPrimitiveType);
                    }
                    Box box = boxDAO.GetBoxByBoxID(boxID);
                    StoredBox sBox = boxDAO.GetStoredBoxbyBoxPK(box.BoxPK);
                    if (sBox != null)
                    {
                        AvailableItem availableItem = new AvailableItem(sBox.StoredBoxPK, itemPK, isRestored);
                        if (availableItem.IsPending)
                        {
                            return Content(HttpStatusCode.Conflict, "ĐƠN VỊ ĐANG CHỜ XỬ LÝ ĐIỀU CHỈNH");
                        }
                        adjustingSession = storingDAO.CreateAdjustingSession(comment, false, userID);
                        if (adjustedQuantity > availableItem.AvailableQuantity && PrimitiveType.isValidQuantity(adjustedQuantity))
                        {
                            storingDAO.CreateAdjustEntry(sBox, itemPK, adjustedQuantity - storingDAO.AvailableQuantity(sBox, itemPK, isRestored), isRestored, false, adjustingSession);
                        }
                        else if (adjustedQuantity < storingDAO.AvailableQuantity(sBox, itemPK, isRestored))
                        {
                            storingDAO.CreateAdjustEntry(sBox, itemPK, storingDAO.AvailableQuantity(sBox, itemPK, isRestored) - adjustedQuantity, isRestored, true, adjustingSession);
                        }
                        else
                        {
                            throw new Exception(SystemMessage.NotPassPrimitiveType);
                        }
                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "THÙNG KHÔNG HỢP LỆ");
                    }
                }
                catch (Exception e)
                {
                    if (adjustingSession != null)
                    {
                        storingDAO.DeleteAdjustingSession(adjustingSession.AdjustingSessionPK);
                    }
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "THAY ĐỔI KHO THÀNH CÔNG");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }
        }

        [Route("api/StoringController/VerifyAdjusting")]
        [HttpPost]
        public IHttpActionResult VerifyAdjusting(int adjustingSessionPK, string userID, bool isApproved)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Manager"))
            {
                StoringDAO storingDAO = new StoringDAO();
                Verification verification = null;
                AdjustingSession adjustingSession = null;
                try
                {
                    adjustingSession = db.AdjustingSessions.Find(adjustingSessionPK);
                    if (adjustingSession != null && adjustingSession.IsVerified == false)
                    {
                        storingDAO.UpdateAdjustingSession(adjustingSession.AdjustingSessionPK, true);
                        verification = storingDAO.CreateVerification(adjustingSession.AdjustingSessionPK, userID, isApproved, false);
                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "PHIÊN THAY SỐ LƯỢNG PHỤ LIỆU TỒN KHO KHÔNG HỢP LỆ!");
                    }
                }
                catch (Exception e)
                {
                    if (adjustingSession != null)
                        storingDAO.UpdateAdjustingSession(adjustingSession.AdjustingSessionPK, false);
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "THAY ĐỔI SỐ LƯỢNG PHỤ LIỆU THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/StoringController/DiscardInventory")]
        [HttpPost]
        public IHttpActionResult DiscardInventory(string boxID, int itemPK, double discardedQuantity, bool isRestored, string userID, string comment)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Staff"))
            {
                BoxDAO boxDAO = new BoxDAO();
                StoringDAO storingDAO = new StoringDAO();
                DiscardingSession discardingSession = null;
                try
                {
                    if (!PrimitiveType.isValidComment(comment))
                    {
                        return Content(HttpStatusCode.Conflict, SystemMessage.NotPassPrimitiveType);
                    }
                    Box box = boxDAO.GetBoxByBoxID(boxID);
                    StoredBox sBox = boxDAO.GetStoredBoxbyBoxPK(box.BoxPK);
                    if (sBox != null)
                    {
                        AvailableItem availableItem = new AvailableItem(sBox.StoredBoxPK, itemPK, isRestored);
                        if (availableItem.IsPending)
                        {
                            return Content(HttpStatusCode.Conflict, "ĐƠN VỊ ĐANG CHỜ XỬ LÝ ĐIỀU CHỈNH");
                        }
                        discardingSession = storingDAO.CreateDiscardingSession(comment, false, userID);
                        if (discardedQuantity <= availableItem.AvailableQuantity && discardedQuantity > 0)
                        {
                            storingDAO.CreateDiscardEntry(sBox, itemPK, discardedQuantity, isRestored, discardingSession);
                        }
                        else
                        {
                            throw new Exception("SỐ LƯỢNG KHÔNG HỢP LỆ");
                        }
                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "THÙNG KHÔNG HỢP LỆ");
                    }
                }
                catch (Exception e)
                {
                    if (discardingSession != null)
                    {
                        storingDAO.DeleteDiscardingSession(discardingSession.DiscardingSessionPK);
                    }
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "THAY ĐỔI KHO THÀNH CÔNG");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }
        }

        [Route("api/StoringController/VerifyDiscarding")]
        [HttpPost]
        public IHttpActionResult VerifyDiscarding(int discardingSessionPK, string userID, bool isApproved)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Manager"))
            {
                StoringDAO storingDAO = new StoringDAO();
                Verification verification = null;
                DiscardingSession discardingSession = null;
                try
                {
                    discardingSession = db.DiscardingSessions.Find(discardingSessionPK);
                    if (discardingSession != null && discardingSession.IsVerified == false)
                    {
                        storingDAO.UpdateDiscardingSession(discardingSession.DiscardingSessionPK, true);
                        verification = storingDAO.CreateVerification(discardingSession.DiscardingSessionPK, userID, isApproved, true);
                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "DiscardingSession SAI!");
                    }
                }
                catch (Exception e)
                {
                    if (discardingSession != null)
                        storingDAO.UpdateDiscardingSession(discardingSession.DiscardingSessionPK, false);
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "VERIFY ADJUSTING THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

    }
}
