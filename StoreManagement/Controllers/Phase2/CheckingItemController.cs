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
    public class CheckingItemController
    {
        private UserModel db = new UserModel();

        internal void createCheckingSession(CheckingSession checkingSession)
        {
            try
            {
                db.CheckingSessions.Add(checkingSession);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void updateIsCheckedOfIdentifiedItem(int identifiedItemPK, bool IsChecked)
        {
            try
            {
                IdentifiedItem identifiedItem = db.IdentifiedItems.Find(identifiedItemPK);
                identifiedItem.IsChecked = IsChecked;
                db.Entry(identifiedItem).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void updateCheckingSession(int checkingSessionPK, int checkedQuantity, int unqualifiedQuantity)
        {
            try
            {
                CheckingSession checkingSession = db.CheckingSessions.Find(checkingSessionPK);
                checkingSession.CheckedQuantity = checkedQuantity;
                checkingSession.UnqualifiedQuantity = unqualifiedQuantity;
                checkingSession.ExecutedDate = DateTime.Now;
                db.Entry(checkingSession).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void deleteCheckingSession(int checkingSessionPK)
        {
            try
            {
                CheckingSession checkingSession = db.CheckingSessions.Find(checkingSessionPK);
                db.CheckingSessions.Remove(checkingSession);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

}
