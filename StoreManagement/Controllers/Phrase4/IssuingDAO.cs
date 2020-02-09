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

        // InStoredQuantity là available quantity của tất cả các box
        public double InStoredQuantity(int accessoryPK)
        {
            double result = 0;
            StoringDAO storingDAO = new StoringDAO();
            try
            {
                //List<Entry> entries = (from e in db.Entries
                //                       where e.AccessoryPK == accessoryPK
                //                       select e).ToList();

                //Entry entry = entries[0];

                Entry entry = db.Entries.Where(e => e.AccessoryPK == accessoryPK).FirstOrDefault();
                if (entry != null)
                {
                    StoredBox sBox = db.StoredBoxes.Find(entry.StoredBoxPK);
                    result += storingDAO.AvailableQuantity(sBox, entry.ItemPK, entry.IsRestored);
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

        public void CreateDemandedItems(Demand demand, List<Client_Accessory_DemandedQuantity_Comment> list, int conceptionPK)
        {
            try
            {
                foreach (var item in list)
                {
                    if (PrimitiveType.isValidQuantity(item.DemandedQuantity))
                    {
                        Accessory accessory = (from a in db.Accessories
                                               where a.AccessoryID == item.AccessoryID
                                               select a).FirstOrDefault();

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

        public Demand CreateDemand(int customerPK, int conceptionPK, double totalDemand, int workplacePK, string userID)
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
                if (inStoredQuantity == 0) throw new Exception("HÀNG TRONG KHO ĐÃ HẾT!");
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
                            result.Add(new Client_Box_Shelf_Row(box.BoxID, storedBox.StoredBoxPK, shelf.ShelfID, row.RowID, item.Key.ItemPK, item.Key.IsRestored,pack.PackID, item.Value.AvailableQuantity));
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

                    restorationID = "AST-PT" + tempStr;
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
                    restoredItem.RestoredQuantity = item.RestoredQuantity;
                    db.Entry(restoredItem).State = EntityState.Modified;
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
    }
}

