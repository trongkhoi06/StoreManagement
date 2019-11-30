namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Entry")]
    public partial class Entry
    {
        public Entry()
        {
        }

        public Entry(StoredBox storedBox, string kindRoleName, int sessionPK, bool isRestored, double quantity, int itemPK)
        {
            StoredBoxPK = storedBox.StoredBoxPK;
            KindRoleName = kindRoleName;
            SessionPK = sessionPK;
            IsRestored = isRestored;
            Quantity = quantity;
            ItemPK = itemPK;
        }

        [Key]
        public int EntryPK { get; set; }

        public int StoredBoxPK { get; set; }

        [Required]
        [StringLength(100)]
        public string KindRoleName { get; set; }

        public int SessionPK { get; set; }

        public bool IsRestored { get; set; }

        public double Quantity { get; set; }

        public int ItemPK { get; set; }
    }
}
