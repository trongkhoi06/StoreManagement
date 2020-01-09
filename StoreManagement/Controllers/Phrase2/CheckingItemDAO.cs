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
    public class CheckingItemDAO
    {
        private UserModel db = new UserModel();

        internal void createCheckingSession(CheckingSession checkingSession, int identifiedItemPK)
        {
            try
            {
                if (checkingSession.Comment.Length > 50)
                {
                    throw new Exception(SystemMessage.NotPassPrimitiveType);
                }
                // create checking session
                db.CheckingSessions.Add(checkingSession);

                // update identifiedItem IsChecked
                IdentifiedItem identifiedItem = db.IdentifiedItems.Find(identifiedItemPK);
                identifiedItem.IsChecked = false;
                db.Entry(identifiedItem).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void updateCheckingSession(int checkingSessionPK, double checkedQuantity, double unqualifiedQuantity, string comment)
        {
            try
            {
                if (comment.Length > 50)
                {
                    throw new Exception(SystemMessage.NotPassPrimitiveType);
                }
                CheckingSession checkingSession = db.CheckingSessions.Find(checkingSessionPK);
                checkingSession.CheckedQuantity = checkedQuantity;
                checkingSession.UnqualifiedQuantity = unqualifiedQuantity;
                checkingSession.ExecutedDate = DateTime.Now;
                checkingSession.Comment = comment;
                db.Entry(checkingSession).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void deleteCheckingSession(int checkingSessionPK, int identifiedItemPK)
        {
            try
            {
                // delete checking session
                CheckingSession checkingSession = db.CheckingSessions.Find(checkingSessionPK);
                db.CheckingSessions.Remove(checkingSession);

                // update identifiedItem IsChecked
                IdentifiedItem identifiedItem = db.IdentifiedItems.Find(identifiedItemPK);
                identifiedItem.IsChecked = false;
                db.Entry(identifiedItem).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

}
