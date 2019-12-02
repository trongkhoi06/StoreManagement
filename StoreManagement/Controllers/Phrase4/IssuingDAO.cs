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

        public List<Client_Box_Shelf_Row> StoredBoxesOfEntries(Accessory accessory)
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
                HashSet<StoredBox> tempSBox = new HashSet<StoredBox>();
                Dictionary<StoredBox, double> tempDictionary = new Dictionary<StoredBox, double>();
                foreach (var entry in entries)
                {
                    StoredBox storedBox = db.StoredBoxes.Find(entry.StoredBoxPK);
                    Box box = db.Boxes.Find(storedBox.BoxPK);
                    if (box.IsActive)
                    {
                        tempSBox.Add(storedBox);
                        if (!tempDictionary.ContainsKey(storedBox))
                        {
                            tempDictionary.Add(storedBox, storingDAO.EntryQuantity(entry));
                        }
                        else
                        {
                            tempDictionary[storedBox] += storingDAO.EntryQuantity(entry);
                        }
                    }
                }

                foreach (var item in tempDictionary)
                {
                    if (item.Value > 0)
                    {
                        Box box = db.Boxes.Find(item.Key.BoxPK);
                        Shelf shelf = db.Shelves.Find(item.Key.ShelfPK);
                        Row row = db.Rows.Find(shelf.RowPK);
                        result.Add(new Client_Box_Shelf_Row(box.BoxID,shelf.ShelfID,row.RowID,item.Value));
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }
    }
}

