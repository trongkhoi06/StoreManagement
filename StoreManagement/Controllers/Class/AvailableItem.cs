using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Controllers.Class
{
    public class AvailableItem
    {
        private UserModel db = new UserModel();

        public AvailableItem(int storedBoxPK, int itemPK, bool isRestored)
        {
            StoredBoxPK = storedBoxPK;
            ItemPK = itemPK;
            IsRestored = isRestored;
            IsPending = false;

            List<Entry> entries = db.Entries.Where(unit => unit.StoredBoxPK == storedBoxPK && unit.ItemPK == itemPK
                                            && unit.IsRestored == isRestored).ToList();
            AccessoryPK = entries[0].AccessoryPK;

            StoringDAO storingDAO = new StoringDAO();
            AvailableQuantity = storingDAO.AvailableQuantity(db.StoredBoxes.Find(storedBoxPK), itemPK, isRestored);

            if (storingDAO.EntriesQuantity(entries) == -1)
            {
                IsPending = true;
            }
        }

        public int StoredBoxPK { get; set; }

        public int ItemPK { get; set; }

        public bool IsRestored { get; set; }

        public int AccessoryPK { get; set; }

        public double AvailableQuantity { get; set; }

        public bool IsPending { get; set; }
    }
}