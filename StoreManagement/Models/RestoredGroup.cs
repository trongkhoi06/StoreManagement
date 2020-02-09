namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RestoredGroup")]
    public partial class RestoredGroup
    {
        [Key]
        public int RestoredGroupPK { get; set; }

        public double GroupQuantity { get; set; }

        public int RestoredItemPK { get; set; }

        public int UnstoredBoxPK { get; set; }
    }
}
