namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RestoredItem")]
    public partial class RestoredItem
    {
        public RestoredItem()
        {

        }

        public RestoredItem(int accessoryPK, double restoredQuantity, int restorationPK)
        {
            AccessoryPK = accessoryPK;
            RestoredQuantity = restoredQuantity;
            RestorationPK = restorationPK;
        }

        [Key]
        public int RestoredItemPK { get; set; }

        public int AccessoryPK { get; set; }

        public double RestoredQuantity { get; set; }

        public int RestorationPK { get; set; }
    }
}
