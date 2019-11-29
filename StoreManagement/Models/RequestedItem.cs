namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RequestedItem")]
    public partial class RequestedItem
    {
        public RequestedItem()
        {
        }

        [Key]
        public int RequestedItemPK { get; set; }

        public int RequestedQuantity { get; set; }

        public int RequestPK { get; set; }

        public int DemandedItemPK { get; set; }
    }
}
