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

        [Key]
        public int RestoredItemPK { get; set; }

        public int AccessoryPK { get; set; }

        public double RestoredQuantity { get; set; }

        public string Comment { get; set; }

        public int RestorationPK { get; set; }
    }
}
