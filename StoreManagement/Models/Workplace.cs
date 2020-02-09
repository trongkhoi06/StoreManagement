namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Workplace")]
    public partial class Workplace
    {
        public Workplace()
        {

        }

        [Key]
        public int WorkplacePK { get; set; }

        [Required]
        [StringLength(50)]
        public string WorkplaceID { get; set; }

        public bool IsHost { get; set; }
    }
}
