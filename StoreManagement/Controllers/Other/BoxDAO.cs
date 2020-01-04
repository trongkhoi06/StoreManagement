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
                    where sh.ShelfID == shelfID && sh.ShelfID != "InvisibleShelf"
                    select sh).FirstOrDefault();
        }

        public void ChangeIsActiveBoxes(List<string> boxIDs, bool isActive)
        {
            if (boxIDs != null)
                foreach (var boxID in boxIDs)
                {
                    Box box = GetBoxByBoxID(boxID);
                    box.IsActive = isActive;
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
                box.IsActive = false;
                db.Entry(box).State = EntityState.Modified;
                StoredBox sBox = db.StoredBoxes.Where(unit => unit.BoxPK == box.BoxPK).FirstOrDefault();
                if (sBox != null)
                {
                    sBox.ShelfPK = db.Shelves.Where(unit => unit.ShelfID == "InvisibleShelf").FirstOrDefault().ShelfPK;
                    db.Entry(sBox).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Row CreateRow(string rowID, int floor, int col)
        {
            try
            {
                Row row = new Row(rowID, false, floor, col);
                db.Rows.Add(row);
                db.SaveChanges();
                row = db.Rows.OrderByDescending(unit => unit.RowPK).FirstOrDefault();
                return row;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteRow(int rowPK)
        {
            try
            {
                Row row = db.Rows.Find(rowPK);
                List<Shelf> shelves = db.Shelves.Where(unit => unit.RowPK == row.RowPK).ToList();
                bool isDeletable = true;
                foreach (var shelf in shelves)
                {
                    List<StoredBox> storedBoxes = db.StoredBoxes.Where(unit => unit.ShelfPK == shelf.ShelfPK).ToList();
                    if (storedBoxes.Count > 0)
                    {
                        isDeletable = false;
                        break;
                    }
                }
                if (isDeletable)
                {
                    foreach (var shelf in shelves)
                    {
                        db.Shelves.Remove(shelf);
                    }
                    row.IsActive = false;
                    db.Entry(row).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    throw new Exception("KHÔNG THỂ XÓA DÃY VÌ TẤT CẢ KỆ TRONG DÃY CHƯA TRỐNG");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

