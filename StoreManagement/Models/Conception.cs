namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Conception")]
    public partial class Conception
    {
        public Conception()
        {
        }

        [Key]
        public int ConceptionPK { get; set; }

        public int ConceptionCode { get; set; }

        public string Description { get; set; }

        public int Year { get; set; }

        [Required]
        [StringLength(50)]
        public string Season { get; set; }

        public bool IsActive { get; set; }
    }
}
