namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("IssuingSession")]
    public partial class IssuingSession
    {
        public IssuingSession()
        {
        }

        [Key]
        public int IssuingSessionPK { get; set; }

        public DateTime ExecutedDate { get; set; }

        [Required]
        [StringLength(50)]
        public string UserID { get; set; }

        public int RequestPK { get; set; }

        public string DeactivatedBoxes { get; set; }
    }
}
