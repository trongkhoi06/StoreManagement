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
                    List<IdentifyingSession> identifyingSessions = (from ss in db.IdentifyingSessions
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
                        string content = pack.PackID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, ss.UserID + " (" + systemUser.Name + ")", content));
                    }
                    break;
                case 2:
                    List<CountingSession> countingSessions = (from ss in db.CountingSessions
                                                              where ss.ExecutedDate >= start && ss.ExecutedDate <= end
                                                              select ss).ToList();
                    foreach (var ss in countingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        // query pack
                        IdentifiedItem identifiedItem = db.IdentifiedItems.Find(ss.IdentifiedItemPK);
                        PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                        Pack pack = db.Packs.Find(packedItem.PackPK);
                        string content = pack.PackID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, ss.UserID + " (" + systemUser.Name + ")", content));
                    }
                    break;
                case 3:
                    List<CheckingSession> checkingSessions = (from ss in db.CheckingSessions
                                                              where ss.ExecutedDate >= start && ss.ExecutedDate <= end
                                                              select ss).ToList();
                    foreach (var ss in checkingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        IdentifiedItem identifiedItem = db.IdentifiedItems.Find(ss.IdentifiedItemPK);
                        PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                        Pack pack = db.Packs.Find(packedItem.PackPK);
                        string content = pack.PackID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, ss.UserID + " (" + systemUser.Name + ")", content));
                    }
                    break;
                case 4:
                    List<ClassifyingSession> classifyingSessions = (from ss in db.ClassifyingSessions
                                                                    where ss.ExecutedDate >= start && ss.ExecutedDate <= end
                                                                    select ss).ToList();
                    foreach (var ss in classifyingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        ClassifiedItem classifiedItem = db.ClassifiedItems.Find(ss.ClassifiedItemPK);
                        PackedItem packedItem = db.PackedItems.Find(classifiedItem.PackedItemPK);
                        Pack pack = db.Packs.Find(packedItem.PackPK);
                        string content = pack.PackID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, ss.UserID + " (" + systemUser.Name + ")", content));
                    }
                    break;
                case 5:
                    List<ArrangingSession> arrangingSessions = (from ss in db.ArrangingSessions
                                                                where ss.ExecutedDate >= start && ss.ExecutedDate <= end
                                                                select ss).ToList();
                    foreach (var ss in arrangingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        UnstoredBox uBox1 = db.UnstoredBoxes.Find(ss.StartBoxPK);
                        Box box1 = db.Boxes.Find(uBox1.BoxPK);
                        UnstoredBox uBox2 = db.UnstoredBoxes.Find(ss.DestinationBoxPK);
                        Box box2 = db.Boxes.Find(uBox2.BoxPK);
                        string content = box1.BoxID.Substring(0,box1.BoxID.Length-3) + "~!~Div~!~" + box2.BoxID.Substring(0,box2.BoxID.Length-3);
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, ss.UserID + " (" + systemUser.Name + ")", content));
                    }
                    break;
                case 6:
                    List<ReturningSession> returningSessions = (from ss in db.ReturningSessions
                                                                where ss.ExecutedDate >= start && ss.ExecutedDate <= end
                                                                select ss).ToList();
                    foreach (var ss in returningSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        FailedItem failedItem = db.FailedItems.Find(ss.FailedItemPK);
                        ClassifiedItem classifiedItem = db.ClassifiedItems.Find(failedItem.ClassifiedItemPK);
                        PackedItem packedItem = db.PackedItems.Find(classifiedItem.PackedItemPK);
                        Pack pack = db.Packs.Find(packedItem.PackPK);
                        string content = pack.PackID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, ss.UserID + " (" + systemUser.Name + ")", content));
                    }
                    break;
                case 7:
                    List<StoringSession> storingSessions = (from ss in db.StoringSessions
                                                            where ss.ExecutedDate >= start && ss.ExecutedDate <= end
                                                            select ss).ToList();
                    foreach (var ss in storingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        Box box = db.Boxes.Find(ss.BoxPK);
                        string content = box.BoxID.Substring(0,box.BoxID.Length-3);
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, ss.UserID + " (" + systemUser.Name + ")", content));
                    }
                    break;
                case 8:
                    List<MovingSession> movingSessions = (from ss in db.MovingSessions
                                                          where ss.ExecutedDate >= start && ss.ExecutedDate <= end
                                                          select ss).ToList();
                    foreach (var ss in movingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        StoredBox sBox = db.StoredBoxes.Find(ss.StoredBoxPK);
                        Box box = db.Boxes.Find(sBox.BoxPK);
                        Shelf shelf1 = db.Shelves.Find(ss.StartShelfPK);
                        Shelf shelf2 = db.Shelves.Find(ss.DestinationShelfPK);
                        string content = box.BoxID.Substring(0, box.BoxID.Length - 3) + "~!~Div~!~" + shelf1.ShelfID + "~!~Div~!~" + shelf2.ShelfID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, ss.UserID + " (" + systemUser.Name + ")", content));
                    }
                    break;
                case 9:
                    List<TransferringSession> tranferringSessions = (from ss in db.TransferringSessions
                                                                     where ss.ExecutedDate >= start && ss.ExecutedDate <= end
                                                                     select ss).ToList();
                    foreach (var ss in tranferringSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        StoredBox sBox1 = db.StoredBoxes.Find(ss.StartBoxPK);
                        Box box1 = db.Boxes.Find(sBox1.BoxPK);

                        StoredBox sBox2 = db.StoredBoxes.Find(ss.StartBoxPK);
                        Box box2 = db.Boxes.Find(sBox2.BoxPK);

                        string content = box1.BoxID.Substring(0,box1.BoxID.Length-3) + "~!~Div~!~" + box2.BoxID.Substring(0,box2.BoxID.Length-3);
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, ss.UserID + " (" + systemUser.Name + ")", content));
                    }
                    break;
                case 10:
                    List<IssuingSession> issuingSessions = (from ss in db.IssuingSessions
                                                           where ss.ExecutedDate >= start && ss.ExecutedDate <= end
                                                           select ss).ToList();
                    foreach (var ss in issuingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        Request request = db.Requests.Find(ss.RequestPK);
                        string content = request.RequestID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, ss.UserID + " (" + systemUser.Name + ")", content));
                    }
                    break;
                case 11:
                    List<ReceivingSession> restoringSessions = (from ss in db.ReceivingSessions
                                                                where ss.ExecutedDate >= start && ss.ExecutedDate <= end
                                                                select ss).ToList();
                    foreach (var ss in restoringSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        Restoration restoration = db.Restorations.Find(ss.RestorationPK);
                        string content = restoration.RestorationID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, ss.UserID + " (" + systemUser.Name + ")", content));
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
                    List<IdentifyingSession> identifyingSessions = (from ss in db.IdentifyingSessions
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
                        string content = pack.PackID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, ss.UserID + " (" + systemUser.Name + ")", content));
                    }
                    break;
                case 2:
                    List<CountingSession> countingSessions = (from ss in db.CountingSessions
                                                              select ss).ToList();
                    foreach (var ss in countingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        // query pack
                        IdentifiedItem identifiedItem = db.IdentifiedItems.Find(ss.IdentifiedItemPK);
                        PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                        Pack pack = db.Packs.Find(packedItem.PackPK);
                        string content = pack.PackID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, ss.UserID + " (" + systemUser.Name + ")", content));
                    }
                    break;
                case 3:
                    List<CheckingSession> checkingSessions = (from ss in db.CheckingSessions
                                                              select ss).ToList();
                    foreach (var ss in checkingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        IdentifiedItem identifiedItem = db.IdentifiedItems.Find(ss.IdentifiedItemPK);
                        PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                        Pack pack = db.Packs.Find(packedItem.PackPK);
                        string content = pack.PackID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, ss.UserID + " (" + systemUser.Name + ")", content));
                    }
                    break;
                case 4:
                    List<ClassifyingSession> classifyingSessions = (from ss in db.ClassifyingSessions
                                                                    select ss).ToList();
                    foreach (var ss in classifyingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        ClassifiedItem classifiedItem = db.ClassifiedItems.Find(ss.ClassifiedItemPK);
                        PackedItem packedItem = db.PackedItems.Find(classifiedItem.PackedItemPK);
                        Pack pack = db.Packs.Find(packedItem.PackPK);
                        string content = pack.PackID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, ss.UserID + " (" + systemUser.Name + ")", content));
                    }
                    break;
                case 5:
                    List<ArrangingSession> arrangingSessions = (from ss in db.ArrangingSessions
                                                                select ss).ToList();
                    foreach (var ss in arrangingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        UnstoredBox uBox1 = db.UnstoredBoxes.Find(ss.StartBoxPK);
                        Box box1 = db.Boxes.Find(uBox1.BoxPK);
                        UnstoredBox uBox2 = db.UnstoredBoxes.Find(ss.DestinationBoxPK);
                        Box box2 = db.Boxes.Find(uBox2.BoxPK);
                        string content = box1.BoxID.Substring(0,box1.BoxID.Length-3) + "~!~Div~!~" + box2.BoxID.Substring(0,box2.BoxID.Length-3);
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, ss.UserID + " (" + systemUser.Name + ")", content));
                    }
                    break;
                case 6:
                    List<ReturningSession> returningSessions = (from ss in db.ReturningSessions
                                                                select ss).ToList();
                    foreach (var ss in returningSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        FailedItem failedItem = db.FailedItems.Find(ss.FailedItemPK);
                        ClassifiedItem classifiedItem = db.ClassifiedItems.Find(failedItem.ClassifiedItemPK);
                        PackedItem packedItem = db.PackedItems.Find(classifiedItem.PackedItemPK);
                        Pack pack = db.Packs.Find(packedItem.PackPK);
                        string content = pack.PackID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, ss.UserID + " (" + systemUser.Name + ")", content));
                    }
                    break;
                case 7:
                    List<StoringSession> storingSessions = (from ss in db.StoringSessions
                                                            select ss).ToList();
                    foreach (var ss in storingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        Box box = db.Boxes.Find(ss.BoxPK);
                        string content = box.BoxID.Substring(0,box.BoxID.Length-3);
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, ss.UserID + " (" + systemUser.Name + ")", content));
                    }
                    break;
                case 8:
                    List<MovingSession> movingSessions = (from ss in db.MovingSessions
                                                          select ss).ToList();
                    foreach (var ss in movingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        StoredBox sBox = db.StoredBoxes.Find(ss.StoredBoxPK);
                        Box box = db.Boxes.Find(sBox.BoxPK);
                        Shelf shelf1 = db.Shelves.Find(ss.StartShelfPK);
                        Shelf shelf2 = db.Shelves.Find(ss.DestinationShelfPK);
                        string content = box.BoxID.Substring(0,box.BoxID.Length-3) + "~!~Div~!~" + shelf1.ShelfID + "~!~Div~!~" + shelf2.ShelfID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, ss.UserID + " (" + systemUser.Name + ")", content));
                    }
                    break;
                case 9:
                    List<TransferringSession> tranferringSessions = (from ss in db.TransferringSessions
                                                                     select ss).ToList();
                    foreach (var ss in tranferringSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        StoredBox sBox1 = db.StoredBoxes.Find(ss.StartBoxPK);
                        Box box1 = db.Boxes.Find(sBox1.BoxPK);

                        StoredBox sBox2 = db.StoredBoxes.Find(ss.StartBoxPK);
                        Box box2 = db.Boxes.Find(sBox2.BoxPK);

                        string content = box1.BoxID.Substring(0,box1.BoxID.Length-3) + "~!~Div~!~" + box2.BoxID.Substring(0,box2.BoxID.Length-3);
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, ss.UserID + " (" + systemUser.Name + ")", content));
                    }
                    break;
                case 10:
                    List<IssuingSession> issuingSessions = (from ss in db.IssuingSessions
                                                            select ss).ToList();
                    foreach (var ss in issuingSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        Request request = db.Requests.Find(ss.RequestPK);
                        string content = request.RequestID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, ss.UserID + " (" + systemUser.Name + ")", content));
                    }
                    break;
                case 11:
                    List<ReceivingSession> restoringSessions = (from ss in db.ReceivingSessions
                                                                select ss).ToList();
                    foreach (var ss in restoringSessions)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                        Restoration restoration = db.Restorations.Find(ss.RestorationPK);
                        string content = restoration.RestorationID;
                        result.Add(new Client_Session_Activity_Angular(ss.ExecutedDate, ss.UserID + " (" + systemUser.Name + ")", content));
                    }
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}

