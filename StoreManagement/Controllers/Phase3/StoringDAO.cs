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
    public class StoringDAO
    {
        private UserModel db = new UserModel();

        public StoredBox CreateStoredBox(int boxPK, int shelfPK)
        {
            try
            {
                // tạo storedBox
                StoredBox storedBox = new StoredBox(boxPK, shelfPK);
                db.StoredBoxes.Add(storedBox);
                db.SaveChanges();
                return db.StoredBoxes.OrderByDescending(unit => unit.StoredBoxPK).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public StoringSession CreateStoringSession(int boxPK, string userID)
        {
            try
            {
                // tạo storedBox
                StoringSession storingSession = new StoringSession(boxPK, userID);
                db.StoringSessions.Add(storingSession);
                db.SaveChanges();
                return db.StoringSessions.OrderByDescending(unit => unit.StoringSessionPK).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void CreateEntriesUpdatePassedItem(Box box, StoredBox storedBox, StoringSession storingSession)
        {
            try
            {
                IdentifyItemDAO identifyItemDAO = new IdentifyItemDAO();
                BoxDAO boxDAO = new BoxDAO();
                UnstoredBox unstoredBox = boxDAO.GetUnstoredBoxbyBoxPK(box.BoxPK);
                List<IdentifiedItem> identifiedItems = (from iI in db.IdentifiedItems
                                                        where iI.UnstoredBoxPK == unstoredBox.UnstoredBoxPK
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
                            updatePassedItem(passedItem.PassedItemPK);
                            // tạo entry
                            Entry entry = new Entry(storedBox, "Storing", storingSession.StoringSessionPK, false,
                                identifyItemDAO.ActualQuantity(identifiedItem.IdentifiedItemPK), passedItem.PassedItemPK);
                            db.Entries.Add(entry);
                        }
                        else
                        {
                            throw new Exception("ITEM KHÔNG HỢP LỆ");
                        }
                    }
                    else
                    {
                        throw new Exception("ITEM KHÔNG HỢP LỆ");
                    }
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void updatePassedItem(int passedItemPK)
        {
            try
            {
                PassedItem passedItem = db.PassedItems.Find(passedItemPK);
                passedItem.IsStored = true;
                db.Entry(passedItem).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void deleteStoredBox(int storedBoxPK)
        {
            try
            {
                StoredBox storedBox = db.StoredBoxes.Find(storedBoxPK);
                db.StoredBoxes.Remove(storedBox);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void deleteStoringSession(int storingSessionPK)
        {
            try
            {
                StoringSession storingSession = db.StoringSessions.Find(storingSessionPK);
                db.StoringSessions.Remove(storingSession);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public double InBoxQuantity(List<Entry> entries)
        {
            double result = 0;
            foreach (var entry in entries)
            {
                AdjustingSession adjustingSession;
                Verification verification;
                KindRole kindRole = db.KindRoles.Find(entry.KindRoleName);
                switch (entry.KindRoleName)
                {
                    case "Discarding":
                        DiscardingSession discardingSession = db.DiscardingSessions.Find(entry.SessionPK);
                        verification = (from ver in db.Verifications
                                        where ver.SessionPK == discardingSession.DiscardingSessionPK && ver.IsDiscard
                                        select ver).FirstOrDefault();
                        if (!(discardingSession.IsVerified && !verification.IsApproved))
                        {
                            result += entry.Quantity * (kindRole.Sign ? 1 : -1);
                        }
                        break;
                    case "AdjustingMinus":
                        adjustingSession = db.AdjustingSessions.Find(entry.SessionPK);
                        verification = (from ver in db.Verifications
                                        where ver.SessionPK == adjustingSession.AdjustingSessionPK && !ver.IsDiscard
                                        select ver).FirstOrDefault();
                        if (!(adjustingSession.IsVerified && !verification.IsApproved))
                        {
                            result += entry.Quantity * (kindRole.Sign ? 1 : -1);
                        }
                        break;
                    case "AdjustingPlus":
                        adjustingSession = db.AdjustingSessions.Find(entry.SessionPK);
                        verification = (from ver in db.Verifications
                                        where ver.SessionPK == adjustingSession.AdjustingSessionPK && !ver.IsDiscard
                                        select ver).FirstOrDefault();
                        if (adjustingSession.IsVerified && verification.IsApproved)
                        {
                            result += entry.Quantity * (kindRole.Sign ? 1 : -1);
                        }
                        break;
                    case "In":
                        result += entry.Quantity * (kindRole.Sign ? 1 : -1);
                        break;
                    case "Issuing":
                        result += entry.Quantity * (kindRole.Sign ? 1 : -1);
                        break;
                    case "Out":
                        result += entry.Quantity * (kindRole.Sign ? 1 : -1);
                        break;
                    case "Receiving":
                        result += entry.Quantity * (kindRole.Sign ? 1 : -1);
                        break;
                    case "Storing":
                        result += entry.Quantity * (kindRole.Sign ? 1 : -1);
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        public TransferringSession CreateTransferingSession(string boxFromID, string boxToID, string userID)
        {
            TransferringSession result;
            BoxDAO boxDAO = new BoxDAO();
            try
            {
                Box boxFrom = boxDAO.GetBoxByBoxID(boxFromID);
                Box boxTo = boxDAO.GetBoxByBoxID(boxToID);
                db.TransferringSessions.Add(new TransferringSession(boxFrom.BoxPK, boxTo.BoxPK, userID));
                db.SaveChanges();
                result = (from tSS in db.TransferringSessions.OrderByDescending(unit => unit.TransferingSessionPK)
                          select tSS).FirstOrDefault();
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void CreateInAndOutEntry(List<Client_ItemPK_TransferQuantity_IsRestored> list, StoredBox sBoxFrom, StoredBox sBoxTo, TransferringSession transferringSession)
        {
            try
            {
                foreach (var item in list)
                {
                    // Tạo out entry
                    Entry entry = new Entry(sBoxFrom, "Out", transferringSession.TransferingSessionPK,
                    item.IsRestored, item.TransferQuantity, item.ItemPK);
                    db.Entries.Add(entry);
                    // Tạo in entry
                    entry = new Entry(sBoxTo, "In", transferringSession.TransferingSessionPK,
                    item.IsRestored, item.TransferQuantity, item.ItemPK);
                    db.Entries.Add(entry);
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public void DeleteTransferingSession(int transferingSessionPK)
        {
            try
            {
                TransferringSession transferringSession = db.TransferringSessions.Find(transferingSessionPK);
                db.TransferringSessions.Remove(transferringSession);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public MovingSession CreateMovingSession(StoredBox storedBox, Shelf shelf, string userID)
        {
            try
            {
                MovingSession movingSession = new MovingSession(storedBox, shelf, userID);
                db.MovingSessions.Add(movingSession);
                db.SaveChanges();
                movingSession = (from Mss in db.MovingSessions.OrderByDescending(unit => unit.MovingSessionPK)
                                 select Mss).FirstOrDefault();
                return movingSession;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void UpdateStoredBoxShelfPK(int storedBoxPK, int shelfPK)
        {
            try
            {
                StoredBox storedBox = db.StoredBoxes.Find(storedBoxPK);
                storedBox.ShelfPK = shelfPK;
                db.Entry(storedBox).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteMovingSession(int movingSessionPK)
        {
            try
            {
                MovingSession movingSession = db.MovingSessions.Find(movingSessionPK);
                db.MovingSessions.Remove(movingSession);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public AdjustingSession CreateAdjustingSession(string comment, bool isVerified, string userID)
        {
            try
            {
                AdjustingSession adjustingSession = new AdjustingSession(comment, isVerified, userID);
                db.AdjustingSessions.Add(adjustingSession);
                db.SaveChanges();
                adjustingSession = (from Ass in db.AdjustingSessions.OrderByDescending(unit => unit.AdjustingSessionPK)
                                    select Ass).FirstOrDefault();
                return adjustingSession;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void createAdjustEntry(StoredBox sBox, int itemPK, double adjustedQuantity, bool isRestored, bool isMinus,AdjustingSession adjustingSession)
        {
            try
            {
                Entry entry;
                if (isMinus)
                {
                    entry = new Entry(sBox, "AdjustingMinus", adjustingSession.AdjustingSessionPK, isRestored, adjustedQuantity, itemPK);
                }
                else
                {
                    entry = new Entry(sBox, "AdjustingPlus", adjustingSession.AdjustingSessionPK, isRestored, adjustedQuantity, itemPK);
                }
                db.Entries.Add(entry);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteAdjustingSession(int adjustingSessionPK)
        {
            try
            {
                AdjustingSession adjustingSession = db.AdjustingSessions.Find(adjustingSessionPK);
                db.AdjustingSessions.Remove(adjustingSession);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

