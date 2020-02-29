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
    public class IdentifyItemDAO
    {
        private UserModel db = new UserModel();

        public IdentifyingSession createdIdentifyingSession(string userID)
        {
            try
            {
                IdentifyingSession identifyingSession = new IdentifyingSession(userID);
                db.IdentifyingSessions.Add(identifyingSession);
                db.SaveChanges();
                identifyingSession = (from iss in db.IdentifyingSessions.OrderByDescending(unit => unit.IdentifyingSessionPK)
                                      select iss).FirstOrDefault();
                return identifyingSession;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void createIndentifyItem(IdentifyingSession Iss, List<Client_PackedItemPK_IdentifiedQuantity> list, UnstoredBox uBox)
        {
            try
            {
                foreach (var el in list)
                {
                    // querry lấy pack
                    PackedItem packedItem = db.PackedItems.Find(el.PackedItemPK);
                    Pack pack = db.Packs.Find(packedItem.PackPK);
                    // pack đang mở
                    if (pack.IsOpened)
                    {
                        if (PrimitiveType.isValidQuantity(el.IdentifiedQuantity))
                        {
                            db.IdentifiedItems.Add(new IdentifiedItem(el.IdentifiedQuantity, el.PackedItemPK, Iss.IdentifyingSessionPK, uBox.UnstoredBoxPK));
                        }
                        else
                        {
                            throw new Exception(SystemMessage.NotPassPrimitiveType);
                        }
                    }
                    else
                    {
                        throw new Exception("PHIẾU NHẬP ĐANG ĐÓNG, KO GHI NHẬN NHẬP HÀNG ĐƯỢC");
                    }
                }
                StoredBox sBox = db.StoredBoxes.Where(unit => unit.BoxPK == uBox.BoxPK).FirstOrDefault();
                sBox.ShelfPK = null;
                db.Entry(sBox).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        public void deleteIdentifiedItemsOfSession(int IdentifyingSessionPK)
        {
            try
            {
                IdentifyingSession identifyingSession = db.IdentifyingSessions.Find(IdentifyingSessionPK);
                List<IdentifiedItem> list = (from ii in db.IdentifiedItems
                                             where ii.IdentifyingSessionPK == IdentifyingSessionPK
                                             select ii).ToList();
                foreach (var item in list)
                {
                    db.IdentifiedItems.Remove(item);
                }
                db.IdentifyingSessions.Remove(identifyingSession);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ArrangingSession createArrangingSession(int boxFromPK, int boxToPK, string userID)
        {
            ArrangingSession arrangingSession = new ArrangingSession(boxFromPK, boxToPK, userID);
            db.ArrangingSessions.Add(arrangingSession);
            try
            {
                db.SaveChanges();
                arrangingSession = (from ass in db.ArrangingSessions.OrderByDescending(unit => unit.ArrangingSessionPK)
                                    select ass).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e;
            }
            return arrangingSession;
        }

        public double ActualQuantity(int identifiedItemPK)
        {
            double result = 0;
            //int numCase = 0;
            try
            {
                IdentifiedItem item = db.IdentifiedItems.Find(identifiedItemPK);
                CheckingSession checkingSession = (from checkss in db.CheckingSessions
                                                   where checkss.IdentifiedItemPK == item.IdentifiedItemPK
                                                   select checkss).FirstOrDefault();
                CountingSession countingSession = (from countss in db.CountingSessions
                                                   where countss.IdentifiedItemPK == item.IdentifiedItemPK
                                                   select countss).FirstOrDefault();
                //if (item.IsChecked == false && item.IsCounted == false) numCase = 1;
                //if (item.IsChecked == false && item.IsCounted == true) numCase = 2;
                //if (item.IsChecked == true && item.IsCounted == false) numCase = 3;
                //if (item.IsChecked == true && item.IsCounted == true)
                //{

                //    if (checkingSession.ExecutedDate < countingSession.ExecutedDate) numCase = 4;
                //    else numCase = 5;
                //}
                //switch (numCase)
                //{
                //    case 1:
                //        result += item.IdentifiedQuantity;
                //        break;
                //    case 2:
                //        result += countingSession.CountedQuantity;
                //        break;
                //    case 3:
                //        result += item.IdentifiedQuantity - checkingSession.UnqualifiedQuantity;
                //        break;
                //    case 4:
                //        result += countingSession.CountedQuantity;
                //        break;
                //    case 5:
                //        result += countingSession.CountedQuantity - checkingSession.UnqualifiedQuantity;
                //        break;
                //    default:
                //        break;
                //}

                if (item.IsCounted) result = countingSession.CountedQuantity;
                else result = item.IdentifiedQuantity;
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }

        public double FinalQuantity(int packedItemPK)
        {
            double result = 0;
            try
            {
                List<IdentifiedItem> identifiedItems = (from iI in db.IdentifiedItems
                                                        where iI.PackedItemPK == packedItemPK
                                                        select iI).ToList();
                foreach (var item in identifiedItems)
                {
                    int numCase = 0;
                    CheckingSession checkingSession = (from checkss in db.CheckingSessions
                                                       where checkss.IdentifiedItemPK == item.IdentifiedItemPK
                                                       select checkss).FirstOrDefault();
                    CountingSession countingSession = (from countss in db.CountingSessions
                                                       where countss.IdentifiedItemPK == item.IdentifiedItemPK
                                                       select countss).FirstOrDefault();
                    if (item.IsChecked == false && item.IsCounted == false) numCase = 1;
                    if (item.IsChecked == false && item.IsCounted == true) numCase = 2;
                    if (item.IsChecked == true && item.IsCounted == false) numCase = 3;
                    if (item.IsChecked == true && item.IsCounted == true)
                    {

                        if (checkingSession.ExecutedDate < countingSession.ExecutedDate) numCase = 4;
                        else numCase = 5;
                    }
                    switch (numCase)
                    {
                        case 1:
                            result += item.IdentifiedQuantity;
                            break;
                        case 2:
                            result += countingSession.CountedQuantity;
                            break;
                        case 3:
                            result += item.IdentifiedQuantity - checkingSession.UnqualifiedQuantity;
                            break;
                        case 4:
                            result += countingSession.CountedQuantity;
                            break;
                        case 5:
                            result += countingSession.CountedQuantity - checkingSession.UnqualifiedQuantity;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }

        public void EditIdentification(int identifyingSessionPK, string userID, List<Client_IdentifiedItemPK_IdentifiedQuantity> list)
        {
            try
            {
                IdentifyingSession identifyingSession = db.IdentifyingSessions.Find(identifyingSessionPK);
                if (identifyingSession.UserID == userID)
                {
                    bool temp = false;
                    foreach (var el in list)
                    {
                        if (el.IdentifiedQuantity != 0)
                        {
                            temp = true;
                            break;
                        }
                    }
                    if (temp == false)
                    {
                        throw new Exception("KO ĐƯỢC XÓA HẾT CỤM PHỤ LIỆU!");
                    }
                    foreach (var el in list)
                    {
                        // querry lấy pack
                        IdentifiedItem identifiedItem = db.IdentifiedItems.Find(el.IdentifiedItemPK);
                        PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                        Pack pack = db.Packs.Find(packedItem.PackPK);
                        // pack đang mở
                        if (pack.IsOpened)
                        {
                            // switch case xoa sua
                            switch (el.IdentifiedQuantity)
                            {
                                case 0:
                                    db.IdentifiedItems.Remove(identifiedItem);
                                    break;
                                default:
                                    if (PrimitiveType.isValidQuantity(el.IdentifiedQuantity))
                                    {

                                        identifiedItem.IdentifiedQuantity = el.IdentifiedQuantity;
                                        db.Entry(identifiedItem).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        throw new Exception(SystemMessage.NotPassPrimitiveType);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            throw new Exception("PHIẾU NHẬP ĐANG ĐÓNG, KO GHI NHẬN ĐƯỢC");
                        }
                    }
                    identifyingSession.ExecutedDate = DateTime.Now;
                    db.Entry(identifyingSession).State = EntityState.Modified;

                    db.SaveChanges();
                }
                else
                {
                    throw new Exception("BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void ArrangeItem(List<ReceivingController.Client_GroupItemArrange> items, UnstoredBox uBoxFrom, UnstoredBox uBoxTo, ArrangingSession arrangingSession)
        {
            foreach (var item in items)
            {
                if (item.IsRestored == false)
                {
                    IdentifiedItem identifiedItem = db.IdentifiedItems.Find(item.ItemPK);
                    // check if box from really contain that item
                    if (identifiedItem.UnstoredBoxPK == uBoxFrom.UnstoredBoxPK)
                    {
                        identifiedItem.UnstoredBoxPK = uBoxTo.UnstoredBoxPK;
                        db.Entry(identifiedItem).State = EntityState.Modified;

                        // Map session with item
                        GroupItem_ArrangingSession groupItem_ArrangingSession = new GroupItem_ArrangingSession(
                            identifiedItem.IdentifiedItemPK, false, arrangingSession.ArrangingSessionPK);

                        db.GroupItem_ArrangingSession.Add(groupItem_ArrangingSession);
                    }
                }
                else
                {
                    RestoredGroup restoredGroup = db.RestoredGroups.Find(item.ItemPK);
                    // check if box from really contain that item
                    if (restoredGroup.UnstoredBoxPK == uBoxFrom.UnstoredBoxPK)
                    {
                        restoredGroup.UnstoredBoxPK = uBoxTo.UnstoredBoxPK;
                        db.Entry(restoredGroup).State = EntityState.Modified;

                        // Map session with item
                        GroupItem_ArrangingSession groupItem_ArrangingSession = new GroupItem_ArrangingSession(
                            restoredGroup.RestoredGroupPK, true, arrangingSession.ArrangingSessionPK);

                        db.GroupItem_ArrangingSession.Add(groupItem_ArrangingSession);
                    }
                }
            }

            StoredBox sBox = db.StoredBoxes.Where(unit => unit.BoxPK == uBoxTo.BoxPK).FirstOrDefault();
            sBox.ShelfPK = null;
            db.Entry(sBox).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
