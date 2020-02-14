using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static StoreManagement.Controllers.IssuingController;

namespace StoreManagement.Controllers
{
    public class BoxDAO
    {
        private UserModel db = new UserModel();

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

        //public void ChangeIsActiveBoxes(List<string> boxIDs, bool isActive)
        //{
        //    if (boxIDs != null)
        //    {
        //        foreach (var boxID in boxIDs)
        //        {
        //            Box box = GetBoxByBoxID(boxID);

        //            // kiểm box empty
        //            StoredBox storedBox = GetStoredBoxbyBoxPK(box.BoxPK);

        //            List<Entry> entries = db.Entries.Where(e => e.StoredBoxPK == storedBox.StoredBoxPK).ToList();
        //            double quantity = 0;
        //            foreach (var entry in entries)
        //            {
        //                quantity += new StoringDAO().EntryQuantity(entry);
        //            }
        //            if (quantity != 0) throw new Exception("THÙNG CHƯA TRỐNG~AST-ERR~");
        //            box.IsActive = isActive;
        //            db.Entry(box).State = EntityState.Modified;
        //        }
        //    }
        //    db.SaveChanges();
        //}

        public Box CreateBox()
        {
            try
            {
                // delete accessory
                DateTime now = DateTime.Now;
                string tempDay = (now.Day + "").Length == 1 ? '0' + (now.Day + "") : (now.Day + "");
                string tempMonth = (now.Month + "").Length == 1 ? '0' + (now.Month + "") : (now.Month + "");
                string tempYear = (now.Year + "").Substring((now.Year + "").Length - 2);

                string boxID = tempDay + tempMonth + tempYear;
                Box box = (from b in db.Boxes.OrderByDescending(unit => unit.BoxPK)
                           where b.BoxID.Contains(boxID)
                           select b).FirstOrDefault();

                if (box == null)
                {
                    boxID += "001";
                }
                else
                {
                    int tempInt = Int32.Parse(box.BoxID.Substring(box.BoxID.Length - 6, 3)) + 1;
                    string tempStr = tempInt + "";
                    if (tempStr.Length == 1) boxID += "00" + tempStr;
                    if (tempStr.Length == 2) boxID += "0" + tempStr;
                    if (tempStr.Length == 3) boxID += tempStr;
                }
                boxID += "box";
                box = new Box(boxID);
                db.Boxes.Add(box);
                db.SaveChanges();
                return db.Boxes.OrderByDescending(unit => unit.BoxPK).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void CreateUnstoredBoxStoredBox(int boxPK)
        {
            try
            {
                UnstoredBox unstoredBox = new UnstoredBox(boxPK);
                db.UnstoredBoxes.Add(unstoredBox);
                StoredBox storedBox = new StoredBox(boxPK, null);
                db.StoredBoxes.Add(storedBox);
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
                StoredBox sBox = db.StoredBoxes.Where(unit => unit.BoxPK == box.BoxPK).FirstOrDefault();

                box.IsActive = false;
                db.Entry(box).State = EntityState.Modified;

                sBox.ShelfPK = null;
                db.Entry(sBox).State = EntityState.Modified;

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

        public bool IsStoredBoxContainItem(StoredBox sBox)
        {
            List<Entry> entries = db.Entries.Where(unit => unit.StoredBoxPK == sBox.StoredBoxPK).ToList();
            Dictionary<Tuple<int, bool>, double> allInBoxItems = new Dictionary<Tuple<int, bool>, double>();
            StoringDAO storingDAO = new StoringDAO();
            foreach (var entry in entries)
            {
                Tuple<int, bool> key = new Tuple<int, bool>(entry.ItemPK, entry.IsRestored);
                if (!allInBoxItems.ContainsKey(key))
                {
                    allInBoxItems.Add(key, storingDAO.InBoxQuantity(sBox, key.Item1, key.Item2));
                }
                else
                {
                    allInBoxItems[key] += storingDAO.InBoxQuantity(sBox, key.Item1, key.Item2);
                }
            }
            foreach (var inBoxItem in allInBoxItems)
            {
                if (inBoxItem.Value > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsUnstoredCase(int boxPK)
        {
            try
            {
                StoringDAO storingDAO = new StoringDAO();
                StoredBox sBox = db.StoredBoxes.Where(unit => unit.BoxPK == boxPK).FirstOrDefault();
                UnstoredBox uBox = db.UnstoredBoxes.Where(unit => unit.BoxPK == boxPK).FirstOrDefault();
                List<Entry> entries = db.Entries.Where(unit => unit.StoredBoxPK == sBox.StoredBoxPK).ToList();
                if (storingDAO.EntriesQuantity(entries) == 0 &&
                    db.IssuedGroups.Where(unit => unit.UnstoredBoxPK == uBox.UnstoredBoxPK).FirstOrDefault() == null)
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

        public bool IsStoredCase(int boxPK)
        {
            try
            {
                UnstoredBox uBox = db.UnstoredBoxes.Where(unit => unit.BoxPK == boxPK).FirstOrDefault();
                if (db.IssuedGroups.Where(unit => unit.UnstoredBoxPK == uBox.UnstoredBoxPK).FirstOrDefault() == null &&
                    db.RestoredGroups.Where(unit => unit.UnstoredBoxPK == uBox.UnstoredBoxPK).FirstOrDefault() == null &&
                    db.IdentifiedItems.Where(unit => unit.UnstoredBoxPK == uBox.UnstoredBoxPK).FirstOrDefault() == null)
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

        public bool IsEmptyCase(int boxPK)
        {
            try
            {
                StoringDAO storingDAO = new StoringDAO();
                StoredBox sBox = db.StoredBoxes.Where(unit => unit.BoxPK == boxPK).FirstOrDefault();
                UnstoredBox uBox = db.UnstoredBoxes.Where(unit => unit.BoxPK == boxPK).FirstOrDefault();
                List<Entry> entries = db.Entries.Where(unit => unit.StoredBoxPK == sBox.StoredBoxPK).ToList();
                if (storingDAO.EntriesQuantity(entries) == 0 &&
                    db.IssuedGroups.Where(unit => unit.UnstoredBoxPK == uBox.UnstoredBoxPK).FirstOrDefault() == null &&
                    db.RestoredGroups.Where(unit => unit.UnstoredBoxPK == uBox.UnstoredBoxPK).FirstOrDefault() == null &&
                    db.IdentifiedItems.Where(unit => unit.UnstoredBoxPK == uBox.UnstoredBoxPK).FirstOrDefault() == null)
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
    }
}

