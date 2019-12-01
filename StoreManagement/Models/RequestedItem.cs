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

        public RequestedItem(double requestedQuantity, int requestPK, int demandedItemPK)
        {
            RequestedQuantity = requestedQuantity;
            RequestPK = requestPK;
            DemandedItemPK = demandedItemPK;
        }

        [Key]
        public int RequestedItemPK { get; set; }

        public double RequestedQuantity { get; set; }

        public int RequestPK { get; set; }

        public int DemandedItemPK { get; set; }
    }
}
