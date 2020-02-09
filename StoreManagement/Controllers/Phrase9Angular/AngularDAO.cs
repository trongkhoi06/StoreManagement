using StoreManagement.Class;
using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static StoreManagement.Controllers.AngularController;

namespace StoreManagement.Controllers
{
    public class AngularDAO
    {
        private UserModel db = new UserModel();

        public List<Client_Session_Activity_Angular> GetSessions(DateTime start, DateTime end, int sessionNum)
        {
            List<Client_Session_Activity_Angular> result = new List<Client_Session_Activity_Angular>();

            switch (sessionNum)
            {
                case 1:
                    List<IdentifyingSession> identifyingSessions = (from ss in db.IdentifyingSessions.OrderByDescending(unit => unit.IdentifyingSessionPK)
                                                                    where ss.ExecutedDate >= start && ss.ExecutedDate <= end
                                                                    select ss).ToList();
                    foreach (var ss in identifyingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        // query pack
                        IdentifiedItem identifiedItem = (from iI in db.IdentifiedItems
                                                         where iI.IdentifyingSessionPK == ss.IdentifyingSessionPK
                                                         select iI).FirstOrDefault();
                        PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                        Pack pack = db.Packs.Find(packedItem.PackPK);
                        string content = "Ghi nhận " + "cụm phụ liệu thuộc Phiếu nhập mã số " + pack.PackID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, systemUser.Name + " (" + ss.UserID + ")", content));
                    }
                    break;
                case 2:
                    List<CountingSession> countingSessions = (from ss in db.CountingSessions.OrderByDescending(unit => unit.CountingSessionPK)
                                                              where ss.ExecutedDate >= start && ss.ExecutedDate <= end
                                                              select ss).ToList();
                    foreach (var ss in countingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        // query pack
                        IdentifiedItem identifiedItem = db.IdentifiedItems.Find(ss.IdentifiedItemPK);
                        PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                        Pack pack = db.Packs.Find(packedItem.PackPK);
                        string content = "Kiểm số lượng " + "cụm phụ liệu thuộc Phiếu nhập mã số " + pack.PackID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, systemUser.Name + " (" + ss.UserID + ")", content));
                    }
                    break;
                case 3:
                    List<CheckingSession> checkingSessions = (from ss in db.CheckingSessions.OrderByDescending(unit => unit.CheckingSessionPK)
                                                              where ss.ExecutedDate >= start && ss.ExecutedDate <= end
                                                              select ss).ToList();
                    foreach (var ss in checkingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        IdentifiedItem identifiedItem = db.IdentifiedItems.Find(ss.IdentifiedItemPK);
                        PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                        Pack pack = db.Packs.Find(packedItem.PackPK);
                        string content = "Kiểm chất lượng " + "cụm phụ liệu thuộc Phiếu nhập mã số " + pack.PackID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, systemUser.Name + " (" + ss.UserID + ")", content));
                    }
                    break;
                case 4:
                    List<ClassifyingSession> classifyingSessions = (from ss in db.ClassifyingSessions.OrderByDescending(unit => unit.ClassifyingSessionPK)
                                                                    where ss.ExecutedDate >= start && ss.ExecutedDate <= end
                                                                    select ss).ToList();
                    foreach (var ss in classifyingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        ClassifiedItem classifiedItem = db.ClassifiedItems.Find(ss.ClassifiedItemPK);
                        PackedItem packedItem = db.PackedItems.Find(classifiedItem.PackedItemPK);
                        Pack pack = db.Packs.Find(packedItem.PackPK);
                        string content = "Đánh giá " + "phụ liệu thuộc Phiếu nhập mã số " + pack.PackID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, systemUser.Name + " (" + ss.UserID + ")", content));
                    }
                    break;
                case 5:
                    List<ArrangingSession> arrangingSessions = (from ss in db.ArrangingSessions.OrderByDescending(unit => unit.ArrangingSessionPK)
                                                                where ss.ExecutedDate >= start && ss.ExecutedDate <= end
                                                                select ss).ToList();
                    foreach (var ss in arrangingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        UnstoredBox uBox1 = db.UnstoredBoxes.Find(ss.StartBoxPK);
                        Box box1 = db.Boxes.Find(uBox1.BoxPK);
                        UnstoredBox uBox2 = db.UnstoredBoxes.Find(ss.DestinationBoxPK);
                        Box box2 = db.Boxes.Find(uBox2.BoxPK);
                        string content = "Sắp xếp " + "cụm phụ liệu từ thùng mã số " + box1.BoxID.Substring(0, box1.BoxID.Length - 3) + " sang thùng mã số " + box2.BoxID.Substring(0, box2.BoxID.Length - 3);
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, systemUser.Name + " (" + ss.UserID + ")", content));
                    }
                    break;
                case 6:
                    List<ReturningSession> returningSessions = (from ss in db.ReturningSessions.OrderByDescending(unit => unit.ReturningSessionPK)
                                                                where ss.ExecutedDate >= start && ss.ExecutedDate <= end
                                                                select ss).ToList();
                    foreach (var ss in returningSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        FailedItem failedItem = db.FailedItems.Find(ss.FailedItemPK);
                        ClassifiedItem classifiedItem = db.ClassifiedItems.Find(failedItem.ClassifiedItemPK);
                        PackedItem packedItem = db.PackedItems.Find(classifiedItem.PackedItemPK);
                        Pack pack = db.Packs.Find(packedItem.PackPK);
                        string content = "Trả hàng lỗi " + "thuộc Phiếu nhập mã số " + pack.PackID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, systemUser.Name + " (" + ss.UserID + ")", content));
                    }
                    break;
                case 7:
                    //List<StoringSession> storingSessions = (from ss in db.StoringSessions.OrderByDescending(unit => unit.StoringSessionPK)
                    //                                        where ss.ExecutedDate >= start && ss.ExecutedDate <= end
                    //                                        select ss).ToList();
                    //foreach (var ss in storingSessions)
                    //{
                    //    SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                    //    Box box = db.Boxes.Find(ss.BoxPK);
                    //    string content = "Lưu kho " + "thùng mã số " + box.BoxID.Substring(0, box.BoxID.Length - 3);
                    //    result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, systemUser.Name + " (" + ss.UserID + ")", content));
                    //}
                    break;
                case 8:
                    List<MovingSession> movingSessions = (from ss in db.MovingSessions.OrderByDescending(unit => unit.MovingSessionPK)
                                                          where ss.ExecutedDate >= start && ss.ExecutedDate <= end
                                                          select ss).ToList();
                    foreach (var ss in movingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        StoredBox sBox = db.StoredBoxes.Find(ss.StoredBoxPK);
                        Box box = db.Boxes.Find(sBox.BoxPK);
                        Shelf shelf1 = db.Shelves.Find(ss.StartShelfPK);
                        Shelf shelf2 = db.Shelves.Find(ss.DestinationShelfPK);
                        string content = "Di chuyển thùng " + "mã số " + box.BoxID.Substring(0, box.BoxID.Length - 3)
                            + " từ kệ " + shelf1.ShelfID + " sang kệ " + shelf2.ShelfID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, systemUser.Name + " (" + ss.UserID + ")", content));
                    }
                    break;
                case 9:
                    List<TransferringSession> tranferringSessions = (from ss in db.TransferringSessions.OrderByDescending(unit => unit.TransferingSessionPK)
                                                                     where ss.ExecutedDate >= start && ss.ExecutedDate <= end
                                                                     select ss).ToList();
                    foreach (var ss in tranferringSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        StoredBox sBox1 = db.StoredBoxes.Find(ss.StartBoxPK);
                        Box box1 = db.Boxes.Find(sBox1.BoxPK);

                        StoredBox sBox2 = db.StoredBoxes.Find(ss.StartBoxPK);
                        Box box2 = db.Boxes.Find(sBox2.BoxPK);

                        string content = "Chuyển phụ liệu " + "tồn kho từ thùng mã số " + box1.BoxID.Substring(0, box1.BoxID.Length - 3)
                            + " sang thùng mã số " + box2.BoxID.Substring(0, box2.BoxID.Length - 3);
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, systemUser.Name + " (" + ss.UserID + ")", content));
                    }
                    break;
                case 10:
                    //List<IssuingSession> issuingSessions = (from ss in db.IssuingSessions.OrderByDescending(unit => unit.IssuingSessionPK)
                    //                                        where ss.ExecutedDate >= start && ss.ExecutedDate <= end
                    //                                        select ss).ToList();
                    //foreach (var ss in issuingSessions)
                    //{
                    //    SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                    //    Request request = db.Requests.Find(ss.RequestPK);
                    //    string content = "Xuất kho " + "phụ liệu cho Yêu cầu nhận mã số " + request.RequestID;
                    //    result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, systemUser.Name + " (" + ss.UserID + ")", content));
                    //}
                    break;
                case 11:
                    List<ReceivingSession> restoringSessions = (from ss in db.ReceivingSessions.OrderByDescending(unit => unit.ReceivingSessionPK)
                                                                where ss.ExecutedDate >= start && ss.ExecutedDate <= end
                                                                select ss).ToList();
                    foreach (var ss in restoringSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        Restoration restoration = db.Restorations.Find(ss.RestorationPK);
                        string content = "Nhận tồn " + "phụ liệu thuộc Phiếu trả mã số " + restoration.RestorationID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, systemUser.Name + " (" + ss.UserID + ")", content));
                    }
                    break;
                default:
                    break;
            }

            return result;
        }

        public List<Client_Session_Activity_Angular> GetSessions(int sessionNum)
        {
            List<Client_Session_Activity_Angular> result = new List<Client_Session_Activity_Angular>();

            switch (sessionNum)
            {
                case 1:
                    List<IdentifyingSession> identifyingSessions = (from ss in db.IdentifyingSessions.OrderByDescending(unit => unit.IdentifyingSessionPK)
                                                                    select ss).ToList();
                    foreach (var ss in identifyingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        // query pack
                        IdentifiedItem identifiedItem = (from iI in db.IdentifiedItems
                                                         where iI.IdentifyingSessionPK == ss.IdentifyingSessionPK
                                                         select iI).FirstOrDefault();
                        PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                        Pack pack = db.Packs.Find(packedItem.PackPK);
                        string content = "Ghi nhận " + "cụm phụ liệu thuộc Phiếu nhập mã số " + pack.PackID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, systemUser.Name + " (" + ss.UserID + ")", content));
                    }
                    break;
                case 2:
                    List<CountingSession> countingSessions = (from ss in db.CountingSessions.OrderByDescending(unit => unit.CountingSessionPK)
                                                              select ss).ToList();
                    foreach (var ss in countingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        // query pack
                        IdentifiedItem identifiedItem = db.IdentifiedItems.Find(ss.IdentifiedItemPK);
                        PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                        Pack pack = db.Packs.Find(packedItem.PackPK);
                        string content = "Kiểm số lượng " + "cụm phụ liệu thuộc Phiếu nhập mã số " + pack.PackID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, systemUser.Name + " (" + ss.UserID + ")", content));
                    }
                    break;
                case 3:
                    List<CheckingSession> checkingSessions = (from ss in db.CheckingSessions.OrderByDescending(unit => unit.CheckingSessionPK)
                                                              select ss).ToList();
                    foreach (var ss in checkingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        IdentifiedItem identifiedItem = db.IdentifiedItems.Find(ss.IdentifiedItemPK);
                        PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                        Pack pack = db.Packs.Find(packedItem.PackPK);
                        string content = "Kiểm chất lượng " + "cụm phụ liệu thuộc Phiếu nhập mã số " + pack.PackID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, systemUser.Name + " (" + ss.UserID + ")", content));
                    }
                    break;
                case 4:
                    List<ClassifyingSession> classifyingSessions = (from ss in db.ClassifyingSessions.OrderByDescending(unit => unit.ClassifyingSessionPK)
                                                                    select ss).ToList();
                    foreach (var ss in classifyingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        ClassifiedItem classifiedItem = db.ClassifiedItems.Find(ss.ClassifiedItemPK);
                        PackedItem packedItem = db.PackedItems.Find(classifiedItem.PackedItemPK);
                        Pack pack = db.Packs.Find(packedItem.PackPK);
                        string content = "Đánh giá " + "phụ liệu thuộc Phiếu nhập mã số " + pack.PackID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, systemUser.Name + " (" + ss.UserID + ")", content));
                    }
                    break;
                case 5:
                    List<ArrangingSession> arrangingSessions = (from ss in db.ArrangingSessions.OrderByDescending(unit => unit.ArrangingSessionPK)
                                                                select ss).ToList();
                    foreach (var ss in arrangingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        UnstoredBox uBox1 = db.UnstoredBoxes.Find(ss.StartBoxPK);
                        Box box1 = db.Boxes.Find(uBox1.BoxPK);
                        UnstoredBox uBox2 = db.UnstoredBoxes.Find(ss.DestinationBoxPK);
                        Box box2 = db.Boxes.Find(uBox2.BoxPK);
                        string content = "Sắp xếp " + "cụm phụ liệu từ thùng mã số " + box1.BoxID.Substring(0, box1.BoxID.Length - 3) + " sang thùng mã số " + box2.BoxID.Substring(0, box2.BoxID.Length - 3);
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, systemUser.Name + " (" + ss.UserID + ")", content));
                    }
                    break;
                case 6:
                    List<ReturningSession> returningSessions = (from ss in db.ReturningSessions.OrderByDescending(unit => unit.ReturningSessionPK)
                                                                select ss).ToList();
                    foreach (var ss in returningSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        FailedItem failedItem = db.FailedItems.Find(ss.FailedItemPK);
                        ClassifiedItem classifiedItem = db.ClassifiedItems.Find(failedItem.ClassifiedItemPK);
                        PackedItem packedItem = db.PackedItems.Find(classifiedItem.PackedItemPK);
                        Pack pack = db.Packs.Find(packedItem.PackPK);
                        string content = "Trả hàng lỗi " + "thuộc Phiếu nhập mã số " + pack.PackID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, systemUser.Name + " (" + ss.UserID + ")", content));
                    }
                    break;
                case 7:
                    //List<StoringSession> storingSessions = (from ss in db.StoringSessions.OrderByDescending(unit => unit.StoringSessionPK)
                    //                                        select ss).ToList();
                    //foreach (var ss in storingSessions)
                    //{
                    //    SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                    //    Box box = db.Boxes.Find(ss.BoxPK);
                    //    string content = "Lưu kho " + "thùng mã số " + box.BoxID.Substring(0, box.BoxID.Length - 3);
                    //    result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, systemUser.Name + " (" + ss.UserID + ")", content));
                    //}
                    break;
                case 8:
                    List<MovingSession> movingSessions = (from ss in db.MovingSessions.OrderByDescending(unit => unit.MovingSessionPK)
                                                          select ss).ToList();
                    foreach (var ss in movingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        StoredBox sBox = db.StoredBoxes.Find(ss.StoredBoxPK);
                        Box box = db.Boxes.Find(sBox.BoxPK);
                        Shelf shelf1 = db.Shelves.Find(ss.StartShelfPK);
                        Shelf shelf2 = db.Shelves.Find(ss.DestinationShelfPK);
                        string content = "Di chuyển thùng " + "mã số " + box.BoxID.Substring(0, box.BoxID.Length - 3)
                            + " từ kệ " + shelf1.ShelfID + " sang kệ " + shelf2.ShelfID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, systemUser.Name + " (" + ss.UserID + ")", content));
                    }
                    break;
                case 9:
                    List<TransferringSession> tranferringSessions = (from ss in db.TransferringSessions.OrderByDescending(unit => unit.TransferingSessionPK)
                                                                     select ss).ToList();
                    foreach (var ss in tranferringSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        StoredBox sBox1 = db.StoredBoxes.Find(ss.StartBoxPK);
                        Box box1 = db.Boxes.Find(sBox1.BoxPK);

                        StoredBox sBox2 = db.StoredBoxes.Find(ss.StartBoxPK);
                        Box box2 = db.Boxes.Find(sBox2.BoxPK);

                        string content = "Chuyển phụ liệu " + "tồn kho từ thùng mã số " + box1.BoxID.Substring(0, box1.BoxID.Length - 3)
                            + "sang thùng mã số " + box2.BoxID.Substring(0, box2.BoxID.Length - 3);
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, systemUser.Name + " (" + ss.UserID + ")", content));
                    }
                    break;
                case 10:
                    //List<IssuingSession> issuingSessions = (from ss in db.IssuingSessions.OrderByDescending(unit => unit.IssuingSessionPK)
                    //                                        select ss).ToList();
                    //foreach (var ss in issuingSessions)
                    //{
                    //    SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                    //    Request request = db.Requests.Find(ss.RequestPK);
                    //    string content = "Xuất kho " + "phụ liệu cho Yêu cầu nhận mã số " + request.RequestID;
                    //    result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, systemUser.Name + " (" + ss.UserID + ")", content));
                    //}
                    break;
                case 11:
                    List<ReceivingSession> restoringSessions = (from ss in db.ReceivingSessions.OrderByDescending(unit => unit.ReceivingSessionPK)
                                                                select ss).ToList();
                    foreach (var ss in restoringSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        Restoration restoration = db.Restorations.Find(ss.RestorationPK);
                        string content = "Nhận tồn " + "phụ liệu thuộc Phiếu trả mã số " + restoration.RestorationID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, systemUser.Name + " (" + ss.UserID + ")", content));
                    }
                    break;
                default:
                    break;
            }

            return result;
        }
        
    }
}

