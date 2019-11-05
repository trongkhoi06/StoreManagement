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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DemandedItemID { get; set; }

        public int? DemandedQuantity { get; set; }

        public string Comment { get; set; }

        public int? DemandID { get; set; }
    }
}
