namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Shelf")]
    public partial class Shelf
    {
        public Shelf()
        {
        }

        [Key]
        public int ShelfPK { get; set; }

        [Required]
        [StringLength(50)]
        public string ShelfName { get; set; }

        public int? Row { get; set; }
    }
}
