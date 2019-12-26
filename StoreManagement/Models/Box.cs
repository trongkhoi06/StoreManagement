namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Box")]
    public partial class Box
    {
        public Box()
        {
        }

        public Box(string boxID)
        {
            BoxID = boxID;
            DateCreated = DateTime.Now;
            IsActive = true;
        }

        [Key]
        public int BoxPK { get; set; }

        [Required]
        [StringLength(100)]
        public string BoxID { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsActive { get; set; }
    }
}
