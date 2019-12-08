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

        public void CreateBox(int boxKind, int boxPK)
        {
            try
            {
                UnstoredBox unstoredBox;
                //StoredBox storedBox;
                switch (boxKind)
                {
                    case 1:
                        unstoredBox = new UnstoredBox(boxPK, false);
                        db.UnstoredBoxes.Add(unstoredBox);
                        break;
                    case 2:
                        unstoredBox = new UnstoredBox(boxPK, true);
                        db.UnstoredBoxes.Add(unstoredBox);
                        break;
                    case 3:
                        break;
                    default:
                        break;
                }

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteBox(int boxPK, string userID)
        {
            try
            {
                Box box = db.Boxes.Find(boxPK);
                UnstoredBox unstoredBox = db.UnstoredBoxes.Find(box.BoxPK);
                db.UnstoredBoxes.Remove(unstoredBox);
                db.Boxes.Remove(box);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

