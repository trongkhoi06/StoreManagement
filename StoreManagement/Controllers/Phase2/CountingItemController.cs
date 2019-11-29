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
    public class CountingItemController
    {
        private UserModel db = new UserModel();

        internal void createCountingSession(CountingSession countingSession)
        {
            try
            {
                db.CountingSessions.Add(countingSession);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void updateIsCountedOfIdentifiedItem(int identifiedItemPK,bool IsCounted)
        {
            try
            {
                IdentifiedItem identifiedItem = db.IdentifiedItems.Find(identifiedItemPK);
                identifiedItem.IsCounted = IsCounted;
                db.Entry(identifiedItem).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void updateCountingSession(int countingSessionPK, double countedQuantity)
        {
            try
            {
                CountingSession countingSession = db.CountingSessions.Find(countingSessionPK);
                countingSession.CountedQuantity = countedQuantity;
                countingSession.ExecutedDate = DateTime.Now;
                db.Entry(countingSession).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        internal void deleteCountingSession(int countingSessionPK)
        {
            try
            {
                CountingSession countingSession = db.CountingSessions.Find(countingSessionPK);
                db.CountingSessions.Remove(countingSession);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
