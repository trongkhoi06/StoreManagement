namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Activity")]
    public partial class Activity
    {
        [Key]
        public int ActivityPK { get; set; }

        [Required]
        [StringLength(50)]
        public string Action { get; set; }

        [Required]
        [StringLength(50)]
        public string ObjectID { get; set; }

        [Required]
        [StringLength(50)]
        public string Object { get; set; }

        public DateTime ExecutedDate { get; set; }

        [Required]
        [StringLength(50)]
        public string UserID { get; set; }
    }
}
