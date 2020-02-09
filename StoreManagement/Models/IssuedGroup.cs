namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("IssuedGroup")]
    public partial class IssuedGroup
    {
        [Key]
        public int IssuedGroupPK { get; set; }

        public double IssuedGroupQuantity { get; set; }

        public int IssuePK { get; set; }

        public int DemandedItemPK { get; set; }

        public int UnstoredBoxPK { get; set; }

        public int ItemPK { get; set; }
    }
}
