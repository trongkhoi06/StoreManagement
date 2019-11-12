namespace StoreManagement.Models
{
    using StoreManagement.Class;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PackedItem")]
    public partial class PackedItem
    {
        public PackedItem()
        {
        }

        public PackedItem(int PackPK, Client_OrderedItemPK_PackedQuantity_Comment temp)
        {
            this.PackPK = PackPK;
            this.PackedQuantity = temp.PackedQuantity;
            this.Comment = temp.Comment;
            this.OrderedItemPK = temp.OrderedItemPK;
            IsClassified = false;
        }

        public PackedItem(int packedItemPK, int packedQuantity, bool isClassified, string comment, int orderedItemPK, int packPK)
        {
            PackedItemPK = packedItemPK;
            PackedQuantity = packedQuantity;
            IsClassified = isClassified;
            Comment = comment;
            OrderedItemPK = orderedItemPK;
            PackPK = packPK;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PackedItemPK { get; set; }

        public int PackedQuantity { get; set; }

        public bool IsClassified { get; set; }

        [Required]
        public string Comment { get; set; }

        public int OrderedItemPK { get; set; }

        public int PackPK { get; set; }
    }
}
