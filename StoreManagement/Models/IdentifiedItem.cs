namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("IdentifiedItem")]
    public partial class IdentifiedItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdentifiedItemPK { get; set; }

        public bool IsChecked { get; set; }

        public bool IsCounted { get; set; }

        public int ActualQuantity { get; set; }

        public int PackedItemPK { get; set; }

        public int? IdentifyingSessionPK { get; set; }
    }
}
