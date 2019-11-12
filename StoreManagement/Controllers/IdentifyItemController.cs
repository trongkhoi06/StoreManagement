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
    public class IdentifyItemController
    {
        private UserModel db = new UserModel();

        public IdentifyingSession createdIdentifyingSession()
        {
            IdentifyingSession identifyingSession = new IdentifyingSession();
            db.IdentifyingSessions.Add(identifyingSession);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
            identifyingSession = (from iss in db.IdentifyingSessions.OrderByDescending(unit => unit.IdentifyingSessionPK)
                                  select iss).FirstOrDefault();
            return identifyingSession;
        }

        public void createIndentifyItem(IdentifiedItem item)
        {
            try
            {
                db.IdentifiedItems.Add(item);
                db.SaveChanges();
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        public void updateIsIdentifyUnstoreBox(UnstoredBox uBox)
        {
            uBox.IsIdentified = true;
            db.Entry(uBox).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
