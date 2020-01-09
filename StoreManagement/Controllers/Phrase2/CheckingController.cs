using StoreManagement.Class;
using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace StoreManagement.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CheckingController : ApiController
    {
        private UserModel db = new UserModel();

        // Count

        [Route("api/CheckingController/GetIdentifyItemByBoxIDCounted")]
        [HttpGet]
        public IHttpActionResult GetIdentifyItemByBoxIDCounted(string boxID)
        {
            List<IdentifiedItem> identifiedItems;
            List<Client_IdentifiedItemCounted> client_IdentifiedItems = new List<Client_IdentifiedItemCounted>();
            BoxDAO boxController = new BoxDAO();
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

                    client_IdentifiedItems.Add(new Client_IdentifiedItemCounted(identifiedItem, accessory, pack, packedItem));
                }

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, client_IdentifiedItems);
        }

        [Route("api/CheckingController/GetCountingSessionByUserID")]
        [HttpGet]
        public IHttpActionResult GetCountingSessionByUserID(string userID)
        {
            List<Client_CountingSessionDetail> client_CountingSessions = new List<Client_CountingSessionDetail>();

            try
            {
                List<CountingSession> countingSessions = (from ss in db.CountingSessions.OrderByDescending(unit => unit.CountingSessionPK)
                                                          where ss.UserID == userID
                                                          select ss).ToList();
                foreach (var countingSession in countingSessions)
                {
                    IdentifiedItem identifiedItems = (from iI in db.IdentifiedItems
                                                      where iI.IdentifiedItemPK == countingSession.IdentifiedItemPK
                                                      select iI).FirstOrDefault();

                    // lấy box tương ứng
                    UnstoredBox uBox = db.UnstoredBoxes.Find(identifiedItems.UnstoredBoxPK);
                    Box box = db.Boxes.Find(uBox.BoxPK);

                    // lấy pack tương ứng
                    PackedItem packedItem = db.PackedItems.Find(identifiedItems.PackedItemPK);
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

                    client_CountingSessions.Add(new Client_CountingSessionDetail(accessory, pack, countingSession, identifiedItems, box, packedItem));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, client_CountingSessions);
        }

        [Route("api/CheckingController/CountItemBusiness")]
        [HttpPost]
        public IHttpActionResult CountItemBusiness(int identifiedItemPK, double countedQuantity, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Staff"))
            {
                // khởi tạo
                CountingItemDAO countingItemController = new CountingItemDAO();
                PackedItemsDAO packedItemsController = new PackedItemsDAO();
                CountingSession countingSession = null;
                // chạy lệnh counting
                try
                {
                    IdentifiedItem identifiedItem = db.IdentifiedItems.Find(identifiedItemPK);
                    // kiểm state pack is closed
                    Pack pack = (from p in db.Packs
                                 where p.PackPK ==
                                            (from pI in db.PackedItems
                                             where pI.PackedItemPK == identifiedItem.PackedItemPK
                                             select pI).FirstOrDefault().PackPK
                                 select p).FirstOrDefault();
                    if (!pack.IsOpened)
                    {
                        // kiểm packeditem ứng với identified item đã classified chưa
                        if (!packedItemsController.IsPackedItemClassified(identifiedItem))
                        {
                            // kiểm identified item đã được đếm hay chưa
                            if (!identifiedItem.IsCounted)
                            {
                                // tạo session update và iscounted
                                countingSession = countingItemController.createCountingSession(new CountingSession(identifiedItemPK, countedQuantity, userID));
                                countingItemController.updateIsCountedOfIdentifiedItem(identifiedItemPK, true);
                            }
                            else
                            {
                                return Content(HttpStatusCode.Conflict, "CỤM PHỤ LIỆU ĐÃ ĐẾM RỒI, KHÔNG THỂ ĐẾM LẠI");
                            }
                        }
                        else
                        {
                            return Content(HttpStatusCode.Conflict, "CỤM PHỤ LIỆU ĐÃ ĐÁNH GIÁ");
                        }

                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "PHIẾU NHẬP CHƯA ĐÓNG");
                    }
                }
                catch (Exception e)
                {
                    if (countingSession != null)
                    {
                        countingItemController.deleteCountingSession(countingSession.CountingSessionPK);
                    }
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }

                return Content(HttpStatusCode.OK, "ĐẾM CỤM PHỤ LIỆU THÀNH CÔNG");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }

        }

        [Route("api/CheckingController/EditCountItemBusiness")]
        [HttpPut]
        public IHttpActionResult EditCountItemBusiness(int countingSessionPK, double countedQuantity, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Staff"))
            {
                // khởi tạo
                CountingItemDAO countingItemController = new CountingItemDAO();
                try
                {
                    CountingSession countingSession = db.CountingSessions.Find(countingSessionPK);
                    if (countingSession.UserID == userID)
                    {
                        IdentifiedItem identifiedItem = db.IdentifiedItems.Find(countingSession.IdentifiedItemPK);
                        PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                        if (!packedItem.IsClassified)
                        {
                            if (PrimitiveType.isValidQuantity(countedQuantity))
                            {
                                // chạy lệnh edit counting
                                countingItemController.updateCountingSession(countingSessionPK, countedQuantity);
                            }
                            else
                            {
                                return Content(HttpStatusCode.Conflict, SystemMessage.NotPassPrimitiveType);
                            }
                        }
                        else
                        {
                            return Content(HttpStatusCode.Conflict, "PHỤ LIỆU PHIẾU NHẬP ĐÃ ĐƯỢC ĐÁNH GIÁ");
                        }
                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
                    }
                    return Content(HttpStatusCode.OK, "SỬA PHIÊN ĐẾM THÀNH CÔNG");
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

        [Route("api/CheckingController/DeleteCountItemBusiness")]
        [HttpDelete]
        public IHttpActionResult DeleteCountItemBusiness(int countingSessionPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Staff"))
            {
                // khởi tạo
                CountingItemDAO countingItemController = new CountingItemDAO();
                // chạy lệnh delete counting
                try
                {
                    CountingSession countingSession = db.CountingSessions.Find(countingSessionPK);
                    if (countingSession.UserID == userID)
                    {
                        IdentifiedItem identifiedItem = db.IdentifiedItems.Find(countingSession.IdentifiedItemPK);
                        PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                        if (!packedItem.IsClassified)
                        {
                            // chạy lệnh edit counting
                            countingItemController.updateIsCountedOfIdentifiedItem(countingSession.IdentifiedItemPK, false);
                            countingItemController.deleteCountingSession(countingSessionPK);
                        }
                        else
                        {
                            return Content(HttpStatusCode.Conflict, "PHỤ LIỆU PHIẾU NHẬP ĐÃ ĐƯỢC ĐÁNH GIÁ");
                        }
                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
                    }
                    return Content(HttpStatusCode.OK, "DELETE COUNTING THÀNH CÔNG");

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

        // Check
        [Route("api/CheckingController/GetIdentifyItemByBoxIDChecked")]
        [HttpGet]
        public IHttpActionResult GetIdentifyItemByBoxIDChecked(string boxID)
        {
            List<IdentifiedItem> identifiedItems;
            List<Client_IdentifiedItemChecked> client_IdentifiedItems = new List<Client_IdentifiedItemChecked>();
            BoxDAO boxController = new BoxDAO();
            try
            {
                Box box = boxController.GetBoxByBoxID(boxID);
                UnstoredBox uBox = boxController.GetUnstoredBoxbyBoxPK(box.BoxPK);
                if (!(boxController.IsStored(box.BoxPK) || uBox.IsIdentified == false))
                {
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

                        client_IdentifiedItems.Add(new Client_IdentifiedItemChecked(identifiedItem, accessory, pack, packedItem));
                    }
                }
                else
                {
                    return Content(HttpStatusCode.Conflict, "THÙNG ĐÃ ĐƯỢC LƯU KHO HOẶC CHƯA GHI NHẬN");
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, client_IdentifiedItems);
        }

        [Route("api/CheckingController/GetIdentifyItemByPK")]
        [HttpGet]
        public IHttpActionResult GetIdentifyItemByPK(int identifiedItemPK)
        {
            List<Client_IdentifiedItemCheckedDetail> client_IdentifiedItems = new List<Client_IdentifiedItemCheckedDetail>();
            BoxDAO boxController = new BoxDAO();
            PackedItemsDAO packedItemsController = new PackedItemsDAO();
            try
            {
                IdentifiedItem identifiedItem = db.IdentifiedItems.Find(identifiedItemPK);
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
                packedItemsController.IsInitAllCalculate(packedItem.PackedItemPK);
                client_IdentifiedItems.Add(new Client_IdentifiedItemCheckedDetail(identifiedItem, accessory, pack.PackID,
                    packedItemsController.Sample, packedItemsController.SumOfCheckedQuantity));

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, client_IdentifiedItems);
        }

        [Route("api/CheckingController/GetCheckingSessionByUserID")]
        [HttpGet]
        public IHttpActionResult GetCheckingSessionByUserID(string userID)
        {
            List<Client_CheckingSessionDetail> client_CheckingSessions = new List<Client_CheckingSessionDetail>();
            try
            {
                List<CheckingSession> checkingSessions = (from ss in db.CheckingSessions.OrderByDescending(unit => unit.CheckingSessionPK)
                                                          where ss.UserID == userID
                                                          select ss).ToList();
                foreach (var checkingSession in checkingSessions)
                {
                    IdentifiedItem identifiedItems = (from iI in db.IdentifiedItems
                                                      where iI.IdentifiedItemPK == checkingSession.IdentifiedItemPK
                                                      select iI).FirstOrDefault();
                    // lấy box tương ứng
                    UnstoredBox uBox = db.UnstoredBoxes.Find(identifiedItems.UnstoredBoxPK);
                    Box box = db.Boxes.Find(uBox.BoxPK);
                    // lấy packID tương ứng
                    PackedItem packedItem = (from pI in db.PackedItems
                                             where pI.PackedItemPK == identifiedItems.PackedItemPK
                                             select pI).FirstOrDefault();
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
                    PackedItemsDAO packedItemsController = new PackedItemsDAO();
                    packedItemsController.IsInitAllCalculate(packedItem.PackedItemPK);
                    client_CheckingSessions.Add(new Client_CheckingSessionDetail(accessory, pack, checkingSession, box, packedItem, packedItemsController.Sample));


                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, client_CheckingSessions);
        }

        [Route("api/CheckingController/CheckItemBusiness")]
        [HttpPost]
        public IHttpActionResult CheckItemBusiness(int identifiedItemPK, double checkedQuantity, double unqualifiedQuantity, string userID, string comment)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Inspector"))
            {
                // khởi tạo
                CheckingItemDAO checkingItemController = new CheckingItemDAO();
                PackedItemsDAO packedItemsController = new PackedItemsDAO();
                // chạy lệnh checking
                try
                {
                    IdentifiedItem identifiedItem = db.IdentifiedItems.Find(identifiedItemPK);
                    // kiểm state pack is closed
                    PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                    Pack pack = db.Packs.Find(packedItem.PackPK);
                    if (pack.IsOpened == false)
                    {
                        // kiểm packeditem ứng với identified item đã classified chưa
                        if (!packedItemsController.IsPackedItemClassified(identifiedItem))
                        {
                            if (!identifiedItem.IsChecked)
                            {
                                packedItemsController.IsInitAllCalculate(packedItem.PackedItemPK);
                                if (checkedQuantity <= packedItemsController.Sample && unqualifiedQuantity <= checkedQuantity && PrimitiveType.isValidQuantity(checkedQuantity))
                                {
                                    // tạo session update và ischecked
                                    checkingItemController.createCheckingSession(new CheckingSession(checkedQuantity, unqualifiedQuantity, identifiedItemPK, userID, comment)
                                        , identifiedItemPK);
                                }
                                else
                                {
                                    return Content(HttpStatusCode.Conflict, SystemMessage.NotPassPrimitiveType);
                                }
                            }
                            else
                            {
                                return Content(HttpStatusCode.Conflict, "ĐÃ ĐƯỢC KIỂM");
                            }
                        }
                        else
                        {
                            return Content(HttpStatusCode.Conflict, "CỤM PHỤ LIỆU ĐÃ ĐƯỢC ĐÁNH GIÁ");
                        }
                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "PHIẾU NHẬP CHƯA ĐÓNG");
                    }
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }

                return Content(HttpStatusCode.OK, "KIỂM CỤM PHỤ LIỆU THÀNH CÔNG");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }

        }

        [Route("api/CheckingController/EditCheckItemBusiness")]
        [HttpPut]
        public IHttpActionResult EditCheckItemBusiness(int checkingSessionPK, double checkedQuantity, double unqualifiedQuantity, string userID, string comment)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Inspector"))
            {
                // khởi tạo
                CheckingItemDAO checkingItemController = new CheckingItemDAO();
                PackedItemsDAO packedItemsController = new PackedItemsDAO();
                // chạy lệnh edit checking
                try
                {
                    CheckingSession checkingSession = db.CheckingSessions.Find(checkingSessionPK);
                    IdentifiedItem identifiedItem = db.IdentifiedItems.Find(checkingSession.IdentifiedItemPK);
                    if (checkingSession.UserID == userID)
                    {
                        if (!packedItemsController.IsPackedItemClassified(identifiedItem))
                        {
                            if (!identifiedItem.IsChecked)
                            {
                                if (checkedQuantity <= packedItemsController.Sample && unqualifiedQuantity <= checkedQuantity && PrimitiveType.isValidQuantity(checkedQuantity))
                                {
                                    // update session check
                                    checkingItemController.updateCheckingSession(checkingSessionPK, checkedQuantity, unqualifiedQuantity, comment);
                                }
                                else
                                {
                                    return Content(HttpStatusCode.Conflict, SystemMessage.NotPassPrimitiveType);
                                }

                            }
                            else
                            {
                                return Content(HttpStatusCode.Conflict, "PHỤ LIỆU PHIẾU NHẬP CHƯA ĐƯỢC KIỂM");
                            }
                        }
                        else
                        {
                            return Content(HttpStatusCode.Conflict, "PHỤ LIỆU PHIẾU NHẬP ĐÃ ĐƯỢC ĐÁNH GIÁ");
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

                return Content(HttpStatusCode.OK, "THAY ĐỔI PHIÊN KIỂM THÀNH CÔNG");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }

        }

        [Route("api/CheckingController/DeleteCheckItemBusiness")]
        [HttpDelete]
        public IHttpActionResult DeleteCheckItemBusiness(int checkingSessionPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Inspector"))
            {
                // khởi tạo
                CheckingItemDAO checkingItemController = new CheckingItemDAO();
                PackedItemsDAO packedItemsController = new PackedItemsDAO();
                // chạy lệnh edit checking
                try
                {
                    CheckingSession checkingSession = db.CheckingSessions.Find(checkingSessionPK);
                    IdentifiedItem identifiedItem = db.IdentifiedItems.Find(checkingSession.IdentifiedItemPK);
                    if (checkingSession.UserID == userID)
                    {
                        if (!packedItemsController.IsPackedItemClassified(identifiedItem))
                        {
                            checkingItemController.deleteCheckingSession(checkingSessionPK, identifiedItem.IdentifiedItemPK);
                        }
                        else
                        {
                            return Content(HttpStatusCode.Conflict, "PHỤ LIỆU PHIẾU NHẬP ĐÃ ĐƯỢC ĐÁNH GIÁ");
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

                return Content(HttpStatusCode.OK, "XÓA PHIÊN KIỂM PHỤ LIỆU THÀNH CÔNG");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }

        }

        // Classify
        [Route("api/CheckingController/GetPackedItemByBoxIDUserID")]
        [HttpGet]
        public IHttpActionResult GetPackedItemByBoxIDUserID(string boxID, string userID)
        {
            List<Client_PackedItemClassified> client_PackedItemClassifieds = new List<Client_PackedItemClassified>();
            BoxDAO boxController = new BoxDAO();
            try
            {
                Box box = boxController.GetBoxByBoxID(boxID);
                UnstoredBox uBox = boxController.GetUnstoredBoxbyBoxPK(box.BoxPK);
                List<IdentifiedItem> identifiedItems = (from iI in db.IdentifiedItems.OrderByDescending(unit => unit.PackedItemPK)
                                                        where iI.UnstoredBoxPK == uBox.UnstoredBoxPK
                                                        select iI).ToList();
                // get packed items distinct from identified item
                HashSet<PackedItem> packedItems = new HashSet<PackedItem>();
                foreach (var identifiedItem in identifiedItems)
                {
                    PackedItem packedItem = (from pI in db.PackedItems
                                             where pI.PackedItemPK == identifiedItem.PackedItemPK
                                             select pI).FirstOrDefault();
                    packedItems.Add(packedItem);
                }
                foreach (var packedItem in packedItems)
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

                    // lấy classifiedItem
                    ClassifiedItem classifiedItem = (from cI in db.ClassifiedItems
                                                     where cI.PackedItemPK == packedItem.PackedItemPK
                                                     select cI).FirstOrDefault();
                    if (classifiedItem != null)
                    {
                        ClassifyingSession classifyingSession = (from Css in db.ClassifyingSessions
                                                                 where Css.ClassifiedItemPK == classifiedItem.ClassifiedItemPK
                                                                 select Css).FirstOrDefault();
                        bool isEditable = false;
                        if (classifyingSession.UserID == userID)
                        {
                            isEditable = true;
                        }
                        client_PackedItemClassifieds.Add(new Client_PackedItemClassified(accessory, pack, packedItem, isEditable, classifiedItem));
                    }
                    else
                    {
                        client_PackedItemClassifieds.Add(new Client_PackedItemClassified(accessory, pack, packedItem));
                    }

                }

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, client_PackedItemClassifieds);
        }

        [Route("api/CheckingController/GetPackedItemByPackedItemPK")]
        [HttpGet]
        public IHttpActionResult GetPackedItemByPackedItemPK(int packedItemPK)
        {
            List<Client_PackedItemClassified2> client_PackedItemClassifieds = new List<Client_PackedItemClassified2>();
            BoxDAO boxController = new BoxDAO();
            PackedItemsDAO packedItemsController = new PackedItemsDAO();
            try
            {
                PackedItem packedItem = db.PackedItems.Find(packedItemPK);
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

                // lấy sum identified quantity, sample, defectlimit
                // lấy sum counted quantity, sum checked quantity
                packedItemsController.IsInitAllCalculate(packedItem.PackedItemPK);
                client_PackedItemClassifieds.Add(new Client_PackedItemClassified2(accessory, pack, packedItem,
                packedItemsController.Sample, packedItemsController.DefectLimit,
                packedItemsController.SumOfIdentifiedQuantity, packedItemsController.SumOfCountedQuantity,
                packedItemsController.SumOfCheckedQuantity, packedItemsController.SumOfUnqualifiedQuantity));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, client_PackedItemClassifieds);
        }

        [Route("api/CheckingController/ClassifyItemBusiness")]
        [HttpPost]
        public IHttpActionResult ClassifyItemBusiness(int packedItemPK, string comment, int qualityState, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Inspector"))
            {
                // khởi tạo
                IdentifyItemDAO identifyItemController = new IdentifyItemDAO();
                ClassifyItemDAO classifyingItemController = new ClassifyItemDAO();
                PackedItemsDAO packedItemsController = new PackedItemsDAO();
                // chạy lệnh classify
                try
                {
                    if (comment.Length > 50)
                    {
                        return Content(HttpStatusCode.Conflict, SystemMessage.NotPassPrimitiveType);
                    }
                    PackedItem packedItem = db.PackedItems.Find(packedItemPK);
                    Pack pack = db.Packs.Find(packedItem.PackPK);
                    // pack đang được đóng
                    if (!pack.IsOpened)
                    {
                        ClassifiedItem tempItem = (from cI in db.ClassifiedItems
                                                   where cI.PackedItemPK == packedItemPK
                                                   select cI).FirstOrDefault();


                        // nếu có classify item của packeditem thì edit
                        if (tempItem != null)
                        {
                            ClassifyingSession tempSS = (from cS in db.ClassifyingSessions
                                                         where cS.ClassifiedItemPK == tempItem.ClassifiedItemPK
                                                         select cS).FirstOrDefault();
                            if (userID.Equals(tempSS.UserID))
                            {
                                if (classifyingItemController.isNotStoredOrReturned(tempItem.ClassifiedItemPK))
                                {
                                    // tạo failed or passed item
                                    classifyingItemController.manageItemByQualityState(tempItem.ClassifiedItemPK, tempItem.QualityState, qualityState);
                                    // edit


                                    classifyingItemController.updateClassifiedItem(tempItem.ClassifiedItemPK, qualityState);
                                    classifyingItemController.updateClassifyingSession(tempSS.ClassifyingSessionPK, comment);
                                }
                                else
                                {
                                    return Content(HttpStatusCode.Conflict, "PHỤ LIỆU PHIẾU NHẬP ĐÃ ĐƯỢC TRẢ HOẶC LƯU KHO");
                                }
                            }
                            else
                            {
                                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN THỰC HIỆN VIỆC NÀY");
                            }

                        }
                        // nếu chưa có classify item của packitem thì tạo mới
                        else
                        {
                            double finalQuantity = identifyItemController.FinalQuantity(packedItemPK);
                            ClassifiedItem classifiedItem = new ClassifiedItem(qualityState, finalQuantity, packedItemPK);

                            // tạo classified item
                            classifiedItem = classifyingItemController.createClassifiedItem(classifiedItem);

                            // tạo classifying session
                            classifyingItemController.createClassifyingSession(new ClassifyingSession(comment, classifiedItem.ClassifiedItemPK, userID));

                            // đổi IsClassified của pack item
                            packedItem.IsClassified = true;
                            packedItemsController.IsUpdatedPackedItem(packedItem);

                            // tạo failed or passed item
                            classifyingItemController.createItemByQualityState(classifiedItem.ClassifiedItemPK, qualityState);
                        }
                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "PHIẾU NHẬP CHƯA ĐÓNG, KHÔNG THỂ ĐÁNH GIÁ");
                    }
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }

                return Content(HttpStatusCode.OK, "ĐÁNH GIÁ PHỤ LIỆU PHIẾU NHẬP THÀNH CÔNG");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }

        }

        [Route("api/CheckingController/GetClassifyingSessionsByUserID")]
        [HttpGet]
        public IHttpActionResult GetClassifyingSessionsByUserID(string userID)
        {
            List<Client_ClassifyingSession> client_ClassifyingSessions = new List<Client_ClassifyingSession>();

            try
            {
                List<ClassifyingSession> classifyingSessions = (from Css in db.ClassifyingSessions
                                                                where Css.UserID == userID
                                                                select Css).ToList();
                foreach (var classifyingSession in classifyingSessions)
                {
                    // lấy classifiedItem
                    ClassifiedItem classifiedItem = (from cI in db.ClassifiedItems
                                                     where cI.ClassifiedItemPK == classifyingSession.ClassifiedItemPK
                                                     select cI).FirstOrDefault();
                    PackedItem packedItem = (from pI in db.PackedItems
                                             where pI.PackedItemPK == classifiedItem.PackedItemPK
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
                    bool isStoredOrReturn = false;
                    if (classifiedItem.QualityState == 2)
                    {
                        PassedItem passedItem = (from pI in db.PassedItems
                                                 where pI.ClassifiedItemPK == classifiedItem.ClassifiedItemPK
                                                 select pI).FirstOrDefault();
                        isStoredOrReturn = passedItem.IsStored;
                    }
                    else if (classifiedItem.QualityState == 3)
                    {
                        FailedItem failedItem = (from fI in db.FailedItems
                                                 where fI.ClassifiedItemPK == classifiedItem.ClassifiedItemPK
                                                 select fI).FirstOrDefault();
                        isStoredOrReturn = failedItem.IsReturned;
                    }
                    client_ClassifyingSessions.Add(new Client_ClassifyingSession(accessory, pack, classifyingSession, classifiedItem, isStoredOrReturn));
                }

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, client_ClassifyingSessions);
        }

        [Route("api/CheckingController/GetClassifyingSessionsByPK")]
        [HttpGet]
        public IHttpActionResult GetClassifyingSessionsByPK(int classifyingSessionPK)
        {
            List<Client_ClassifyingSessionDetail> client_ClassifyingSessions = new List<Client_ClassifyingSessionDetail>();
            PackedItemsDAO packedItemsController = new PackedItemsDAO();
            try
            {
                ClassifyingSession classifyingSession = db.ClassifyingSessions.Find(classifyingSessionPK);
                // lấy classifiedItem
                ClassifiedItem classifiedItem = (from cI in db.ClassifiedItems
                                                 where cI.ClassifiedItemPK == classifyingSession.ClassifiedItemPK
                                                 select cI).FirstOrDefault();
                PackedItem packedItem = (from pI in db.PackedItems
                                         where pI.PackedItemPK == classifiedItem.PackedItemPK
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
                packedItemsController.IsInitAllCalculate(packedItem.PackedItemPK);
                client_ClassifyingSessions.Add(new Client_ClassifyingSessionDetail(accessory, pack, classifyingSession, classifiedItem, packedItem,
                packedItemsController.Sample, packedItemsController.DefectLimit,
                packedItemsController.SumOfIdentifiedQuantity, packedItemsController.SumOfCountedQuantity,
                packedItemsController.SumOfCheckedQuantity, packedItemsController.SumOfUnqualifiedQuantity));


            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, client_ClassifyingSessions);
        }

        [Route("api/CheckingController/DeleteClassifyItemBusiness")]
        [HttpDelete]
        public IHttpActionResult DeleteClassifyBusiness(int classifyingSessionPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Inspector"))
            {
                // khởi tạo
                ClassifyItemDAO classifyingItemController = new ClassifyItemDAO();
                PackedItemsDAO packedItemsController = new PackedItemsDAO();
                // chạy lệnh delete classify
                try
                {
                    // init
                    ClassifyingSession classifyingSession = db.ClassifyingSessions.Find(classifyingSessionPK);
                    if (userID.Equals(classifyingSession.UserID))
                    {
                        ClassifiedItem classifiedItem = db.ClassifiedItems.Find(classifyingSession.ClassifiedItemPK);
                        PackedItem packedItem = db.PackedItems.Find(classifiedItem.PackedItemPK);

                        if (classifyingItemController.isNotStoredOrReturned(classifiedItem.ClassifiedItemPK))
                        {
                            // delete
                            classifyingItemController.deleteClassifyingSession(classifyingSession.ClassifyingSessionPK);
                            classifyingItemController.deleteItemByQualityState(classifiedItem.ClassifiedItemPK, classifiedItem.QualityState);
                            classifyingItemController.deleteClassifiedItem(classifiedItem.ClassifiedItemPK);
                            packedItem.IsClassified = false;
                            packedItemsController.IsUpdatedPackedItem(packedItem);
                        }
                        else
                        {
                            return Content(HttpStatusCode.Conflict, "PHỤ LIỆU PHIẾU NHẬP ĐÃ ĐƯỢC TRẢ HOẶC LƯU KHO");
                        }
                    }
                    else
                    {

                    }
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }

                return Content(HttpStatusCode.OK, "XÓA PHIÊN ĐÁNH GIÁ THÀNH CÔNG");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }

        }

        [Route("api/CheckingController/GetFailedItem")]
        [HttpGet]
        public IHttpActionResult GetFailedItem()
        {
            List<Client_FailedItem> client_FailedItems = new List<Client_FailedItem>();
            PackedItemsDAO packedItemsController = new PackedItemsDAO();
            try
            {

                List<FailedItem> failedItems = db.FailedItems.Where(unit => unit.IsReturned == false).ToList();

                foreach (var failedItem in failedItems)
                {
                    // lấy classifiedItem
                    ClassifiedItem classifiedItem = (from cI in db.ClassifiedItems
                                                     where cI.ClassifiedItemPK == failedItem.ClassifiedItemPK
                                                     select cI).FirstOrDefault();
                    ClassifyingSession classifyingSession = (from Css in db.ClassifyingSessions
                                                             where Css.ClassifiedItemPK == classifiedItem.ClassifiedItemPK
                                                             select Css).FirstOrDefault();
                    PackedItem packedItem = (from pI in db.PackedItems
                                             where pI.PackedItemPK == classifiedItem.PackedItemPK
                                             select pI).FirstOrDefault();
                    // Tùng chưa chốt
                    IdentifiedItem identifiedItem = (from iI in db.IdentifiedItems
                                                     where iI.PackedItemPK == packedItem.PackedItemPK
                                                     select iI).FirstOrDefault();
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
                    packedItemsController.IsInitAllCalculate(packedItem.PackedItemPK);
                    client_FailedItems.Add(new Client_FailedItem(accessory, pack, classifyingSession, failedItem, packedItemsController.SumOfIdentifiedQuantity));

                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, client_FailedItems);
        }

        [Route("api/CheckingController/GetFailedItemByFailedItemPK")]
        [HttpGet]
        public IHttpActionResult GetFailedItemByFailedItemPK(int FailedItemPK)
        {
            List<Client_FailedItemDetail> client_FailedItems = new List<Client_FailedItemDetail>();
            PackedItemsDAO packedItemsController = new PackedItemsDAO();
            try
            {

                FailedItem failedItem = db.FailedItems.Find(FailedItemPK);
                // lấy classifiedItem
                ClassifiedItem classifiedItem = (from cI in db.ClassifiedItems
                                                 where cI.ClassifiedItemPK == failedItem.ClassifiedItemPK
                                                 select cI).FirstOrDefault();
                ClassifyingSession classifyingSession = (from Css in db.ClassifyingSessions
                                                         where Css.ClassifiedItemPK == classifiedItem.ClassifiedItemPK
                                                         select Css).FirstOrDefault();
                // lấy user
                SystemUser systemUser = db.SystemUsers.Find(classifyingSession.UserID);

                // lấy packed item
                PackedItem packedItem = (from pI in db.PackedItems
                                         where pI.PackedItemPK == classifiedItem.PackedItemPK
                                         select pI).FirstOrDefault();
                List<IdentifiedItem> identifiedItems = (from iI in db.IdentifiedItems
                                                        where iI.PackedItemPK == packedItem.PackedItemPK
                                                        select iI).ToList();
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

                // init all caculate
                packedItemsController.IsInitAllCalculate(packedItem.PackedItemPK);
                HashSet<string> boxIDs = new HashSet<string>();
                foreach (var identifiedItem in identifiedItems)
                {
                    UnstoredBox unstoredBox = (from uBox in db.UnstoredBoxes
                                               where uBox.UnstoredBoxPK == identifiedItem.UnstoredBoxPK
                                               select uBox).FirstOrDefault();

                    Box boxk = (from box in db.Boxes
                                where box.BoxPK == unstoredBox.BoxPK
                                select box).FirstOrDefault();
                    boxIDs.Add(boxk.BoxID);
                }
                client_FailedItems.Add(new Client_FailedItemDetail(accessory, pack, classifyingSession, identifiedItems[0], failedItem, systemUser,
                    packedItem, packedItemsController.Sample, packedItemsController.DefectLimit,
                    packedItemsController.SumOfIdentifiedQuantity, packedItemsController.SumOfCountedQuantity,
                    packedItemsController.SumOfCheckedQuantity, packedItemsController.SumOfUnqualifiedQuantity, boxIDs));

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, client_FailedItems);
        }

        [Route("api/CheckingController/ReturnItemBusiness")]
        [HttpPost]
        public IHttpActionResult ReturnItemBusiness(int failedItemPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Inspector"))
            {
                // khởi tạo
                ReturningItemDAO returningItemController = new ReturningItemDAO();
                // chạy lệnh classify
                try
                {
                    returningItemController.createReturningSession(failedItemPK, userID);
                    returningItemController.updateFailedItemIsReturned(failedItemPK);
                    returningItemController.updateAllIdentifiedItemsToVirtualBox(failedItemPK);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }

                return Content(HttpStatusCode.OK, "TRẢ PHỤ LIỆU PHIẾU NHẬP THÀNH CÔNG");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }

        }
    }
}
