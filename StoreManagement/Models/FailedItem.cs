namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FailedItem")]
    public partial class FailedItem
    {
        public FailedItem()
        {

        }

        public FailedItem(int classifiedItemPK)
        {
            ClassifiedItemPK = classifiedItemPK;
            IsReturned = false;
        }

        [Key]
        public int FailedItemPK { get; set; }

        public bool IsReturned { get; set; }

        public int ClassifiedItemPK { get; set; }
    }
}
