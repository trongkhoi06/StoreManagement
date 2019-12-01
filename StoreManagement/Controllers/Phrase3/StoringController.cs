using StoreManagement.Class;
using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace StoreManagement.Controllers
{
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
                if (!boxController.IsStored(box.BoxPK) && uBox.IsIdentified == true)
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
                               where sh.ShelfID == shelfID
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
            // kiểm trước khi chạy lệnh
            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 4)
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
                                   where s.ShelfID == shelfID
                                   select s).FirstOrDefault();
                    if (boxDAO.GetUnstoredBoxbyBoxPK(box.BoxPK).IsIdentified && !boxDAO.IsStored(box.BoxPK))
                    {
                        // chạy lệnh store box
                        storedBox = storingItemDAO.CreateStoredBox(box.BoxPK, shelf.ShelfPK);
                        storingSession = storingItemDAO.CreateStoringSession(box.BoxPK, userID);
                        storingItemDAO.CreateEntriesUpdatePassedItem(box, storedBox, storingSession);
                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "THÙNG KHÔNG HỢP LỆ");
                    }
                }
                catch (Exception e)
                {
                    if (storedBox != null)
                        storingItemDAO.DeleteStoredBox(storedBox.StoredBoxPK);
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
            Client_InBoxItems_Shelf result;
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
                Shelf shelf = db.Shelves.Find(sBox.ShelfPK);
                Row row = db.Rows.Find(shelf.RowPK);
                client_Shelf = new Client_Shelf(shelf.ShelfID, row.RowID);

                // Get list inBoxItem
                List<Entry> entries = (from e in db.Entries
                                       where e.StoredBoxPK == sBox.StoredBoxPK
                                       select e).ToList();
                HashSet<int> listPK = new HashSet<int>();
                foreach (var entry in entries)
                {
                    listPK.Add(entry.ItemPK);
                }
                foreach (var itemPK in listPK)
                {
                    List<Entry> tempEntries = new List<Entry>();
                    foreach (var entry in entries)
                    {
                        if (entry.ItemPK == itemPK) tempEntries.Add(entry);
                    }
                    if (tempEntries.Count > 0)
                    {
                        Entry entry = tempEntries[0];
                        PassedItem passedItem;
                        RestoredItem restoredItem;
                        if (entry.IsRestored)
                        {
                            restoredItem = db.RestoredItems.Find(entry.ItemPK);
                            Restoration restoration = db.Restorations.Find(restoredItem.RestorationPK);
                            Accessory accessory = db.Accessories.Find(restoredItem.AccessoryPK);
                            client_InBoxItems.Add(new Client_InBoxItem(accessory, restoration.RestorationID, storingDAO.InBoxQuantity(tempEntries), restoredItem.RestoredItemPK, true));
                        }
                        else
                        {
                            passedItem = db.PassedItems.Find(entry.ItemPK);
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
                            client_InBoxItems.Add(new Client_InBoxItem(accessory, pack.PackID, storingDAO.InBoxQuantity(tempEntries), passedItem.PassedItemPK, false));
                        }
                    }
                }
                result = new Client_InBoxItems_Shelf(client_Shelf, client_InBoxItems);
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
            // kiểm trước khi chạy lệnh
            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 4)
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
                        if (sBoxFrom == null)
                        {
                            return Content(HttpStatusCode.Conflict, "BOX KHÔNG HỢP LỆ!");
                        }
                        List<Entry> entries = (from e in db.Entries
                                               where e.StoredBoxPK == sBoxFrom.StoredBoxPK
                                               select e).ToList();
                        foreach (var item in list)
                        {
                            List<Entry> tempEntries = new List<Entry>();
                            foreach (var entry in entries)
                            {
                                if (entry.ItemPK == item.ItemPK) tempEntries.Add(entry);
                            }
                            if (item.TransferQuantity > storingDAO.InBoxQuantity(tempEntries))
                            {
                                return Content(HttpStatusCode.Conflict, "SỐ LƯỢNG KHÔNG HỢP LỆ!");
                            }
                        }
                        transferringSession = storingDAO.CreateTransferingSession(boxFromID, boxToID, userID);
                        storingDAO.CreateInAndOutEntry(list, sBoxFrom, sBoxTo, transferringSession);
                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "BOX KHÔNG HỢP LỆ!");
                    }
                    return Content(HttpStatusCode.OK, "TRANSFER THÀNH CÔNG!");
                }
                catch (Exception e)
                {
                    if (transferringSession != null)
                        storingDAO.DeleteTransferingSession(transferringSession.TransferingSessionPK);
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
            // kiểm trước khi chạy lệnh
            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 4)
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
            // kiểm trước khi chạy lệnh
            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 4)
            {
                BoxDAO boxDAO = new BoxDAO();
                StoringDAO storingDAO = new StoringDAO();
                AdjustingSession adjustingSession = null;
                try
                {
                    Box box = boxDAO.GetBoxByBoxID(boxID);
                    StoredBox sBox = boxDAO.GetStoredBoxbyBoxPK(box.BoxPK);
                    if (sBox != null)
                    {
                        List<Entry> entries = (from e in db.Entries
                                               where e.StoredBoxPK == sBox.StoredBoxPK && e.ItemPK == itemPK && e.IsRestored == isRestored
                                               select e).ToList();
                        adjustingSession = storingDAO.CreateAdjustingSession(comment, false, userID);
                        if (adjustedQuantity > storingDAO.InBoxQuantity(entries))
                        {
                            storingDAO.CreateAdjustEntry(sBox, itemPK, adjustedQuantity, isRestored, false, adjustingSession);
                        }
                        if (adjustedQuantity < storingDAO.InBoxQuantity(entries))
                        {
                            storingDAO.CreateAdjustEntry(sBox, itemPK, adjustedQuantity, isRestored, true, adjustingSession);
                        }
                        else
                        {
                            return Content(HttpStatusCode.Conflict, "SỐ LƯỢNG KHÔNG HỢP LỆ!");
                        }
                    }
                    else
                    {
                        if (adjustingSession != null)
                        {
                            storingDAO.DeleteAdjustingSession(adjustingSession.AdjustingSessionPK);
                        }
                        return Content(HttpStatusCode.Conflict, "THÙNG KHÔNG HỢP LỆ!");
                    }
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "THAY ĐỔI KHO THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/StoringController/VerifyAdjusting")]
        [HttpPost]
        public IHttpActionResult VerifyAdjusting(int adjustingSessionPK, string userID, bool isApproved)
        {
            // kiểm trước khi chạy lệnh
            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 2)
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
                        return Content(HttpStatusCode.Conflict, "AdjustingSession SAI!");
                    }
                }
                catch (Exception e)
                {
                    if (adjustingSession != null)
                        storingDAO.UpdateAdjustingSession(adjustingSession.AdjustingSessionPK, false);
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "VERIFY ADJUSTING THÀNH CÔNG!");
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
            // kiểm trước khi chạy lệnh
            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 4)
            {
                BoxDAO boxDAO = new BoxDAO();
                StoringDAO storingDAO = new StoringDAO();
                DiscardingSession discardingSession = null;
                try
                {
                    Box box = boxDAO.GetBoxByBoxID(boxID);
                    StoredBox sBox = boxDAO.GetStoredBoxbyBoxPK(box.BoxPK);
                    if (sBox != null)
                    {
                        List<Entry> entries = (from e in db.Entries
                                               where e.StoredBoxPK == sBox.StoredBoxPK && e.ItemPK == itemPK && e.IsRestored == isRestored
                                               select e).ToList();
                        discardingSession = storingDAO.CreateDiscardingSession(comment, false, userID);
                        if (discardedQuantity > storingDAO.InBoxQuantity(entries))
                        {
                            storingDAO.CreateDiscardEntry(sBox, itemPK, discardedQuantity, isRestored, false, discardingSession);
                        }
                        if (discardedQuantity < storingDAO.InBoxQuantity(entries))
                        {
                            storingDAO.CreateDiscardEntry(sBox, itemPK, discardedQuantity, isRestored, true, discardingSession);
                        }
                        else
                        {
                            return Content(HttpStatusCode.Conflict, "SỐ LƯỢNG KHÔNG HỢP LỆ!");
                        }
                    }
                    else
                    {
                        if (discardingSession != null)
                        {
                            storingDAO.DeleteDiscardingSession(discardingSession.DiscardingSessionPK);
                        }
                        return Content(HttpStatusCode.Conflict, "THÙNG KHÔNG HỢP LỆ!");
                    }
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "THAY ĐỔI KHO THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/StoringController/VerifyDiscarding")]
        [HttpPost]
        public IHttpActionResult VerifyDiscarding(int discardingSessionPK, string userID, bool isApproved)
        {
            // kiểm trước khi chạy lệnh
            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 2)
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
