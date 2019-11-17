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
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClassifiedItemPK { get; set; }

        public int FinalQuantity { get; set; }
        
        public int QualityState { get; set; }

        public int PackedItemPK { get; set; }

        public ClassifiedItem()
        {
        }

        public ClassifiedItem(int qualityState, int finalQuantity, int packedItemPK)
        {
            FinalQuantity = finalQuantity;
            QualityState = qualityState;
            PackedItemPK = packedItemPK;
        }
    }
}
