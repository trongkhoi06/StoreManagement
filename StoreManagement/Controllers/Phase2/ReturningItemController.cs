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

        public void createReturningSession(ReturningSession returningSession)
        {
            try
            {
                db.ReturningSessions.Add(returningSession);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void updateReturningSession(ReturningSession returningSession)
        {
            try
            {
                db.Entry(returningSession).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void deleteReturningSession(int returningSessionPK)
        {
            try
            {
                ReturningSession returningSession = db.ReturningSessions.Find(returningSessionPK);
                db.ReturningSessions.Remove(returningSession);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

