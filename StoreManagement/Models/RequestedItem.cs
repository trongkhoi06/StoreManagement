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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RequestedItemID { get; set; }

        public int? RequestedQuantity { get; set; }

        public int? RequestID { get; set; }

        public virtual Request Request { get; set; }
    }
}
