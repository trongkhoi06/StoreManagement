namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StoredBox")]
    public partial class StoredBox
    {
        public StoredBox()
        {
        }

        [Key]
        public int StoredBoxPK { get; set; }

        public int BoxPK { get; set; }

        public int ShelfPK { get; set; }
    }
}
