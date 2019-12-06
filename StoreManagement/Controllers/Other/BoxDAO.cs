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
    public class BoxDAO
    {
        private UserModel db = new UserModel();

        public void UpdateIsIdentifyUnstoreBox(UnstoredBox uBox, bool value)
        {
            try
            {
                uBox.IsIdentified = value;
                db.Entry(uBox).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        public bool IsStored(int boxPK)
        {
            try
            {
                StoredBox storedBox = (from sB in db.StoredBoxes where sB.BoxPK == boxPK select sB).FirstOrDefault();
                if (storedBox != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Box GetBoxByBoxID(string boxID)
        {
            return (from b in db.Boxes
                    where b.BoxID == boxID
                    select b).FirstOrDefault();
        }

        public UnstoredBox GetUnstoredBoxbyBoxPK(int boxPK)
        {
            return (from uB in db.UnstoredBoxes
                    where uB.BoxPK == boxPK
                    select uB).FirstOrDefault();
        }

        public StoredBox GetStoredBoxbyBoxPK(int boxPK)
        {
            return (from sB in db.StoredBoxes
                    where sB.BoxPK == boxPK
                    select sB).FirstOrDefault();
        }

        public Shelf GetShelfByShelfID(string shelfID)
        {
            return (from sh in db.Shelves
                    where sh.ShelfID == shelfID
                    select sh).FirstOrDefault();
        }

        public void ChangeIsActiveBoxes(List<string> boxIDs, bool isActive)
        {
            foreach (var boxID in boxIDs)
            {
                Box box = GetBoxByBoxID(boxID);
                box.IsActive = isActive;
                db.Entry(box).State = EntityState.Modified;
            }
            db.SaveChanges();
        }
    }
}

