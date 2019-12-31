using StoreManagement.Class;
using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace StoreManagement.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AccessingInventoryController : ApiController
    {
        private UserModel db = new UserModel();

        [Route("api/AccessingInventoryController/GetItemByBoxID")]
        [HttpGet]
        public IHttpActionResult GetItemByBoxID(string boxID)
        {
            BoxDAO boxDAO = new BoxDAO();
            StoringDAO storingDAO = new StoringDAO();
            try
            {
                Box box = boxDAO.GetBoxByBoxID(boxID);
                if (box != null)
                {
                    StoredBox sBox = boxDAO.GetStoredBoxbyBoxPK(box.BoxPK);
                    UnstoredBox uBox = boxDAO.GetUnstoredBoxbyBoxPK(box.BoxPK);
                    // Nếu box chưa identify
                    if (uBox.IsIdentified)
                    {
                        // nếu box chưa được store
                        if (!(boxDAO.IsStored(box.BoxPK)))
                        {
                            List<Client_IdentifiedItemRead> client_IdentifiedItems = new List<Client_IdentifiedItemRead>();
                            List<IdentifiedItem> identifiedItems;
                            identifiedItems = (from iI in db.IdentifiedItems.OrderByDescending(unit => unit.PackedItemPK)
                                               where iI.UnstoredBoxPK == uBox.UnstoredBoxPK
                                               select iI).ToList();

                            foreach (var identifiedItem in identifiedItems)
                            {
                                PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                                // lấy pack ID
                                Pack pack = db.Packs.Find(packedItem.PackPK);

                                // lấy phụ liệu tương ứng
                                OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);

                                Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);
                                // lấy qualityState
                                ClassifiedItem classifiedItem = (from cI in db.ClassifiedItems
                                                                 where cI.PackedItemPK == packedItem.PackedItemPK
                                                                 select cI).FirstOrDefault();
                                int? qualityState = null;
                                if (classifiedItem != null) qualityState = classifiedItem.QualityState;
                                client_IdentifiedItems.Add(new Client_IdentifiedItemRead(identifiedItem, accessory, pack.PackID, qualityState));
                            }
                            return Content(HttpStatusCode.OK, client_IdentifiedItems);
                        }
                        else
                        {
                            Client_InBoxItems_Shelf<Client_Shelf> result;
                            Client_Shelf client_Shelf;
                            List<Client_InBoxItem> client_InBoxItems = new List<Client_InBoxItem>();

                            Shelf shelf = db.Shelves.Find(sBox.ShelfPK);
                            Row row = db.Rows.Find(shelf.RowPK);
                            client_Shelf = new Client_Shelf(shelf.ShelfID, row.RowID);

                            // Get list inBoxItem
                            List<Entry> entries = (from e in db.Entries
                                                   where e.StoredBoxPK == sBox.StoredBoxPK
                                                   select e).ToList();
                            HashSet<KeyValuePair<int, bool>> listItemPK = new HashSet<KeyValuePair<int, bool>>();
                            foreach (var entry in entries)
                            {
                                listItemPK.Add(new KeyValuePair<int, bool>(entry.ItemPK, entry.IsRestored));
                            }
                            foreach (var itemPK in listItemPK)
                            {
                                List<Entry> tempEntries = new List<Entry>();
                                foreach (var entry in entries)
                                {
                                    if (entry.ItemPK == itemPK.Key && entry.IsRestored == itemPK.Value) tempEntries.Add(entry);
                                }
                                if (tempEntries.Count > 0 && storingDAO.EntriesQuantity(tempEntries) > 0)
                                {
                                    Entry entry = tempEntries[0];
                                    PassedItem passedItem;
                                    RestoredItem restoredItem;
                                    if (entry.IsRestored)
                                    {
                                        restoredItem = db.RestoredItems.Find(entry.ItemPK);
                                        Restoration restoration = db.Restorations.Find(restoredItem.RestorationPK);
                                        Accessory accessory = db.Accessories.Find(restoredItem.AccessoryPK);
                                        client_InBoxItems.Add(new Client_InBoxItem(accessory, restoration.RestorationID, storingDAO.EntriesQuantity(tempEntries), restoredItem.RestoredItemPK, true));
                                    }
                                    else
                                    {
                                        passedItem = db.PassedItems.Find(entry.ItemPK);
                                        ClassifiedItem classifiedItem = db.ClassifiedItems.Find(passedItem.ClassifiedItemPK);
                                        PackedItem packedItem = db.PackedItems.Find(classifiedItem.PackedItemPK);
                                        // lấy pack ID
                                        Pack pack = (from p in db.Packs
                                                     where p.PackPK == packedItem.PackPK
                                                     select p).FirstOrDefault();

                                        // lấy phụ liệu tương ứng
                                        OrderedItem orderedItem = (from oI in db.OrderedItems
                                                                   where oI.OrderedItemPK == packedItem.OrderedItemPK
                                                                   select oI).FirstOrDefault();

                                        Accessory accessory = (from a in db.Accessories
                                                               where a.AccessoryPK == orderedItem.AccessoryPK
                                                               select a).FirstOrDefault();
                                        client_InBoxItems.Add(new Client_InBoxItem(accessory, pack.PackID, storingDAO.EntriesQuantity(tempEntries), passedItem.PassedItemPK, false));
                                    }
                                }
                            }
                            result = new Client_InBoxItems_Shelf<Client_Shelf>(client_Shelf, client_InBoxItems);
                            return Content(HttpStatusCode.OK, result);

                        }
                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "THÙNG NÀY CHƯA ĐƯỢC GHI NHẬN");
                    }
                }
                else
                {
                    return Content(HttpStatusCode.Conflict, "ĐỐI TƯỢNG KHÔNG TỒN TẠI");
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        [Route("api/AccessingInventoryController/GetItemByShelfID")]
        [HttpGet]
        public IHttpActionResult GetItemByShelfID(string shelfID)
        {
            BoxDAO boxDAO = new BoxDAO();
            StoringDAO storingDAO = new StoringDAO();
            try
            {
                Client_InBoxItems_Box<List<string>> result;
                List<string> boxIDs = new List<string>();
                Dictionary<KeyValuePair<int, bool>, Client_InBoxItem> client_InBoxItems = new Dictionary<KeyValuePair<int, bool>, Client_InBoxItem>();

                Shelf shelf = (from sh in db.Shelves
                               where sh.ShelfID == shelfID
                               select sh).FirstOrDefault();
                if (shelf != null)
                {
                    List<StoredBox> sBoxes = (from sB in db.StoredBoxes
                                              where sB.ShelfPK == shelf.ShelfPK
                                              select sB).ToList();
                    if (sBoxes.Count == 0) return Content(HttpStatusCode.OK, "");
                    string rowID = db.Rows.Find(shelf.RowPK).RowID;
                    foreach (var sBox in sBoxes)
                    {
                        Box box = db.Boxes.Find(sBox.BoxPK); db.Boxes.Find(sBox.BoxPK);
                        boxIDs.Add(box.BoxID);

                        // Get list inBoxItem
                        List<Entry> entries = (from e in db.Entries
                                               where e.StoredBoxPK == sBox.StoredBoxPK
                                               select e).ToList();

                        // Hiện thực cặp value ko được trùng 2 key là itemPK và isRestored
                        HashSet<KeyValuePair<int, bool>> listItem = new HashSet<KeyValuePair<int, bool>>();
                        foreach (var entry in entries)
                        {
                            listItem.Add(new KeyValuePair<int, bool>(entry.ItemPK, entry.IsRestored));
                        }
                        foreach (var item in listItem)
                        {
                            List<Entry> tempEntries = new List<Entry>();
                            foreach (var entry in entries)
                            {
                                if (entry.ItemPK == item.Key && entry.IsRestored == item.Value) tempEntries.Add(entry);
                            }
                            if (tempEntries.Count > 0 && storingDAO.EntriesQuantity(tempEntries) > 0)
                            {
                                Entry entry = tempEntries[0];
                                PassedItem passedItem;
                                RestoredItem restoredItem;
                                if (item.Value)
                                {
                                    restoredItem = db.RestoredItems.Find(item.Key);
                                    Restoration restoration = db.Restorations.Find(restoredItem.RestorationPK);
                                    Accessory accessory = db.Accessories.Find(restoredItem.AccessoryPK);
                                    if (!client_InBoxItems.ContainsKey(item))
                                    {
                                        client_InBoxItems.Add(item, new Client_InBoxItem(accessory, restoration.RestorationID,
                                        storingDAO.EntriesQuantity(tempEntries), restoredItem.RestoredItemPK, item.Value));
                                    }
                                    else
                                    {
                                        client_InBoxItems[item].InBoxQuantity += storingDAO.EntriesQuantity(tempEntries);
                                    }

                                }
                                else
                                {
                                    passedItem = db.PassedItems.Find(item.Key);
                                    ClassifiedItem classifiedItem = db.ClassifiedItems.Find(passedItem.ClassifiedItemPK);
                                    PackedItem packedItem = db.PackedItems.Find(classifiedItem.PackedItemPK);
                                    // lấy pack ID
                                    Pack pack = db.Packs.Find(packedItem.PackPK);

                                    // lấy phụ liệu tương ứng
                                    OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);

                                    Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);
                                    if (!client_InBoxItems.ContainsKey(item))
                                    {
                                        client_InBoxItems.Add(item, new Client_InBoxItem(accessory, pack.PackID,
                                        storingDAO.EntriesQuantity(tempEntries), passedItem.PassedItemPK, item.Value));
                                    }
                                    else
                                    {
                                        client_InBoxItems[item].InBoxQuantity += storingDAO.EntriesQuantity(tempEntries);
                                    }
                                }
                            }
                        }

                    }

                    result = new Client_InBoxItems_Box<List<string>>(boxIDs, client_InBoxItems.Values.ToList(), rowID);
                    return Content(HttpStatusCode.OK, result);
                }
                else
                {
                    return Content(HttpStatusCode.Conflict, "ĐỐI TƯỢNG KHÔNG TỒN TẠI");
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        [Route("api/AccessingInventoryController/GetItemByRowID")]
        [HttpGet]
        public IHttpActionResult GetItemByRowID(string rowID)
        {
            BoxDAO boxDAO = new BoxDAO();
            StoringDAO storingDAO = new StoringDAO();
            try
            {
                Client_InBoxItems_Shelf<List<string>> result;
                List<string> shelfIDs = new List<string>();
                List<Shelf> shelves;
                Row row = (from r in db.Rows
                           where r.RowID == rowID
                           select r).FirstOrDefault();
                if (row != null || rowID == "TẠM THỜI")
                {
                    if (rowID == "TẠM THỜI")
                    {
                        shelves = (from sh in db.Shelves
                                   where sh.RowPK == null
                                   select sh).ToList();
                    }
                    else
                    {
                        shelves = (from sh in db.Shelves
                                   where sh.RowPK == row.RowPK
                                   select sh).ToList();
                    }
                    Dictionary<KeyValuePair<int, bool>, Client_InBoxItem> client_InBoxItems = new Dictionary<KeyValuePair<int, bool>, Client_InBoxItem>();
                    foreach (var shelf in shelves)
                    {
                        List<StoredBox> sBoxes = (from sB in db.StoredBoxes
                                                  where sB.ShelfPK == shelf.ShelfPK
                                                  select sB).ToList();

                        shelfIDs.Add(shelf.ShelfID);
                        foreach (var sBox in sBoxes)
                        {
                            // Get list inBoxItem
                            List<Entry> entries = (from e in db.Entries
                                                   where e.StoredBoxPK == sBox.StoredBoxPK
                                                   select e).ToList();

                            // Hiện thực cặp value ko được trùng 2 key là itemPK và isRestored
                            HashSet<KeyValuePair<int, bool>> listItem = new HashSet<KeyValuePair<int, bool>>();
                            foreach (var entry in entries)
                            {
                                listItem.Add(new KeyValuePair<int, bool>(entry.ItemPK, entry.IsRestored));
                            }
                            foreach (var item in listItem)
                            {
                                List<Entry> tempEntries = new List<Entry>();
                                foreach (var entry in entries)
                                {
                                    if (entry.ItemPK == item.Key && entry.IsRestored == item.Value) tempEntries.Add(entry);
                                }
                                if (tempEntries.Count > 0 && storingDAO.EntriesQuantity(tempEntries) > 0)
                                {
                                    Entry entry = tempEntries[0];
                                    PassedItem passedItem;
                                    RestoredItem restoredItem;
                                    if (item.Value)
                                    {
                                        restoredItem = db.RestoredItems.Find(item.Key);
                                        Restoration restoration = db.Restorations.Find(restoredItem.RestorationPK);
                                        Accessory accessory = db.Accessories.Find(restoredItem.AccessoryPK);
                                        if (!client_InBoxItems.ContainsKey(item))
                                        {
                                            client_InBoxItems.Add(item, new Client_InBoxItem(accessory, restoration.RestorationID,
                                            storingDAO.EntriesQuantity(tempEntries), restoredItem.RestoredItemPK, item.Value));
                                        }
                                        else
                                        {
                                            client_InBoxItems[item].InBoxQuantity += storingDAO.EntriesQuantity(tempEntries);
                                        }

                                    }
                                    else
                                    {
                                        passedItem = db.PassedItems.Find(item.Key);
                                        ClassifiedItem classifiedItem = db.ClassifiedItems.Find(passedItem.ClassifiedItemPK);
                                        PackedItem packedItem = db.PackedItems.Find(classifiedItem.PackedItemPK);
                                        // lấy pack ID
                                        Pack pack = db.Packs.Find(packedItem.PackPK);

                                        // lấy phụ liệu tương ứng
                                        OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);

                                        Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);
                                        if (!client_InBoxItems.ContainsKey(item))
                                        {
                                            client_InBoxItems.Add(item, new Client_InBoxItem(accessory, pack.PackID,
                                            storingDAO.EntriesQuantity(tempEntries), passedItem.PassedItemPK, item.Value));
                                        }
                                        else
                                        {
                                            client_InBoxItems[item].InBoxQuantity += storingDAO.EntriesQuantity(tempEntries);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    result = new Client_InBoxItems_Shelf<List<string>>(shelfIDs, client_InBoxItems.Values.ToList());
                    return Content(HttpStatusCode.OK, result);
                }
                else
                {
                    return Content(HttpStatusCode.Conflict, "ĐỐI TƯỢNG KHÔNG TỒN TẠI");
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        [Route("api/AccessingInventoryController/GetAllRowIsActive")]
        [HttpGet]
        public IHttpActionResult GetAllRowIsActive()
        {
            List<Row> result = new List<Row>();
            BoxDAO boxDAO = new BoxDAO();
            StoringDAO storingDAO = new StoringDAO();
            try
            {
                result = (from r in db.Rows
                          where r.IsActive
                          select r).ToList();
                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

    }
}
