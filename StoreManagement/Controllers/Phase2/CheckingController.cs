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
    public class CheckingController : ApiController
    {
        private UserModel db = new UserModel();

        // Count
        [Route("api/ReceivingController/CountItemBusiness")]
        [HttpPost]
        public IHttpActionResult CountItemBusiness(int identifiedItemPK, int countedQuantity, string employeeCode)
        {
            // kiểm trước khi chạy lệnh

            // khởi tạo
            CountingItemController countingItemController = new CountingItemController();
            PackedItemsController packedItemsController = new PackedItemsController();
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
                if (pack.IsOpened == false)
                {
                    // kiểm packeditem ứng với identified item đã classified chưa
                    if (!packedItemsController.isPackedItemClassified(identifiedItem))
                    {
                        // kiểm identified item đã được đếm hay chưa
                        if (!identifiedItem.IsCounted)
                        {
                            // tạo session update và iscounted
                            countingItemController.createCountingSession(new CountingSession(identifiedItemPK, countedQuantity, employeeCode));
                            countingItemController.updateIsCountedOfIdentifiedItem(identifiedItemPK, true);
                        }
                        else
                        {
                            return Content(HttpStatusCode.Conflict, "Identified Item ĐÃ ĐẾM RỒI, KHÔNG ĐẾM LẠI");
                        }
                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "Identified Item ĐÃ PHÂN LOẠI");
                    }

                }
                else
                {
                    return Content(HttpStatusCode.Conflict, "Pack CHƯA ĐÓNG");
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, "Counting THÀNH CÔNG");
        }

        [Route("api/ReceivingController/EditCountItemBusiness")]
        [HttpPut]
        public IHttpActionResult EditCountItemBusiness(int countingSessionPK, int countedQuantity)
        {
            // kiểm trước khi chạy lệnh

            // khởi tạo
            CountingItemController countingItemController = new CountingItemController();
            // chạy lệnh edit counting
            try
            {
                countingItemController.updateCountingSession(countingSessionPK, countedQuantity);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, "Edit Counting THÀNH CÔNG");
        }

        [Route("api/ReceivingController/DeleteCountItemBusiness")]
        [HttpDelete]
        public IHttpActionResult DeleteCountItemBusiness(int countingSessionPK)
        {
            // kiểm trước khi chạy lệnh

            // khởi tạo
            CountingItemController countingItemController = new CountingItemController();
            // chạy lệnh edit counting
            try
            {
                CountingSession countingSession = db.CountingSessions.Find(countingSessionPK);
                countingItemController.updateIsCountedOfIdentifiedItem(countingSession.IdentifiedItemPK, false);
                countingItemController.deleteCountingSession(countingSessionPK);

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, "Delete Counting THÀNH CÔNG");
        }

        // Check
        [Route("api/ReceivingController/CheckItemBusiness")]
        [HttpPost]
        public IHttpActionResult CheckItemBusiness(int identifiedItemPK, int checkedQuantity, int unqualifiedQuantity, string employeeCode)
        {
            // kiểm trước khi chạy lệnh

            // khởi tạo
            CheckingItemController checkingItemController = new CheckingItemController();
            PackedItemsController packedItemsController = new PackedItemsController();
            // chạy lệnh checking
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
                if (pack.IsOpened == false)
                {
                    // kiểm packeditem ứng với identified item đã classified chưa
                    if (!packedItemsController.isPackedItemClassified(identifiedItem))
                    {
                        if (!identifiedItem.IsChecked)
                        {
                            // tạo session update và ischecked
                            checkingItemController.createCheckingSession(new CheckingSession(checkedQuantity, unqualifiedQuantity, identifiedItemPK, employeeCode));
                            checkingItemController.updateIsCheckedOfIdentifiedItem(identifiedItemPK, true);
                        }

                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "Identified Item ĐÃ PHÂN LOẠI");
                    }

                }
                else
                {
                    return Content(HttpStatusCode.Conflict, "Pack CHƯA ĐÓNG");
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, "Checking THÀNH CÔNG");
        }

        [Route("api/ReceivingController/EditCheckItemBusiness")]
        [HttpPut]
        public IHttpActionResult EditCheckItemBusiness(int checkingSessionPK, int checkedQuantity, int unqualifiedQuantity)
        {
            // kiểm trước khi chạy lệnh

            // khởi tạo
            CheckingItemController checkingItemController = new CheckingItemController();
            // chạy lệnh edit checking
            try
            {
                checkingItemController.updateCheckingSession(checkingSessionPK, checkedQuantity, unqualifiedQuantity);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, "Edit Checking THÀNH CÔNG");
        }

        [Route("api/ReceivingController/DeleteCheckItemBusiness")]
        [HttpDelete]
        public IHttpActionResult DeleteCheckItemBusiness(int checkingSessionPK)
        {
            // kiểm trước khi chạy lệnh

            // khởi tạo
            CheckingItemController checkingItemController = new CheckingItemController();
            // chạy lệnh edit checking
            try
            {
                CheckingSession checkingSession = db.CheckingSessions.Find(checkingSessionPK);
                checkingItemController.updateIsCheckedOfIdentifiedItem(checkingSession.IdentifiedItemPK, false);
                checkingItemController.deleteCheckingSession(checkingSessionPK);

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, "Delete Checking THÀNH CÔNG");
        }

        [Route("api/ReceivingController/ClassifyItemBusiness")]
        [HttpPost]
        public IHttpActionResult ClassifyItemBusiness(int packedItemPK, string comment, int qualityState, string employeeCode)
        {
            // kiểm trước khi chạy lệnh

            // khởi tạo
            IdentifyItemController identifyItemController = new IdentifyItemController();
            ClassifyingItemController classifyingItemController = new ClassifyingItemController();
            PackedItemsController packedItemsController = new PackedItemsController();
            // chạy lệnh classify
            try
            {
                PackedItem packedItem = db.PackedItems.Find(packedItemPK);
                Pack pack = db.Packs.Find(packedItem.PackPK);
                // pack đang được đóng
                if (!pack.IsOpened)
                {
                    ClassifiedItem tempItem = (from cI in db.ClassifiedItems
                                               where cI.PackedItemPK == packedItemPK
                                               select cI).FirstOrDefault();


                    // nếu có classify item của packitem thì edit
                    if (tempItem != null)
                    {
                        if (classifyingItemController.isNotStoredOrReturned(tempItem.ClassifiedItemPK))
                        {
                            // tạo failed or passed item
                            classifyingItemController.manageItemByQualityState(tempItem.ClassifiedItemPK, tempItem.QualityState, qualityState);
                            // edit
                            tempItem.QualityState = qualityState;
                            ClassifyingSession tempSS = (from cS in db.ClassifyingSessions
                                                         where cS.ClassifiedItemPK == tempItem.ClassifiedItemPK
                                                         select cS).FirstOrDefault();
                            tempSS.Comment = comment;
                            classifyingItemController.updateClassifiedItem(tempItem);
                            classifyingItemController.updateClassifyingSession(tempSS);
                        }
                        else
                        {
                            return Content(HttpStatusCode.Conflict, "ITEM ĐÃ ĐƯỢC TRẢ HOẶC LƯU KHO");
                        }
                    }
                    // nếu chưa có classify item của packitem thì tạo mới
                    else
                    {
                        int finalQuantity = identifyItemController.GenerateFinalQuantity(packedItemPK);
                        ClassifiedItem classifiedItem = new ClassifiedItem(qualityState, finalQuantity, packedItemPK);

                        // tạo classified item
                        classifiedItem = classifyingItemController.createClassifiedItem(classifiedItem);

                        // tạo classifying session
                        classifyingItemController.createClassifyingSession(new ClassifyingSession(comment, classifiedItem.ClassifiedItemPK, employeeCode));

                        // đổi IsClassified của pack item
                        packedItem.IsClassified = true;
                        packedItemsController.isUpdatedPackedItem(packedItem);

                        // tạo failed or passed item
                        classifyingItemController.createItemByQualityState(classifiedItem.ClassifiedItemPK, qualityState);
                    }

                }
                else
                {
                    return Content(HttpStatusCode.Conflict, "PACK CHƯA ĐÓNG, KHÔNG THỂ CLASSIFY");
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, "CLASSIFY THÀNH CÔNG");
        }

        [Route("api/ReceivingController/DeleteClassifyItemBusiness")]
        [HttpDelete]
        public IHttpActionResult DeleteClassifyBusiness(int classifyingSessionPK)
        {
            // kiểm trước khi chạy lệnh

            // khởi tạo
            ClassifyingItemController classifyingItemController = new ClassifyingItemController();
            PackedItemsController packedItemsController = new PackedItemsController();
            // chạy lệnh delete classify
            try
            {
                // init
                ClassifyingSession classifyingSession = db.ClassifyingSessions.Find(classifyingSessionPK);
                ClassifiedItem classifiedItem = db.ClassifiedItems.Find(classifyingSession.ClassifiedItemPK);
                PackedItem packedItem = db.PackedItems.Find(classifiedItem.PackedItemPK);

                if (classifyingItemController.isNotStoredOrReturned(classifiedItem.ClassifiedItemPK))
                {
                    // delete
                    classifyingItemController.deleteClassifyingSession(classifyingSession.ClassifyingSessionPK);
                    classifyingItemController.deleteItemByQualityState(classifiedItem.ClassifiedItemPK, classifiedItem.QualityState);
                    classifyingItemController.deleteClassifiedItem(classifiedItem.ClassifiedItemPK);
                    packedItem.IsClassified = false;
                    packedItemsController.isUpdatedPackedItem(packedItem);
                }
                else
                {
                    return Content(HttpStatusCode.Conflict, "ITEM ĐÃ ĐƯỢC TRẢ HOẶC LƯU KHO");
                }

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, "DELETE CLASSIFY THÀNH CÔNG");
        }

        [Route("api/ReceivingController/ReturnItemBusiness")]
        [HttpPost]
        public IHttpActionResult ReturnItemBusiness(int failedItemPK, string employeeCode)
        {
            // kiểm trước khi chạy lệnh

            // khởi tạo
            ReturningItemController returningItemController = new ReturningItemController();
            // chạy lệnh classify
            try
            {
                returningItemController.createReturningSession(failedItemPK, employeeCode);
                returningItemController.updateFailedItemIsReturned(failedItemPK);
                returningItemController.updateAllIdentifiedItemsToVirtualBox(failedItemPK);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, "RETURN THÀNH CÔNG");
        }


    }
}
