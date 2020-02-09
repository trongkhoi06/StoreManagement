namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PassedItem")]
    public partial class PassedItem
    {
        public PassedItem()
        {
        }

        public PassedItem(int classifiedItemPK)
        {
            ClassifiedItemPK = classifiedItemPK;
            IsStored = false;
        }

        [Key]
        public int PassedItemPK { get; set; }

        public bool IsStored { get; set; }

        public int ClassifiedItemPK { get; set; }
    }
}
