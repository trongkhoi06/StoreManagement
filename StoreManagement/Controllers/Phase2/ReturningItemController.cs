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
    public class ReturningItemController
    {
        private UserModel db = new UserModel();

        public void createReturningSession(int failedItemPK, string employeeCode)
        {
            try
            {
                ReturningSession returningSession = new ReturningSession(failedItemPK,employeeCode);
                db.ReturningSessions.Add(returningSession);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void updateFailedItemIsReturned(int failedItemPK)
        {
            try
            {
                FailedItem failedItem = db.FailedItems.Find(failedItemPK);
                failedItem.IsReturned = true;
                db.Entry(failedItem).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void updateAllIdentifiedItemsToVirtualBox(int failedItemPK)
        {
            try
            {
                FailedItem failedItem = db.FailedItems.Find(failedItemPK);
                ClassifiedItem classifiedItem = db.ClassifiedItems.Find(failedItem.ClassifiedItemPK);
                PackedItem packedItem = db.PackedItems.Find(classifiedItem.PackedItemPK);
                List<IdentifiedItem> identifiedItems = (from iI in db.IdentifiedItems
                                                        where iI.PackedItemPK == packedItem.PackedItemPK
                                                        select iI).ToList();
                UnstoredBox virtualBox = (from uB in db.UnstoredBoxes
                                           where uB.BoxPK == (from b in db.Boxes
                                                              where b.BoxID == "InvisibleBox"
                                                              select b).FirstOrDefault().BoxPK
                                           select uB).FirstOrDefault();

                foreach (var item in identifiedItems)
                {
                    item.UnstoredBoxPK = virtualBox.UnstoredBoxPK;
                    db.Entry(item).State = EntityState.Modified;
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

