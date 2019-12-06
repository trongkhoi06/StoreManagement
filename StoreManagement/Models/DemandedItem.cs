namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DemandedItem")]
    public partial class DemandedItem
    {
        public DemandedItem()
        {
        }

        public DemandedItem(double demandedQuantity, string comment, int demandPK, int accessoryPK)
        {
            DemandedQuantity = demandedQuantity;
            Comment = comment;
            DemandPK = demandPK;
            AccessoryPK = accessoryPK;
        }

        [Key]
        public int DemandedItemPK { get; set; }

        public double DemandedQuantity { get; set; }

        public string Comment { get; set; }

        public int DemandPK { get; set; }

        public int AccessoryPK { get; set; }
    }
}
