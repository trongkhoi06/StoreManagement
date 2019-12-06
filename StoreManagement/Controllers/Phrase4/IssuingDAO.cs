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

        public double InStoredQuantity(int accessoryPK)
        {
            double result = 0;
            StoringDAO storingDAO = new StoringDAO();
            try
            {
                //List<PassedItem> passedItems = new List<PassedItem>();
                //Accessory accessory = db.Accessories.Find(accessoryPK);
                //// lấy restoredItems
                //List<RestoredItem> restoredItems = (from rI in db.RestoredItems
                //                                    where rI.AccessoryPK == accessory.AccessoryPK
                //                                    select rI).ToList();
                //// lấy passedItems
                //List<OrderedItem> orderedItems = (from oI in db.OrderedItems
                //                                  where oI.AccessoryPK == accessory.AccessoryPK
                //                                  select oI).ToList();
                //List<List<PackedItem>> packedItemss = new List<List<PackedItem>>();
                //foreach (var orderedItem in orderedItems)
                //{
                //    List<PackedItem> packedItems = (from pI in db.PackedItems
                //                                    where pI.OrderedItemPK == orderedItem.OrderedItemPK
                //                                    select pI).ToList();
                //    if (packedItems.Count > 0) packedItemss.Add(packedItems);
                //}
                //if (packedItemss.Count > 0)
                //{
                //    foreach (var packedItems in packedItemss)
                //    {
                //        foreach (var packedItem in packedItems)
                //        {
                //            ClassifiedItem classifiedItem = (from cI in db.ClassifiedItems
                //                                             where cI.PackedItemPK == packedItem.PackedItemPK
                //                                             select cI).FirstOrDefault();
                //            if (classifiedItem != null && classifiedItem.QualityState == 2)
                //            {
                //                PassedItem passedItem = (from p)
                //            }
                //        }
                //    }
                //}

                List<Entry> entries = (from e in db.Entries
                                       where e.AccessoryPK == accessoryPK
                                       select e).ToList();
                result = storingDAO.EntriesQuantity(entries);
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

        public void CreateDemandedItems(Demand demand, List<Client_Accessory_DemandedQuantity_Comment> list, string conceptionCode)
        {
            try
            {
                foreach (var item in list)
                {
                    Accessory accessory = (from a in db.Accessories
                                           where a.AccessoryID == item.AccessoryID
                                           select a).FirstOrDefault();

                    Conception conception = db.Conceptions.Find(GetConceptionByConceptionCode(conceptionCode));

                    ConceptionAccessory conceptionAccessory = (from ca in db.ConceptionAccessories
                                                               where ca.AccessoryPK == accessory.AccessoryPK
                                                               && ca.ConceptionPK == conception.ConceptionPK
                                                               select ca).FirstOrDefault();
                    if (accessory == null) throw new Exception("PHỤ LIỆU " + accessory.AccessoryID + " KHÔNG TỒN TẠI!");
                    if (conceptionAccessory == null) throw new Exception("PHỤ LIỆU " + accessory.AccessoryID + " CHƯA ĐƯỢC GẮN CC!");
                    DemandedItem demandedItem = new DemandedItem(item.DemandedQuantity, item.Comment, demand.DemandPK, accessory.AccessoryPK);
                    db.DemandedItems.Add(demandedItem);
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Demand CreateDemand(int customerPK, string demandID, string conceptionCode, int startWeek, int endWeek, double totalDemand, string receiveDevision, string userID)
        {
            try
            {
                Demand demand = new Demand(demandID, startWeek, endWeek, totalDemand, customerPK, receiveDevision, userID);
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

        public double InRequestedQuantity(int accessoryPK)
        {
            double result = 0;
            StoringDAO storingDAO = new StoringDAO();
            try
            {
                List<DemandedItem> demandedItems = (from dI in db.DemandedItems
                                                    where dI.AccessoryPK == accessoryPK
                                                    select dI).ToList();
                foreach (var demandedItem in demandedItems)
                {
                    List<RequestedItem> requestedItems = (from rI in db.RequestedItems
                                                          where rI.DemandedItemPK == demandedItem.DemandedItemPK
                                                          select rI).ToList();
                    foreach (var requestedItem in requestedItems)
                    {
                        Request request = db.Requests.Find(requestedItem.RequestPK);
                        if (request.IsIssued == false)
                        {
                            result += requestedItem.RequestedQuantity;
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

        public double TotalRequestedQuantity(List<RequestedItem> requestedItems)
        {
            double result = 0;
            foreach (var requestedItem in requestedItems)
            {
                result += requestedItem.RequestedQuantity;
            }
            return result;
        }

        public double TotalRequestedQuantityConfirmed(List<RequestedItem> requestedItems)
        {
            double result = 0;
            foreach (var requestedItem in requestedItems)
            {
                Request request = db.Requests.Find(requestedItem.RequestPK);
                if (request.IsConfirmed)
                {
                    result += requestedItem.RequestedQuantity;
                }
            }
            return result;
        }

        public void CreateRequestedItems(List<Client_DemandedItemPK_RequestedQuantity> list, int requestPK)
        {
            try
            {
                foreach (var item in list)
                {
                    RequestedItem requestedItem = new RequestedItem(item.RequestedQuantity, requestPK, item.DemandedItemPK);
                    db.RequestedItems.Add(requestedItem);
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Request CreateRequest(string requestID, DateTime expectedDate, bool isIssued, bool isConfirmed, string comment, int demandPK, string userID)
        {
            try
            {
                Request request = new Request(requestID, expectedDate, isIssued, isConfirmed, comment, demandPK, userID);
                db.Requests.Add(request);
                db.SaveChanges();
                request = (from rq in db.Requests.OrderByDescending(unit => unit.RequestPK)
                           select rq).FirstOrDefault();
                return request;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public void UpdateRequest(int requestPK, string comment, DateTime expectedDate)
        {
            try
            {
                Request request = db.Requests.Find(requestPK);
                request.DateCreated = DateTime.Now;
                request.Comment = comment;
                request.ExpectedDate = expectedDate;
                db.Entry(request).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void UpdateRequest(int requestPK, bool isIssued)
        {
            try
            {
                Request request = db.Requests.Find(requestPK);
                request.IsIssued = isIssued;
                db.Entry(request).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void ConfirmRequest(int requestPK, bool isConfirmed)
        {
            try
            {
                Request request = db.Requests.Find(requestPK);
                request.IsConfirmed = isConfirmed;
                db.Entry(request).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void UpdateRequestedItems(List<Client_RequestedItemPK_RequestedQuantity> list, int requestPK)
        {
            try
            {
                foreach (var item in list)
                {
                    RequestedItem requestedItem = db.RequestedItems.Find(item.RequestedItemPK);
                    requestedItem.RequestedQuantity = item.RequestedQuantity;
                    db.Entry(requestedItem).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteRequestedItems(int requestPK)
        {
            try
            {
                List<RequestedItem> requestedItems = (from rI in db.RequestedItems
                                                      where rI.RequestPK == requestPK
                                                      select rI).ToList();
                foreach (var requestedItem in requestedItems)
                {
                    db.RequestedItems.Remove(requestedItem);
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteRequest(int requestPK)
        {
            try
            {
                Request request = db.Requests.Find(requestPK);
                db.Requests.Remove(request);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public double OtherRequestedItem(int demandedItemPK, int requestedItemPK)
        {
            double result = 0;
            try
            {
                List<RequestedItem> requestedItems = (from rI in db.RequestedItems
                                                      where rI.DemandedItemPK == demandedItemPK && rI.RequestedItemPK != requestedItemPK
                                                      select rI).ToList();
                foreach (var requestedItem in requestedItems)
                {
                    result += requestedItem.RequestedQuantity;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }

        public class StoredBox_ItemPK_IsRestored
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
        }
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
                Dictionary<StoredBox_ItemPK_IsRestored, double> tempDictionary = new Dictionary<StoredBox_ItemPK_IsRestored, double>();
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

                        if (!tempDictionary.ContainsKey(key))
                        {
                            tempDictionary.Add(key, storingDAO.EntryQuantity(entry));
                        }
                        else
                        {
                            tempDictionary[key] += storingDAO.EntryQuantity(entry);
                        }
                    }
                }

                foreach (var item in tempDictionary)
                {
                    if (item.Value > 0)
                    {
                        StoredBox storedBox = db.StoredBoxes.Find(item.Key.StoredBoxPK);
                        Box box = db.Boxes.Find(storedBox.BoxPK);
                        Shelf shelf = db.Shelves.Find(storedBox.ShelfPK);
                        Row row = db.Rows.Find(shelf.RowPK);
                        result.Add(new Client_Box_Shelf_Row(box.BoxID, shelf.ShelfID, row.RowID, item.Key.ItemPK, item.Key.IsRestored, item.Value));
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }

        public List<Client_Box_Shelf_Row2> StoredBox_ItemPK_IsRestoredOfEntries2(Accessory accessory, int issuingSessionPK)
        {
            List<Client_Box_Shelf_Row2> result = new List<Client_Box_Shelf_Row2>();
            StoringDAO storingDAO = new StoringDAO();
            try
            {
                // cực phẩm IQ
                double inStoredQuantity = InStoredQuantity(accessory.AccessoryPK);
                if (inStoredQuantity == 0) throw new Exception("HÀNG TRONG KHO ĐÃ HẾT!");
                List<Entry> entries = (from e in db.Entries
                                       where e.AccessoryPK == accessory.AccessoryPK && e.SessionPK == issuingSessionPK && e.KindRoleName == "Issuing"
                                       select e).ToList();
                Dictionary<StoredBox_ItemPK_IsRestored, double> tempDictionary = new Dictionary<StoredBox_ItemPK_IsRestored, double>();
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

                        if (!tempDictionary.ContainsKey(key))
                        {
                            tempDictionary.Add(key, storingDAO.EntryQuantity(entry));
                        }
                        else
                        {
                            tempDictionary[key] += storingDAO.EntryQuantity(entry);
                        }
                    }
                }

                foreach (var item in tempDictionary)
                {
                    if (item.Value > 0)
                    {
                        StoredBox storedBox = db.StoredBoxes.Find(item.Key.StoredBoxPK);
                        Box box = db.Boxes.Find(storedBox.BoxPK);
                        Shelf shelf = db.Shelves.Find(storedBox.ShelfPK);
                        Row row = db.Rows.Find(shelf.RowPK);
                        result.Add(new Client_Box_Shelf_Row2(box.BoxID, shelf.ShelfID, row.RowID, item.Key.ItemPK, item.Key.IsRestored, item.Value));
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }

        public IssuingSession CreateIssuingSession(string userID, int requestPK, List<string> boxIDs)
        {
            try
            {
                string deactivatedBoxes = "";
                foreach (var boxID in boxIDs)
                {
                    deactivatedBoxes += boxID + "~!~";
                }
                IssuingSession issuingSession = new IssuingSession(userID, requestPK, deactivatedBoxes);
                db.IssuingSessions.Add(issuingSession);
                db.SaveChanges();
                issuingSession = (from iS in db.IssuingSessions.OrderByDescending(unit => unit.IssuingSessionPK)
                                  select iS).FirstOrDefault();
                return issuingSession;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteIssuingSession(int issuingSessionPK)
        {
            try
            {
                IssuingSession issuingSession = db.IssuingSessions.Find(issuingSessionPK);
                db.IssuingSessions.Remove(issuingSession);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteIssueEntries(int issuingSessionPK)
        {
            try
            {
                IssuingSession issuingSession = db.IssuingSessions.Find(issuingSessionPK);
                List<Entry> entries = (from e in db.Entries
                                       where e.KindRoleName == "Issuing" && e.SessionPK == issuingSessionPK
                                       select e).ToList();
                foreach (var entry in entries)
                {
                    db.Entries.Remove(entry);
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

