namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Accessory")]
    public partial class Accessory
    {
        public Accessory()
        {
        }

        [Key]
        public int AccessoryPK { get; set; }

        [Required]
        public string AccessoryID { get; set; }

        [Required]
        public string AccessoryDescription { get; set; }

        public bool IsActive { get; set; }

        [Required]
        [StringLength(50)]
        public string Item { get; set; }

        [StringLength(50)]
        public string Art { get; set; }

        [StringLength(50)]
        public string Color { get; set; }

        public string Comment { get; set; }

        public string Image { get; set; }

        public int AccessoryTypePK { get; set; }

        public int SupplierPK { get; set; }
    }
}
