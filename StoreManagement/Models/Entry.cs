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

        [Key]
        public int EntryPK { get; set; }

        public int StoredBoxPK { get; set; }

        public bool isApproved { get; set; }

        [Required]
        [StringLength(100)]
        public string KindRoleName { get; set; }

        public int SessionPK { get; set; }

        public bool IsRestored { get; set; }
    }
}
