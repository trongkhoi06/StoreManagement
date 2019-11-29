namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Verification")]
    public partial class Verification
    {
        public Verification()
        {
        }

        [Key]
        public int VerificationPK { get; set; }

        public DateTime ExecutedDate { get; set; }

        public bool IsApproved { get; set; }

        [Required]
        [StringLength(50)]
        public string UserID { get; set; }

        public bool IsDiscard { get; set; }

        public int SessionPK { get; set; }
    }
}
