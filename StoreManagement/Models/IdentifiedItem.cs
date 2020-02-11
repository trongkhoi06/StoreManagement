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
        public IdentifiedItem()
        {

        }

        public IdentifiedItem(double identifiedQuantity, int packedItemPK, int identifyingSessionPK, int unstoredBoxPK)
        {
            IdentifiedQuantity = identifiedQuantity;
            PackedItemPK = packedItemPK;
            IdentifyingSessionPK = identifyingSessionPK;
            UnstoredBoxPK = unstoredBoxPK;
            IsChecked = false;
            IsCounted = false;
            StoringSessionPK = null;
        }

        [Key]
        public int IdentifiedItemPK { get; set; }

        public double IdentifiedQuantity { get; set; }

        public bool IsChecked { get; set; }

        public bool IsCounted { get; set; }

        public int PackedItemPK { get; set; }

        public int IdentifyingSessionPK { get; set; }

        public int? UnstoredBoxPK { get; set; }

        public int? StoringSessionPK { get; set; }
    }
}
