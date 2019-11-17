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
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FailedItemPK { get; set; }

        public bool IsReturned { get; set; }

        public int ClassifiedItemPK { get; set; }

        public FailedItem()
        {
        }

        public FailedItem(int classifiedItemPK)
        {
            ClassifiedItemPK = classifiedItemPK;
            IsReturned = false;
        }
    }
}
