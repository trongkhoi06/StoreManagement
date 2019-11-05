namespace StoreManagement.Models
{
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
