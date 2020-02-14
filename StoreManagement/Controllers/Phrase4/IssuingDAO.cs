using StoreManagement.Class;
using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static StoreManagement.Controllers.IssuingController;

namespace StoreManagement.Controllers
{
    public class IssuingDAO
    {
        private UserModel db = new UserModel();

        private string KhoiNKTType(int num)
        {
            //if (num < 1 || num > 31) throw new Exception("THỜI GIAN CỦA MÁY TÍNH CÓ LỖI, CÓ THỂ HACKER TẤN CÔNG!");
            //else
            if (num < 10)
            {
                return num + "";
            }
            else
            {
                return ((char)(num + 86)) + "";
            }
        }

        public class algo_AvailableItem : IEquatable<algo_AvailableItem>
        {
            public algo_AvailableItem(int storeBoxPK, int itemPK, bool isRestored)
            {
                StoreBoxPK = storeBoxPK;
                ItemPK = itemPK;
                IsRestored = isRestored;
            }

            public int StoreBoxPK { get; set; }

            public int ItemPK { get; set; }

            public bool IsRestored { get; set; }

            public override bool Equals(object obj)
            {
                return Equals(obj as algo_AvailableItem);
            }

            public bool Equals(algo_AvailableItem other)
            {
                return other != null &&
                       StoreBoxPK == other.StoreBoxPK &&
                       ItemPK == other.ItemPK &&
                       IsRestored == other.IsRestored;
            }

            public override int GetHashCode()
            {
                var hashCode = -2037694240;
                hashCode = hashCode * -1521134295 + StoreBoxPK.GetHashCode();
                hashCode = hashCode * -1521134295 + ItemPK.GetHashCode();
                hashCode = hashCode * -1521134295 + IsRestored.GetHashCode();
                return hashCode;
            }
        }

        // InStoredQuantity là available quantity của tất cả các box
        public double InStoredQuantity(int accessoryPK)
        {
            double result = 0;
            StoringDAO storingDAO = new StoringDAO();
            try
            {
                HashSet<algo_AvailableItem> availableItems = new HashSet<algo_AvailableItem>();
                List<Entry> entries = (from e in db.Entries
                                       where e.AccessoryPK == accessoryPK
                                       select e).ToList();
                foreach (var item in entries)
                {
                    availableItems.Add(new algo_AvailableItem(item.StoredBoxPK, item.ItemPK, item.IsRestored));
                }

                foreach (var availableItem in availableItems)
                {
                    StoredBox sBox = db.StoredBoxes.Find(availableItem.StoreBoxPK);
                    double availableQuantity = new StoringDAO().AvailableQuantity(sBox, availableItem.ItemPK, availableItem.IsRestored);
                    if (availableQuantity != -1)
                    {
                        result += availableQuantity;
                    }
                }


            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }

        public void DeleteDemand(int demandPK)
        {
            try
            {
                Demand demand = db.Demands.Find(demandPK);
                db.Demands.Remove(demand);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void UpdateDemand(int demandPK)
        {
            try
            {
                Demand demand = db.Demands.Find(demandPK);
                demand.DateCreated = DateTime.Now;
                db.Entry(demand).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void UpdateDemandedItem(int demandedItemPK, double demandedQuantity, string comment)
        {
            try
            {
                DemandedItem demandedItem = db.DemandedItems.Find(demandedItemPK);
                if (!PrimitiveType.isValidQuantity(demandedQuantity) && !PrimitiveType.isValidComment(comment))
                {
                    throw new Exception(SystemMessage.NotPassPrimitiveType);
                }

                if (demandedQuantity > 0)
                {
                    demandedItem.DemandedQuantity = demandedQuantity;
                    demandedItem.Comment = comment;
                    db.Entry(demandedItem).State = EntityState.Modified;
                }
                else
                {
                    db.DemandedItems.Remove(demandedItem);
                }

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Issue> GetIssueFromDemandPKNotStorebacked(int demandPK)
        {
            try
            {
                return db.Issues.Where(unit => unit.DemandPK == demandPK && unit.IsStorebacked == false).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void CreateDemandedItems(Demand demand, List<Client_Accessory_DemandedQuantity_Comment> list, int conceptionPK)
        {
            try
            {
                foreach (var item in list)
                {
                    if (PrimitiveType.isValidQuantity(item.DemandedQuantity))
                    {
                        Accessory accessory = db.Accessories.Where(unit => unit.AccessoryID == item.AccessoryID).FirstOrDefault();

                        Conception conception = db.Conceptions.Find(conceptionPK);

                        ConceptionAccessory conceptionAccessory = (from ca in db.ConceptionAccessories
                                                                   where ca.AccessoryPK == accessory.AccessoryPK
                                                                   && ca.ConceptionPK == conception.ConceptionPK
                                                                   select ca).FirstOrDefault();
                        if (accessory == null) throw new Exception("PHỤ LIỆU " + accessory.AccessoryID + " KHÔNG TỒN TẠI!");
                        if (conception == null) throw new Exception("MÃ HÀNG " + conception.ConceptionCode + " KHÔNG TỒN TẠI!");
                        if (conceptionAccessory == null) throw new Exception("PHỤ LIỆU " + accessory.AccessoryID + " CHƯA ĐƯỢC GẮN CC!");
                        DemandedItem demandedItem = new DemandedItem(item.DemandedQuantity, item.Comment, demand.DemandPK, accessory.AccessoryPK);
                        db.DemandedItems.Add(demandedItem);
                    }
                    else
                    {
                        throw new Exception(SystemMessage.NotPassPrimitiveType);
                    }
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void SwiftDemand(int demandPK)
        {
            try
            {
                Demand demand = db.Demands.Find(demandPK);
                demand.IsOpened = !demand.IsOpened;
                db.Entry(demand).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteDemandedItem(int demandPK)
        {
            try
            {
                List<DemandedItem> demandedItems = (from dI in db.DemandedItems
                                                    where dI.DemandPK == demandPK
                                                    select dI).ToList();
                foreach (var item in demandedItems)
                {
                    db.DemandedItems.Remove(item);
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Demand CreateDemand(int conceptionPK, double totalDemand, int workplacePK, string userID)
        {
            try
            {
                DateTime now = DateTime.Now;
                string tempDay = (now.Day + "").Length == 1 ? '0' + (now.Day + "") : (now.Day + "");
                string tempMonth = (now.Month + "").Length == 1 ? '0' + (now.Month + "") : (now.Month + "");
                string tempYear = (now.Year + "").Substring((now.Year + "").Length - 2);

                string dateNow = tempDay + tempMonth + tempYear;
                string demandID = "";
                Demand demand = (from acc in db.Demands.OrderByDescending(unit => unit.DemandPK)
                                 select acc).FirstOrDefault();
                if (demand == null || !demand.DemandID.Contains(dateNow))
                    demandID = "AST-PCP-" + dateNow + "01";
                else
                {
                    int length = demand.DemandID.Length;
                    string tempStr = (Int32.Parse(demand.DemandID.Substring(length - 2, 2)) + 1) + "";
                    if (tempStr.Length == 1) tempStr = "0" + tempStr;
                    demandID = demand.DemandID.Substring(0, length - 2) + tempStr;
                }
                // create demand

                demand = new Demand(demandID, totalDemand, conceptionPK, workplacePK, userID);
                db.Demands.Add(demand);
                db.SaveChanges();
                demand = (from de in db.Demands.OrderByDescending(unit => unit.DemandPK)
                          select de).FirstOrDefault();
                return demand;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Conception GetConceptionByConceptionCode(string conceptionCode)
        {
            try
            {
                Conception conception = (from c in db.Conceptions
                                         where c.ConceptionCode == conceptionCode
                                         select c).FirstOrDefault();
                return conception;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public object GetDemandByDemandID(string demandID)
        {
            try
            {
                Demand demand = (from de in db.Demands
                                 where de.DemandID == demandID
                                 select de).FirstOrDefault();
                return demand;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public class StoredBox_ItemPK_IsRestored : IEquatable<StoredBox_ItemPK_IsRestored>
        {
            public StoredBox_ItemPK_IsRestored()
            {
            }

            public StoredBox_ItemPK_IsRestored(int storedBoxPK, int itemPK, bool isRestored)
            {
                StoredBoxPK = storedBoxPK;
                ItemPK = itemPK;
                IsRestored = isRestored;
            }

            public int StoredBoxPK { get; set; }

            public int ItemPK { get; set; }

            public bool IsRestored { get; set; }

            public override bool Equals(object obj)
            {
                return Equals(obj as StoredBox_ItemPK_IsRestored);
            }

            public bool Equals(StoredBox_ItemPK_IsRestored other)
            {
                return other != null &&
                       StoredBoxPK == other.StoredBoxPK &&
                       ItemPK == other.ItemPK &&
                       IsRestored == other.IsRestored;
            }

            public override int GetHashCode()
            {
                var hashCode = -1297919728;
                hashCode = hashCode * -1521134295 + StoredBoxPK.GetHashCode();
                hashCode = hashCode * -1521134295 + ItemPK.GetHashCode();
                hashCode = hashCode * -1521134295 + IsRestored.GetHashCode();
                return hashCode;
            }

        }

        public class InBoxQuantity_AvailableQuantity
        {
            public InBoxQuantity_AvailableQuantity()
            {
            }

            public InBoxQuantity_AvailableQuantity(double inBoxQuantity, double availableQuantity)
            {
                InBoxQuantity = inBoxQuantity;
                AvailableQuantity = availableQuantity;
            }

            public double InBoxQuantity { get; set; }

            public double AvailableQuantity { get; set; }
        }

        public StorebackSession CreateStorebackSession(int issuePK, string userID)
        {
            try
            {
                StorebackSession storebackSession = new StorebackSession(userID, issuePK);
                db.StorebackSessions.Add(storebackSession);
                db.SaveChanges();
                return db.StorebackSessions.OrderByDescending(unit => unit.StorebackSessionPK).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void CreateEntryAndUpdateIssueThings(int issuePK, StorebackSession storebackSession, List<Client_Box_Shelf_Storeback> input)
        {
            try
            {
                // update issue
                Issue issue = db.Issues.Find(issuePK);
                issue.IsStorebacked = true;
                db.Entry(issue).State = EntityState.Modified;

                List<IssuedGroup> issuedGroups = db.IssuedGroups.Where(unit => unit.IssuePK == issue.IssuePK).ToList();
                foreach (var issuedGroup in issuedGroups)
                {
                    UnstoredBox uBox = db.UnstoredBoxes.Find(issuedGroup.UnstoredBoxPK);
                    Box box = db.Boxes.Find(uBox.BoxPK);
                    StoredBox storedBox = db.StoredBoxes.Where(unit => unit.BoxPK == box.BoxPK).FirstOrDefault();

                    // update storedbox shelfpk
                    var temp = input.Where(unit => unit.BoxID == box.BoxID).FirstOrDefault();
                    Shelf shelf = db.Shelves.Where(unit => unit.ShelfID == temp.ShelfID).FirstOrDefault();
                    storedBox.ShelfPK = shelf.ShelfPK;
                    db.Entry(storedBox).State = EntityState.Modified;

                    // update issued group
                    issuedGroup.UnstoredBoxPK = null;
                    db.Entry(issuedGroup).State = EntityState.Modified;

                    // create entries
                    Accessory accessory = db.Accessories.Find(issuedGroup.AccessoryPK);
                    Entry entry = new Entry(storedBox, "Storeback", storebackSession.StorebackSessionPK,
                        issuedGroup.IsRestored, issuedGroup.IssuedGroupQuantity, issuedGroup.ItemPK, accessory);
                    db.Entries.Add(entry);
                }

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void CreateConfirmingSessionAndUpdateIssueThings(string userID, int issuePK, List<string> takenAwayBoxIDs)
        {
            try
            {
                // update issue
                Issue issue = db.Issues.Find(issuePK);
                issue.IsConfirmed = true;
                db.Entry(issue).State = EntityState.Modified;

                // update issuegroup
                List<IssuedGroup> issuedGroups = db.IssuedGroups.Where(unit => unit.IssuePK == issuePK).ToList();
                foreach (var item in issuedGroups)
                {
                    item.UnstoredBoxPK = null;
                    db.Entry(item).State = EntityState.Modified;
                }

                // update takenawaybox
                foreach (var item in takenAwayBoxIDs)
                {
                    Box box = db.Boxes.Where(unit => unit.BoxID == item).FirstOrDefault();
                    box.IsActive = false;
                    db.Entry(box).State = EntityState.Modified;
                }

                // create confirmingsession
                ConfirmingSession confirmingSession = new ConfirmingSession(userID, issuePK);
                db.ConfirmingSessions.Add(confirmingSession);

                db.SaveChanges();
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        //public List<Client_Box_Shelf_Row> StoredBox_ItemPK_IsRestoredOfEntries(Accessory accessory)
        //{
        //    List<Client_Box_Shelf_Row> result = new List<Client_Box_Shelf_Row>();
        //    StoringDAO storingDAO = new StoringDAO();
        //    try
        //    {
        //        // cực phẩm IQ
        //        double inStoredQuantity = InStoredQuantity(accessory.AccessoryPK);
        //        if (inStoredQuantity == 0) throw new Exception("HÀNG TRONG KHO ĐÃ HẾT!");
        //        List<Entry> entries = (from e in db.Entries
        //                               where e.AccessoryPK == accessory.AccessoryPK
        //                               select e).ToList();
        //        //Entry entry = entries[0];
        //        // kết xuất 1 dictionary gồm key là itempk và isrestored tuy nhiên gộp số lượng giữa các box
        //        Dictionary<StoredBox_ItemPK_IsRestored, InBoxQuantity_AvailableQuantity> tempDictionary = new Dictionary<StoredBox_ItemPK_IsRestored, InBoxQuantity_AvailableQuantity>();
        //        foreach (var entry in entries)
        //        {
        //            StoredBox storedBox = db.StoredBoxes.Find(entry.StoredBoxPK);
        //            Box box = db.Boxes.Find(storedBox.BoxPK);

        //            PassedItem passedItem;
        //            RestoredItem restoredItem;
        //            StoredBox_ItemPK_IsRestored key;

        //            if (entry.IsRestored)
        //            {
        //                restoredItem = db.RestoredItems.Find(entry.ItemPK);
        //                key = new StoredBox_ItemPK_IsRestored(storedBox.StoredBoxPK, restoredItem.RestoredItemPK, entry.IsRestored);
        //            }
        //            else
        //            {
        //                passedItem = db.PassedItems.Find(entry.ItemPK);
        //                key = new StoredBox_ItemPK_IsRestored(storedBox.StoredBoxPK, passedItem.PassedItemPK, entry.IsRestored);
        //            }
        //            if (box.IsActive)
        //            {
        //                //InBoxQuantity_AvailableQuantity tmp = new InBoxQuantity_AvailableQuantity(
        //                //    storingDAO.AvailableQuantity(storedBox, entry.ItemPK, entry.IsRestored)
        //                //    , storingDAO.AvailableQuantity(storedBox, entry.ItemPK, entry.IsRestored));
        //                if (!tempDictionary.ContainsKey(key))
        //                {
        //                    //tempDictionary.Add(key, tmp);
        //                    tempDictionary.Add(key, new InBoxQuantity_AvailableQuantity(0, 0));
        //                }
        //                //else
        //                //{
        //                //    tempDictionary[key].InBoxQuantity += tmp.InBoxQuantity;
        //                //    tempDictionary[key].AvailableQuantity += tmp.AvailableQuantity;
        //                //}
        //            }
        //        }
        //        foreach (var dictionary in tempDictionary)
        //        {
        //            StoredBox sBox = db.StoredBoxes.Find(dictionary.Key.StoredBoxPK);
        //            InBoxQuantity_AvailableQuantity tmp = new InBoxQuantity_AvailableQuantity(
        //                storingDAO.AvailableQuantity(sBox, dictionary.Key.ItemPK, dictionary.Key.IsRestored)
        //                , storingDAO.AvailableQuantity(sBox, dictionary.Key.ItemPK, dictionary.Key.IsRestored));

        //            dictionary.Value.InBoxQuantity += tmp.InBoxQuantity;
        //            dictionary.Value.AvailableQuantity += tmp.AvailableQuantity;
        //        }

        //        // kiếm từng box dựa trên item dò được
        //        foreach (var item in tempDictionary)
        //        {
        //            if (item.Value.AvailableQuantity > 0)
        //            {
        //                StoredBox storedBox = db.StoredBoxes.Find(item.Key.StoredBoxPK);
        //                Box box = db.Boxes.Find(storedBox.BoxPK);
        //                Shelf shelf = db.Shelves.Find(storedBox.ShelfPK);
        //                Row row = db.Rows.Find(shelf.RowPK);
        //                if (item.Key.IsRestored)
        //                {
        //                    RestoredItem restoredItem = db.RestoredItems.Find(item.Key.ItemPK);
        //                    Restoration restoration = db.Restorations.Find(restoredItem.RestorationPK);
        //                    result.Add(new Client_Box_Shelf_Row(box.BoxID, storedBox.StoredBoxPK, shelf.ShelfID, row.RowID, item.Key.ItemPK, item.Key.IsRestored, item.Value.InBoxQuantity, restoration.RestorationID, item.Value.AvailableQuantity));
        //                }
        //                else
        //                {
        //                    PassedItem passedItem = db.PassedItems.Find(item.Key.ItemPK);
        //                    ClassifiedItem classifiedItem = db.ClassifiedItems.Find(passedItem.ClassifiedItemPK);
        //                    PackedItem packedItem = db.PackedItems.Find(classifiedItem.PackedItemPK);
        //                    Pack pack = db.Packs.Find(packedItem.PackPK);
        //                    result.Add(new Client_Box_Shelf_Row(box.BoxID, storedBox.StoredBoxPK, shelf.ShelfID, row.RowID, item.Key.ItemPK, item.Key.IsRestored, item.Value.InBoxQuantity, pack.PackID, item.Value.AvailableQuantity));
        //                }

        //            }
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //    return result;
        //}

        public List<Client_Box_Shelf_Row> StoredBox_ItemPK_IsRestoredOfEntries(Accessory accessory)
        {
            List<Client_Box_Shelf_Row> result = new List<Client_Box_Shelf_Row>();
            StoringDAO storingDAO = new StoringDAO();
            try
            {
                // cực phẩm IQ
                double inStoredQuantity = InStoredQuantity(accessory.AccessoryPK);
                //if (inStoredQuantity == 0) throw new Exception("HÀNG TRONG KHO ĐÃ HẾT!");
                List<Entry> entries = (from e in db.Entries
                                       where e.AccessoryPK == accessory.AccessoryPK
                                       select e).ToList();
                //Entry entry = entries[0];
                // kết xuất 1 dictionary gồm key là itempk và isrestored tuy nhiên gộp số lượng giữa các box
                Dictionary<StoredBox_ItemPK_IsRestored, InBoxQuantity_AvailableQuantity> tempDictionary = new Dictionary<StoredBox_ItemPK_IsRestored, InBoxQuantity_AvailableQuantity>();
                foreach (var entry in entries)
                {
                    StoredBox storedBox = db.StoredBoxes.Find(entry.StoredBoxPK);
                    Box box = db.Boxes.Find(storedBox.BoxPK);

                    PassedItem passedItem;
                    RestoredItem restoredItem;
                    StoredBox_ItemPK_IsRestored key;

                    if (entry.IsRestored)
                    {
                        restoredItem = db.RestoredItems.Find(entry.ItemPK);
                        key = new StoredBox_ItemPK_IsRestored(storedBox.StoredBoxPK, restoredItem.RestoredItemPK, entry.IsRestored);
                    }
                    else
                    {
                        passedItem = db.PassedItems.Find(entry.ItemPK);
                        key = new StoredBox_ItemPK_IsRestored(storedBox.StoredBoxPK, passedItem.PassedItemPK, entry.IsRestored);
                    }
                    if (box.IsActive)
                    {
                        //InBoxQuantity_AvailableQuantity tmp = new InBoxQuantity_AvailableQuantity(
                        //    storingDAO.AvailableQuantity(storedBox, entry.ItemPK, entry.IsRestored)
                        //    , storingDAO.AvailableQuantity(storedBox, entry.ItemPK, entry.IsRestored));
                        if (!tempDictionary.ContainsKey(key))
                        {
                            //tempDictionary.Add(key, tmp);
                            tempDictionary.Add(key, new InBoxQuantity_AvailableQuantity(0, 0));
                        }
                        //else
                        //{
                        //    tempDictionary[key].InBoxQuantity += tmp.InBoxQuantity;
                        //    tempDictionary[key].AvailableQuantity += tmp.AvailableQuantity;
                        //}
                    }
                }
                foreach (var dictionary in tempDictionary)
                {
                    StoredBox sBox = db.StoredBoxes.Find(dictionary.Key.StoredBoxPK);
                    InBoxQuantity_AvailableQuantity tmp = new InBoxQuantity_AvailableQuantity(
                        storingDAO.AvailableQuantity(sBox, dictionary.Key.ItemPK, dictionary.Key.IsRestored)
                        , storingDAO.AvailableQuantity(sBox, dictionary.Key.ItemPK, dictionary.Key.IsRestored));

                    dictionary.Value.InBoxQuantity += tmp.InBoxQuantity;
                    dictionary.Value.AvailableQuantity += tmp.AvailableQuantity;
                }

                // kiếm từng box dựa trên item dò được
                foreach (var item in tempDictionary)
                {
                    if (item.Value.AvailableQuantity > 0)
                    {
                        StoredBox storedBox = db.StoredBoxes.Find(item.Key.StoredBoxPK);
                        Box box = db.Boxes.Find(storedBox.BoxPK);
                        Shelf shelf = db.Shelves.Find(storedBox.ShelfPK);
                        Row row = db.Rows.Find(shelf.RowPK);
                        if (item.Key.IsRestored)
                        {
                            RestoredItem restoredItem = db.RestoredItems.Find(item.Key.ItemPK);
                            Restoration restoration = db.Restorations.Find(restoredItem.RestorationPK);
                            result.Add(new Client_Box_Shelf_Row(box.BoxID, storedBox.StoredBoxPK, shelf.ShelfID, row.RowID, item.Key.ItemPK, item.Key.IsRestored, restoration.RestorationID, item.Value.AvailableQuantity));
                        }
                        else
                        {
                            PassedItem passedItem = db.PassedItems.Find(item.Key.ItemPK);
                            ClassifiedItem classifiedItem = db.ClassifiedItems.Find(passedItem.ClassifiedItemPK);
                            PackedItem packedItem = db.PackedItems.Find(classifiedItem.PackedItemPK);
                            Pack pack = db.Packs.Find(packedItem.PackPK);
                            result.Add(new Client_Box_Shelf_Row(box.BoxID, storedBox.StoredBoxPK, shelf.ShelfID, row.RowID, item.Key.ItemPK, item.Key.IsRestored, pack.PackID, item.Value.AvailableQuantity));
                        }

                    }
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }

        //public IssuingSession CreateIssuingSession(string userID, int requestPK, List<string> boxIDs)
        //{
        //    try
        //    {
        //        string deactivatedBoxes = "";
        //        foreach (var boxID in boxIDs)
        //        {
        //            deactivatedBoxes += boxID + "~!~";
        //        }
        //        IssuingSession issuingSession = new IssuingSession(userID, requestPK, deactivatedBoxes);
        //        db.IssuingSessions.Add(issuingSession);
        //        db.SaveChanges();
        //        issuingSession = (from iS in db.IssuingSessions.OrderByDescending(unit => unit.IssuingSessionPK)
        //                          select iS).FirstOrDefault();
        //        return issuingSession;
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        //public void DeleteIssuingSession(int issuingSessionPK)
        //{
        //    try
        //    {
        //        IssuingSession issuingSession = db.IssuingSessions.Find(issuingSessionPK);
        //        db.IssuingSessions.Remove(issuingSession);
        //        db.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        //public void DeleteIssueEntries(int issuingSessionPK)
        //{
        //    try
        //    {
        //        IssuingSession issuingSession = db.IssuingSessions.Find(issuingSessionPK);
        //        List<Entry> entries = (from e in db.Entries
        //                               where e.KindRoleName == "Issuing" && e.SessionPK == issuingSessionPK
        //                               select e).ToList();
        //        foreach (var entry in entries)
        //        {
        //            db.Entries.Remove(entry);
        //        }
        //        db.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        public Restoration CreateRestoration(string userID, string comment)
        {
            try
            {
                // Add restoration
                DateTime now = DateTime.Now;
                // Generate Restoration
                //string restorationID = KhoiNKTType(now.Second) + KhoiNKTType(now.Minute) + KhoiNKTType(now.Hour) + KhoiNKTType(now.Day) + KhoiNKTType(now.Month) + now.Year;
                Restoration temp = db.Restorations.OrderByDescending(unit => unit.RestorationPK).FirstOrDefault();

                string restorationID;
                if (temp != null)
                {
                    string tempStr;
                    Int32 tempInt;
                    tempStr = temp.RestorationID.Substring(temp.RestorationID.Length - 5);
                    tempInt = Int32.Parse(tempStr) + 1;

                    tempStr = tempInt + "";
                    if (tempStr.Length == 1) tempStr = "0000" + tempStr;
                    if (tempStr.Length == 2) tempStr = "000" + tempStr;
                    if (tempStr.Length == 3) tempStr = "00" + tempStr;
                    if (tempStr.Length == 4) tempStr = "0" + tempStr;

                    restorationID = "AST-PT-" + tempStr;
                }
                else
                {
                    restorationID = "AST-PT-00001";
                }
                Restoration restoration = new Restoration(restorationID, userID, comment);
                db.Restorations.Add(restoration);
                db.SaveChanges();

                //
                restoration = (from res in db.Restorations.OrderByDescending(unit => unit.RestorationPK)
                               select res).FirstOrDefault();
                return restoration;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void CreateRestoredItems(Restoration restoration, List<IssuingController.Client_AccessoryPK_RestoredQuantity> list)
        {
            try
            {
                // Add restoredItems
                foreach (var item in list)
                {
                    RestoredItem restoredItem = new RestoredItem(item.AssessoryPK, item.RestoredQuantity, restoration.RestorationPK);
                    db.RestoredItems.Add(restoredItem);
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteRestoration(int restorationPK)
        {
            try
            {
                // Remove restoration
                Restoration restoration = db.Restorations.Find(restorationPK);
                db.Restorations.Remove(restoration);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void UpdateRestoration(int restorationPK, bool isReceived)
        {
            try
            {
                // update restoration
                Restoration restoration = db.Restorations.Find(restorationPK);
                restoration.IsReceived = isReceived;
                db.Entry(restoration).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void UpdateRestoration(int restorationPK, string comment)
        {
            try
            {
                // update restoration
                Restoration restoration = db.Restorations.Find(restorationPK);
                restoration.DateCreated = DateTime.Now;
                restoration.Comment = comment;
                db.Entry(restoration).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void UpdateRestoredItems(List<IssuingController.Client_RestoredItemPK_RestoredQuantity> list)
        {
            try
            {
                // update restoredItems
                foreach (var item in list)
                {
                    RestoredItem restoredItem = db.RestoredItems.Find(item.RestoredItemPK);
                    if (item.RestoredQuantity > 0)
                    {
                        restoredItem.RestoredQuantity = item.RestoredQuantity;
                        db.Entry(restoredItem).State = EntityState.Modified;
                    }
                    else
                    {
                        db.RestoredItems.Remove(restoredItem);
                    }
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteRestoredItems(int restorationPK)
        {
            try
            {
                List<RestoredItem> restoredItems = (from rI in db.RestoredItems
                                                    where rI.RestorationPK == restorationPK
                                                    select rI).ToList();

                // delete restoredItems
                foreach (var item in restoredItems)
                {
                    db.RestoredItems.Remove(item);
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ReceivingSession CreateReceivingSession(int restorationPK, string userID)
        {
            try
            {
                ReceivingSession receivingSession = new ReceivingSession(userID, restorationPK);
                db.ReceivingSessions.Add(receivingSession);
                db.SaveChanges();
                receivingSession = (from Rss in db.ReceivingSessions.OrderByDescending(unit => unit.ReceivingSessionPK)
                                    select Rss).FirstOrDefault();
                return receivingSession;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void CreateEntryReceiving(List<IssuingController.Client_Box_List> list, ReceivingSession receivingSession)
        {
            BoxDAO boxDAO = new BoxDAO();
            try
            {
                Dictionary<int, double> mapRestoredItems = new Dictionary<int, double>();
                foreach (var items in list)
                {
                    Box box = boxDAO.GetBoxByBoxID(items.BoxID);
                    StoredBox sBox = boxDAO.GetStoredBoxbyBoxPK(box.BoxPK);
                    foreach (var item in items.ListItem)
                    {
                        RestoredItem restoredItem = db.RestoredItems.Find(item.RestoredItemPK);
                        if (!mapRestoredItems.ContainsKey(restoredItem.RestoredItemPK))
                        {
                            mapRestoredItems.Add(restoredItem.RestoredItemPK, item.PlacedQuantity);
                        }
                        else
                        {
                            mapRestoredItems[restoredItem.RestoredItemPK] += item.PlacedQuantity;
                        }
                        Accessory accessory = db.Accessories.Find(restoredItem.AccessoryPK);
                        Entry entry = new Entry(sBox, "Receiving", receivingSession.ReceivingSessionPK, true, item.PlacedQuantity, item.RestoredItemPK, accessory);
                        db.Entries.Add(entry);
                    }
                }
                foreach (var item in mapRestoredItems)
                {
                    RestoredItem restoredItem = db.RestoredItems.Find(item.Key);
                    if (item.Value != restoredItem.RestoredQuantity) throw new Exception("TỔNG HÀNG LƯU KHO KHÔNG GIỐNG HÀNG ĐƯỢC TRẢ!");
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public double IssuedQuantity(int demandedItemPK)
        {
            double result = 0;
            List<IssuedGroup> issuedGroups = db.IssuedGroups.Where(unit => unit.DemandedItemPK == demandedItemPK).ToList();
            foreach (var item in issuedGroups)
            {
                Issue issue = db.Issues.Find(item.IssuePK);
                if (issue.IsStorebacked == false)
                {
                    result += item.IssuedGroupQuantity;
                }
            }
            return result;
        }

        public Issue CreateIssue(string userID, int demandPK)
        {
            try
            {
                // generate IssueID
                string issueID = db.Demands.Find(demandPK).DemandID;
                int issuePos = db.Issues.Where(unit => unit.DemandPK == demandPK).Count() + 1;
                if (issuePos < 10) issueID += "0" + issuePos;
                else issueID += issuePos;

                Issue issue = new Issue(demandPK, userID, issueID);
                db.Issues.Add(issue);
                db.SaveChanges();
                return db.Issues.OrderByDescending(unit => unit.IssuePK).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteIssue(int issuePK)
        {
            try
            {
                Issue issue = db.Issues.Find(issuePK);
                db.Issues.Remove(issue);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void CreateEntryAndIssuedGroup(List<IssuingController.StoredItemForIssue> input, int demandPK, Issue issue)
        {
            try
            {
                List<IssuedGroup> igList = new List<IssuedGroup>();
                foreach (var item in input)
                {
                    Box box = new BoxDAO().GetBoxByBoxID(item.OldBoxID);
                    StoredBox sBox = new BoxDAO().GetStoredBoxbyBoxPK(box.BoxPK);
                    UnstoredBox uBox = new BoxDAO().GetUnstoredBoxbyBoxPK(box.BoxPK);
                    Accessory accessory = db.Accessories.Find(item.AccessoryPK);
                    Entry entry = new Entry(sBox, "Issuing", issue.IssuePK, item.IsRestored, item.IssuedQuantity, item.ItemPK, accessory);
                    db.Entries.Add(entry);

                    DemandedItem demandedItem = db.DemandedItems.Where(unit => unit.AccessoryPK == accessory.AccessoryPK &&
                                                   unit.DemandPK == demandPK).FirstOrDefault();
                    IssuedGroup issuedGroup = new IssuedGroup(item.IssuedQuantity, issue.IssuePK, demandedItem.DemandedItemPK
                        , uBox.UnstoredBoxPK, item.ItemPK, item.IsRestored, accessory.AccessoryPK);
                    if (!igList.Contains(issuedGroup))
                    {
                        igList.Add(issuedGroup);
                    }
                    else
                    {
                        igList.Find(x => x.Equals(issuedGroup)).IssuedGroupQuantity += issuedGroup.IssuedGroupQuantity;
                    }
                }
                foreach (var item in igList)
                {
                    db.IssuedGroups.Add(item);
                }
                db.SaveChanges();

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void CreateRestoredGroup(int restorationPK, string userID, ReceivingSession receivingSession, List<IssuingController.RestoredGroupItem> input)
        {
            try
            {
                List<RestoredGroup> temp = new List<RestoredGroup>();
                foreach (var item in input)
                {
                    UnstoredBox uBox = db.UnstoredBoxes.Find(item.UnstoredBoxPK);
                    Box box = db.Boxes.Find(uBox.BoxPK);
                    StoredBox sBox = new BoxDAO().GetStoredBoxbyBoxPK(box.BoxPK);
                    if (!new BoxDAO().IsUnstoredCase(box.BoxPK))
                    {
                        throw new Exception("ĐƠN VỊ KHÔNG HỌP LỆ!");
                    }
                    RestoredGroup restoredGroup = new RestoredGroup(item.GroupQuantity, item.RestoredItemPK, item.UnstoredBoxPK);
                    db.RestoredGroups.Add(restoredGroup);

                    sBox.ShelfPK = null;
                    db.Entry(sBox).State = EntityState.Modified;

                    if (!temp.Contains(restoredGroup))
                    {
                        temp.Add(restoredGroup);
                    }
                    else
                    {
                        temp.Find(x => x.Equals(restoredGroup)).GroupQuantity += restoredGroup.GroupQuantity;
                    }
                }

                foreach (var item in temp)
                {
                    RestoredItem restoredItem = db.RestoredItems.Find(item.RestoredItemPK);
                    if (item.GroupQuantity != restoredItem.RestoredQuantity)
                    {
                        throw new Exception("SỐ LƯỢNG GHI NHẬN KHÔNG GIỐNG VỚI ĐƠN TRẢ");
                    }
                }

                Restoration restoration = db.Restorations.Find(restorationPK);
                restoration.IsReceived = true;
                db.Entry(restoration).State = EntityState.Modified;

                db.SaveChanges();

            }
            catch (Exception e)
            {
                db.Dispose();
                db = new UserModel();
                throw e;
            }
        }

        public void DeleteReceivingSession(int receivingSessionPK)
        {
            try
            {
                ReceivingSession receivingSession = db.ReceivingSessions.Find(receivingSessionPK);
                db.ReceivingSessions.Remove(receivingSession);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

