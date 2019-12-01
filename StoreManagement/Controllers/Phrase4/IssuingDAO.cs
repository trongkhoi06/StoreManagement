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

        public void createRequestedItems(List<Client_DemandedItemPK_RequestedQuantity> list, int requestPK)
        {
            try
            {
                foreach (var item in list)
                {
                    RequestedItem requestedItem = new RequestedItem(item.RequestedQuantity,requestPK,item.DemandedItemPK);
                    db.RequestedItems.Add(requestedItem);
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Request CreateRequest(string requestID, DateTime expectedDate, bool isIssued, bool isConfirmed, string comment, int demandPK)
        {
            try
            {
                Request request = new Request(requestID, expectedDate, isIssued, isConfirmed, comment, demandPK);
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
    }
}

