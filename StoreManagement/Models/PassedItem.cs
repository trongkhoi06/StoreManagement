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
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PassedItemPK { get; set; }

        public bool IsStored { get; set; }

        public int? CurrentQuantity { get; set; }

        public int ClassifiedItemPK { get; set; }

        public PassedItem()
        {
        }

        public PassedItem(int classifiedItemPK)
        {
            ClassifiedItemPK = classifiedItemPK;
            IsStored = false;
        }
    }
}
