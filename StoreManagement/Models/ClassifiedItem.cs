namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ClassifiedItem")]
    public partial class ClassifiedItem
    {
        public ClassifiedItem()
        {

        }

        public ClassifiedItem(double finalQuantity, int qualityState, int packedItemPK)
        {
            FinalQuantity = finalQuantity;
            QualityState = qualityState;
            PackedItemPK = packedItemPK;
        }

        [Key]
        public int ClassifiedItemPK { get; set; }

        public double FinalQuantity { get; set; }

        public int QualityState { get; set; }

        public int PackedItemPK { get; set; }
    }
}
