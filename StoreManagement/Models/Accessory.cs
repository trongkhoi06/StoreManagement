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
        public string Name { get; set; }
    }
}
